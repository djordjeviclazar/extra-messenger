using MongoDB.Bson;
using Octokit;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace ExtraMessenger.Services.Github.Cache
{
    public class GithubClientCache
    {
        private readonly ConcurrentDictionary<string, GitHubClient> _cache = new ConcurrentDictionary<string, GitHubClient>();
        // ConcurrentDictionary<userId, GitHubClient>;

        private Dictionary<string, Timer> _timers = new Dictionary<string, Timer>();

        public GithubClientCache()
        {

        }

        public void Add(ObjectId id, GitHubClient gitHubClient)
        {
            var key = id.ToString();

            lock (_cache)
            {
                var newTimer = new Timer(3600000);
                newTimer.Enabled = true;
                newTimer.Start();
                newTimer.Elapsed += (sender, e) => OnTimeElapsedEvent(sender, e, key);

                if (_cache.ContainsKey(key))
                {
                    // update timer:
                    if (_timers.TryGetValue(key, out _))
                    {
                        _timers.Remove(key);
                        _timers.Add(key, newTimer);
                    }
                    else
                    {
                        _timers.Add(key, newTimer);
                    }
                }
                else
                {
                    _cache.TryAdd(key, gitHubClient);
                    _timers.Add(key, newTimer);
                }
            }
        }

        public GitHubClient GetClient(ObjectId id)
        {
            var key = id.ToString();
            if (_cache.TryGetValue(key, out var client))
            {
                lock (_cache) 
                {
                    var newTimer = new Timer(3600000);
                    newTimer.Enabled = true;
                    newTimer.Start();
                    newTimer.Elapsed += (sender, e) => OnTimeElapsedEvent(sender, e, key);
                    _timers.Remove(key);
                    _timers.Add(key, newTimer);
                }
            }
            return client;
        }

        public void Remove(ObjectId id)
        {
            var key = id.ToString();
            _cache.Remove(key, out _);
            _timers.Remove(key, out var timer);
            timer.Stop();
            timer.Dispose();
        }

        private void OnTimeElapsedEvent(object source, ElapsedEventArgs e, string key)
        {
            Remove(new ObjectId(key));
        }
    }
}
