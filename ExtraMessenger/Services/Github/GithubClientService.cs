using ExtraMessenger.Data;
using ExtraMessenger.Services.Github.Cache;
using ExtraMessenger.Services.Github.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExtraMessenger.Services.Github
{
    public class GithubClientService : IGithubClientService
    {
        private readonly GithubClientCache _cache = new GithubClientCache();
        //private readonly GitHubClient _githubClient;
        private readonly IConfiguration _configuration;

        public GithubClientService(IConfiguration configuration)
        {
            _configuration = configuration;
            //_githubClient = new GitHubClient(new ProductHeaderValue(_configuration.GetSection("GithubOAuth:AppHeader").Value));
        }

        public GitHubClient GetGitHubClient(ObjectId id)
        {
            var result = _cache.GetClient(id);
            if (result == null)
            {
                result = new GitHubClient(new ProductHeaderValue(_configuration.GetSection("GithubOAuth:AppHeader").Value));
                _cache.Add(id, result);
            }
            return result;
        }

        public async Task<List<Issue>> GetMyOpenIssues(ObjectId id)
        {
            var githubClient = GetGitHubClient(id);

            List<Issue> issues = new List<Issue>();
            var request = new SearchIssuesRequest();
            request.State = ItemState.Open;
            var result = await githubClient.Search.SearchIssues(request);

            issues = result.Items.ToList();
            return issues;
        }

        public async Task<List<Repository>> GetMyRepositories(ObjectId id)
        {
            var githubClient = GetGitHubClient(id);

            var repos = await githubClient.Repository.GetAllForCurrent();
            return repos.ToList();
        }

        public async Task<Repository> GetRepository(ObjectId id, string name, string owner)
        {
            var githubClient = GetGitHubClient(id);

            return await githubClient.Repository.Get(owner, name);
        }
    }
}
