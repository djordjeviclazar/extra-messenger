using ExtraMessenger.Data;
using ExtraMessenger.DTOs;
using ExtraMessenger.Services.Github.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Neo4jClient;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ExtraMessenger.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RepoController: ControllerBase
    {
        private readonly IGraphClient _neoContext;
        private readonly MongoService _mongoService;
        private readonly IGithubClientService _githubClientService;

        public RepoController(IGraphClient graphClient,
            MongoService mongoService,
            IGithubClientService githubClientService)
        {
            _neoContext = graphClient;
            _mongoService = mongoService;
            _githubClientService = githubClientService;
        }

        [HttpGet("fetchrepos")]
        public async Task<IActionResult> FetchRepos()
        {
            ObjectId currentUser = ObjectId.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var client = _githubClientService.GetGitHubClient(currentUser);

            // Get last fetched time:
            var mongoDb = _mongoService.GetDb;
            var mongoSettings = _mongoService.GetDBSettings;
            var userCollection = mongoDb.GetCollection<Models.User>(mongoSettings.UserCollectionName);
            var filterUser = Builders<Models.User>.Filter.Eq(u => u.Id, currentUser);
            var user = (await userCollection.FindAsync<Models.User>(filterUser)).FirstOrDefault();
            //Fetch:
            List<Repository> resultList = new List<Repository>();
            
            if (user.LastFetchedRepo != null)
            {
                SearchRepositoriesRequest request;
                SearchRepositoryResult result;
                DateTime date = user.LastFetchedRepo ?? DateTime.UtcNow;
                request = new SearchRepositoriesRequest() { Created = DateRange.GreaterThan(date) };
                do
                {
                    result = await client.Search.SearchRepo(request);
                    resultList = result.Items.ToList();
                } while (result.TotalCount > resultList.Count);
            }
            else
            {
                resultList = (await client.Repository.GetAllForCurrent()).ToList();
            }

            DateTime utcNow = DateTime.UtcNow;
            if (resultList.Count > 0)
            {
                // store in Mongo
                var reposCollection = mongoDb.GetCollection<Repository>(mongoSettings.ReposCollectionName);
                var updates = new List<WriteModel<Repository>>();
                foreach (var r in resultList)
                {
                    var filterRepo = Builders<Repository>.Filter.Eq(u => u.Id, r.Id);
                    updates.Add(new ReplaceOneModel<Repository>(filterRepo, r) { IsUpsert = true });
                }
                await reposCollection.BulkWriteAsync(updates, new BulkWriteOptions { IsOrdered = false });
                //await reposCollection.UpdateManyAsync(resultList, ).InsertManyAsync(resultList);

                var updateUser = Builders<Models.User>.Update.Set(u => u.LastFetchedRepo, utcNow);
                await userCollection.UpdateOneAsync(filterUser, updateUser); // update date
                var updateUserRepositories = Builders<Models.User>.Update.PushEach<Repository>(u => u.Repositories, resultList);
                await userCollection.UpdateManyAsync(filterUser, updateUserRepositories); // update Repository list of user
                
                // store in Neo4J
                var query = _neoContext.Cypher
                    .Unwind(resultList, "repo")
                    .Merge("(l:Language {Name: coalesce(repo.Language, 'N/A')})")
                    .OnCreate().Set("l.Name = coalesce(repo.Language, 'N/A')")
                    .Merge("(r:Repo { Id: repo.Id, Name: repo.Name })-[w:WRITTEN_IN]->(l)")
                    .With("(r)")
                    .Match("(u:User {Id:'" + currentUser.ToString() + "'})")
                    .Merge("(u)-[rel:OWNS]->(r)");
                //var queryText = query.Query.DebugQueryText;
                await query.ExecuteWithoutResultsAsync();


                return Ok(resultList.Select(x => new RepoDTO
                {
                    Id = x.Id,
                    OpenIssues = x.OpenIssuesCount,
                    IsPublic = !x.Private,
                    Name = x.Name,
                    Owner = x.Owner.Name,
                    OwnerId = currentUser.ToString(),
                    Language = x.Language
                }));
            }

            return Ok(resultList);
        }

        [HttpGet("getrepos")]
        public async Task<IActionResult> GetRepos()
        {
            ObjectId currentUser = ObjectId.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var client = _githubClientService.GetGitHubClient(currentUser);

            
            var mongoDb = _mongoService.GetDb;
            var mongoSettings = _mongoService.GetDBSettings;
            var userCollection = mongoDb.GetCollection<Models.User>(mongoSettings.UserCollectionName);
            var filterUser = Builders<Models.User>.Filter.Eq(u => u.Id, currentUser);
            var user = (await userCollection.FindAsync<Models.User>(filterUser)).FirstOrDefault();

            return Ok(user.Repositories.Select(x => new RepoDTO
            {
                Id = x.Id,
                OpenIssues = x.OpenIssuesCount,
                IsPublic = !x.Private,
                Name = x.Name,
                Owner = x.Owner.Name,
                OwnerId = currentUser.ToString(),
                Language = x.Language
            }));
        }
    }
}
