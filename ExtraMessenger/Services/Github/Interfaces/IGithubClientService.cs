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
        public void AddGithubClientForUser(ObjectId id);
        public GitHubClient GetGitHubClient(ObjectId id);
        public void RemoveGitHubClient(ObjectId id);
        public Task<Repository> GetRepository(ObjectId id, string name, string owner);
        public Task<List<Repository>> GetMyRepositories(ObjectId id);
        public Task<List<Issue>> GetMyOpenIssues(ObjectId id);
    }
}
