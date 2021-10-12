using MongoDB.Bson;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExtraMessenger.Services.Github.Interfaces
{
    public interface IGithubClientService
    {
        public GitHubClient GetGitHubClient();
        public Task<Repository> GetRepository(ObjectId id, string name, string owner);
        public Task<List<Repository>> GetMyRepositories(ObjectId id);
        public Task<List<Issue>> GetMyOpenIssues(ObjectId id);
    }
}
