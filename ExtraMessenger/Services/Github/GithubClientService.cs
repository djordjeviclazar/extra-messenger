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
        private readonly IConfiguration _configuration;

        public GithubClientService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void AddGithubClientForUser(ObjectId id)
        {
            GitHubClient gitHubClient = new GitHubClient(new ProductHeaderValue(_configuration.GetSection("GithubOAuth:AppHeader").Value));
            _cache.Add(id, gitHubClient);
        }

        public GitHubClient GetGitHubClient(ObjectId id)
        {
            var client = _cache.GetClient(id);
            if (client == null)
            {
                // create client
                
                client = _cache.GetClient(id);
            }

            return client;
        }

        public async Task<List<Issue>> GetMyOpenIssues(ObjectId id)
        {
            var client = GetGitHubClient(id);
            if (client != null)
            {
                List<Issue> issues = new List<Issue>();
                var request = new SearchIssuesRequest();
                request.State = ItemState.Open;
                var result = await client.Search.SearchIssues(request);
                
                issues = result.Items.ToList();
                return issues;
            }

            return null;
        }

        public async Task<List<Repository>> GetMyRepositories(ObjectId id)
        {
            var client = GetGitHubClient(id);
            if (client != null)
            {
                var repos = await client.Repository.GetAllForCurrent();
                return repos.ToList();
            }

            return null;
        }

        public async Task<Repository> GetRepository(ObjectId id, string name, string owner)
        {
            var client = GetGitHubClient(id);
            if (client != null)
            {
                return await client.Repository.Get(owner, name);
                /*
                var repo = await client.Repository.Get(owner, name);
                return repo;*/
            }

            return null;
        }

        public void RemoveGitHubClient(ObjectId id)
        {
            _cache.Remove(id);
        }
    }
}
