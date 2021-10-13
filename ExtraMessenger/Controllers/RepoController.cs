using ExtraMessenger.Data;
using ExtraMessenger.DTOs;
using ExtraMessenger.Services.Github.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
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

        [HttpGet("getrepos")]
        public async Task<IActionResult> GetRepos()
        {
            ObjectId currentUser = ObjectId.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var client = _githubClientService.GetGitHubClient();
            var result = await client.Repository.GetAllForCurrent();
            
            // store in Mongo
            var mongoDb = _mongoService.GetDb;
            var mongoSettings = _mongoService.GetDBSettings;
            var reposCollection = mongoDb.GetCollection<Repository>(mongoSettings.ReposCollectionName);
            await reposCollection.InsertManyAsync(result);

            // store in Neo4J
            await _neoContext.Cypher
                .Unwind(result, "repo")
                .Merge("(r:Repo { Id: repo.Id, Name: repo.Name })")
                .With("(r)")
                .Match($"(u:User {{Id:{0}}})", currentUser.ToString())
                .Merge("((u)-[rel:OWNS]->(r)<-[w:WRITTEN_IN]-(l:Language { Name: repo.Language}))")
                .ExecuteWithoutResultsAsync();
            

            return Ok(result.Select(x => new RepoDTO { 
                Id = x.Id, 
                Name = x.Name, 
                Owner = x.Owner.Name, 
                OwnerId = currentUser.ToString(), 
                Language = x.Language}));
        }
    }
}
