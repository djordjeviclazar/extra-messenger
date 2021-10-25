using ExtraMessenger.Data;
using ExtraMessenger.DTOs;
using ExtraMessenger.Models.Nodes;
using ExtraMessenger.Services.Github.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Neo4jClient;
using Newtonsoft.Json.Linq;
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
    public class RepoController : ControllerBase
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
                RepoUrl = x.HtmlUrl,
                OpenIssues = x.OpenIssuesCount,
                IsPublic = !x.Private,
                Name = x.Name,
                Owner = x.Owner.Name,
                OwnerId = currentUser.ToString(),
                Language = x.Language
            }));
        }

        [HttpGet("getleaningbranches/{name}")]
        public async Task<IActionResult> GetLeaningBranches(string name)
        {
            ObjectId currentUser = ObjectId.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            return Ok();
        }

        [HttpGet("getleaningbranches/{name}")]
        public async Task<IActionResult> GetAffectedBranches(string name)
        {
            ObjectId currentUser = ObjectId.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            return Ok();
        }

        [HttpGet("getbranches/{repoId}")]
        public async Task<IActionResult> GetBranches(long repoId)
        {
            ObjectId currentUser = ObjectId.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var query = _neoContext.Cypher
                    .Match("(r:Repo {Id:" + repoId + "})-[rel:HAS_BRANCH]->(b:Branch)")
                    .With("b.Name AS name")
                    .Return((name) => new { Name = name.As<string>() })
                    ;
            var branchGet = query.Query.DebugQueryText;
            var branches = await query.ResultsAsync;

            return Ok(branches.ToList());
        }

        [HttpPut("fetchissues /{repoId}")]
        public async Task<IActionResult> FetchIssues(long repoId)
        {
            ObjectId currentUser = ObjectId.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var client = _githubClientService.GetGitHubClient(currentUser);

            var mongoDb = _mongoService.GetDb;
            var mongoSettings = _mongoService.GetDBSettings;
            var repoCollection = mongoDb.GetCollection<Repository>(mongoSettings.ReposCollectionName);
            var filterRepo = Builders<Repository>.Filter.Eq(r => r.Id, repoId);
            var repo = (await repoCollection.FindAsync<Repository>(filterRepo)).FirstOrDefault();
            var userCollection = mongoDb.GetCollection<Models.User>(mongoSettings.UserCollectionName);
            var filterUser = Builders<Models.User>.Filter.Eq(u => u.Id, currentUser);
            var user = (await userCollection.FindAsync<Models.User>(filterUser)).FirstOrDefault();

            var issues = await _githubClientService.GetIssues(currentUser, user.GithubLogin, repo.Name);

            DateTime utcNow = DateTime.UtcNow;
            if (issues.Count > 0)
            {
                // store in Mongo
                var issuesCollection = mongoDb.GetCollection<Issue>(mongoSettings.IssuesCollectionName);
                var updates = new List<WriteModel<Issue>>();
                foreach (var issue in issues)
                {
                    var filterIssue = Builders<Issue>.Filter.Eq(x => x.Id, issue.Id);
                    updates.Add(new ReplaceOneModel<Issue>(filterIssue, issue) { IsUpsert = true });

                    // store in Neo4J:
                    var issueState = issue.State == "open" ? "Open" : issue.State == "closed" ? "Closed" : "Reopen";
                    DateTime dateTime = issue.CreatedAt.UtcDateTime;
                    var createdString = dateTime.ToString("yyyy-MM-dd") + "T" + dateTime.ToString("HH:mm:ss.ff") + "Z";
                    var query = _neoContext.Cypher
                    .Merge($"(i:Issue {{Id: {issue.Id}}})")
                    .OnCreate().Set($"i.Number = {issue.Number}, i.Id = {issue.Id}), i.State = '{issueState}'")
                    .OnMatch().Set($"i.State = '{issueState}'")
                    .Merge($"(r:Repo {{ Id: {repoId} }})-[w:CREATED_ISSUE {{Time: datetime('{createdString}')}}]->(i)");
                    var queryText = query.Query.DebugQueryText;
                    await query.ExecuteWithoutResultsAsync();

                    // add references:
                    var issueEvents = await _githubClientService.GetIssueEvents(user.GithubLogin, repo.Name, user.OAuthToken, issue.Number + "");
                    foreach (var iEvent in issueEvents)
                    {
                        var id = ((JValue)iEvent["id"]).ToString();
                        var eventType = ((JValue)iEvent["event"]).ToString();
                        var created = iEvent.Value<DateTime>("created_at");
                        var createdEventString = created.ToString("yyyy-MM-dd") + "T" + created.ToString("HH:mm:ss") + "Z";
                        switch (eventType)
                        {
                            case "referenced":
                                var commitId = iEvent["commit_id"].ToString();
                                var eventQuery = _neoContext.Cypher
                                    .Match($"(c:CommitPush {{Sha: '{id}'}})")
                                    .Match($"i:Issue {{Id: {issue.Id}}}")
                                    .Merge($"(i)-[:REFERENCED]->(c)");
                                var eventQueryText = eventQuery.Query.DebugQueryText;
                                await eventQuery.ExecuteWithoutResultsAsync();
                                break;
                            default:
                                break;
                        }
                    }
                }
                await issuesCollection.BulkWriteAsync(updates, new BulkWriteOptions { IsOrdered = false });

                // store in Neo4J
                
            }
            return Ok();
        }

        [HttpGet("fetchrepoinfo/{repoId}")]
        public async Task<IActionResult> FetchRepoInfo(long repoId)
        {
            ObjectId currentUser = ObjectId.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var client = _githubClientService.GetGitHubClient(currentUser);

            var mongoDb = _mongoService.GetDb;
            var mongoSettings = _mongoService.GetDBSettings;
            var repoCollection = mongoDb.GetCollection<Repository>(mongoSettings.ReposCollectionName);
            var filterRepo = Builders<Repository>.Filter.Eq(r => r.Id, repoId);
            var repo = (await repoCollection.FindAsync<Repository>(filterRepo)).FirstOrDefault();
            var userCollection = mongoDb.GetCollection<Models.User>(mongoSettings.UserCollectionName);
            var filterUser = Builders<Models.User>.Filter.Eq(u => u.Id, currentUser);
            var user = (await userCollection.FindAsync<Models.User>(filterUser)).FirstOrDefault();

            var branches = await _githubClientService.GetBranches(currentUser, repo);
            var query = _neoContext.Cypher
                    .Unwind(branches, "branch")
                    .Match("(r:Repo {Id:" + repo.Id + "})")
                    .Merge("(b:Branch {Name: branch.Name})")
                    .OnCreate().Set("b.Name = branch.Name")
                    .Merge("(r)-[rel:HAS_BRANCH]->(b)")
                    ;
            var branchCreate = query.Query.DebugQueryText;
            await query.ExecuteWithoutResultsAsync();

            List<PullRequestEvent> pullRequestEvents = new List<PullRequestEvent>();
            var gitEventsJsonArray = await _githubClientService.GetRepoEvents(user.GithubLogin, repo.Name, user.OAuthToken);
            foreach (var a in gitEventsJsonArray)
            {
                var type = ((JValue)a["type"]).ToString();
                var payload = ((JObject)a["payload"]);
                var created = a.Value<DateTime>("created_at");
                var createdString = created.ToString("yyyy-MM-dd") + "T" + created.ToString("HH:mm:ss") + "Z";
                switch (type)
                {
                    case "PushEvent":
                        var branch = payload["ref"].ToString();
                        var pom = branch.Split("/");
                        var branchName = pom[pom.Length - 1]; //
                        PushEvent pushEvent = new PushEvent
                        {
                            Id = ((JValue)a["id"]).ToString(),
                            Created = createdString
                        };
                        List<ComitPushNode> commits = new List<ComitPushNode>();
                        foreach(var c in (JArray)payload["commits"])
                        {
                            var commitAuthor = ((JObject)c["author"]);
                            ComitPushNode comitPushNode = new ComitPushNode
                            {
                                Sha = ((JValue)c["sha"]).ToString(),
                                Message = ((JValue)c["message"]).ToString(),
                                Author = commitAuthor["name"].ToString(),
                                Push_Id = pushEvent.Id
                            };
                            commits.Add(comitPushNode);
                        }

                        // add to Neo4J
                        var queryAddPush = _neoContext.Cypher
                            .Match("(r:Repo {Id: " + repo.Id + $" }})-[rel1:HAS_BRANCH]->(b:Branch {{Name: '{branchName}'}})")
                            .Merge($"(p:PushEvent {{Id:'{pushEvent.Id}'}})")
                            .OnCreate().Set($"p.Id = '{pushEvent.Id}'")
                            .Merge("(b)<-[rel2:PUSHED_TO]-(p)")
                            .Set($"rel2.Created = datetime('{pushEvent.Created}')")
                            ;
                        var queryPushText = queryAddPush.Query.DebugQueryText;
                        await queryAddPush.ExecuteWithoutResultsAsync();

                        var queryCommitsImport = _neoContext.Cypher
                           .Unwind(commits, "commit")
                           .Match($"(p:PushEvent {{Id:'{pushEvent.Id}'}})")
                           .Merge("(c:CommitPush {Sha: commit.Sha})")
                           .OnCreate().Set("c.Sha = commit.Sha, c.Author = commit.Author, c.Message = commit.Message, c.Push_Id = commit.Push_Id")
                           .Merge("(p)-[rel:CONTAINS_COMMIT]->(c)")
                           ;
                        var queryCommitsText = queryCommitsImport.Query.DebugQueryText;
                        await queryCommitsImport.ExecuteWithoutResultsAsync();

                        break;
                    case "PullRequestEvent":

                        var pullRequestInfo = (JObject)payload["pull_request"];
                        JObject headBranch = (JObject)pullRequestInfo["head"], baseBranch = (JObject)pullRequestInfo["base"];
                        PullRequestEvent pullRequestEvent = new PullRequestEvent
                        {
                            Id = ((JValue)a["id"]).ToString(),
                            Action = ((JValue)payload["action"]).ToString(),
                            CreatedAt = createdString,
                            Merged = (bool)pullRequestInfo["merged"],
                            PullRequestUrl = ((JValue)pullRequestInfo["url"]).ToString(),
                            BaseBranch = ((JValue)baseBranch["ref"]).ToString(),
                            HeadBranch = ((JValue)headBranch["ref"]).ToString()
                        };

                        if (pullRequestEvent.Action == "closed")
                        {
                            var queryPullRequest = _neoContext.Cypher
                                .Match($"(r {{Id: {repoId} }})-[:HAS_BRANCH]->(b1:Branch {{Name: '{pullRequestEvent.HeadBranch}'}})")
                                .Match($"(r)-[:HAS_BRANCH]->(b2:Branch {{Name: '{pullRequestEvent.BaseBranch}'}})")
                            .Merge($"(b1)-" +
                                    $"[rel:MERGED_INTO {{Id: '{pullRequestEvent.Id}', " +
                                    $"CreatedAt:datetime('{pullRequestEvent.CreatedAt}')}}]" +
                                    $"->(b2)")
                            ;
                            var queryPullRequestText = queryPullRequest.Query.DebugQueryText;
                            await queryPullRequest.ExecuteWithoutResultsAsync();
                        }
                        break;
                    default:
                        break;
                }
            }
            return Ok();
        }
    }
}
