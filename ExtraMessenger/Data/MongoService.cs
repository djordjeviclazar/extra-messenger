﻿using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExtraMessenger.Data
{
    public class MongoService
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IMongoDBSettings _settings;

        public MongoService(IMongoDBSettings settings)
        {
            _settings = settings;

            _client = new MongoClient(settings.ConnectionString);
            _database = _client.GetDatabase(settings.DatabaseName);
        }

        public IMongoDatabase GetDb { get { return _database; } }
        public IMongoDBSettings GetDBSettings { get { return _settings; } }
    }
}
