using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExtraMessenger.Services.Github.Interfaces
{
    public interface IGithubClientService
    {
        public GitHubClient GetGitHubClient(ObjectId id);
        public Task<Repository> GetRepository(ObjectId id, string name, string owner);
        public Task<List<Repository>> GetMyRepositories(ObjectId id);
        public Task<List<Issue>> GetMyOpenIssues(ObjectId id);
        public Task<List<Branch>> GetBranches(ObjectId id, Repository repo);
        public Task<JArray> GetRepoEvents(string owner, string repoName, string token);
    }
}
