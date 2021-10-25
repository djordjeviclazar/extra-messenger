using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExtraMessenger.Data
{
    public class MongoDBSettings: IMongoDBSettings
    {
        public string IssuesCollectionName { get; set; }
        public string UserCollectionName { get; set; }

        public string ChatColletionName { get; set; }

        public string ReposCollectionName { get; set; }

        public string TicketsCollectionName { get; set; }

        public string OGitAuthCollectionName { get; set; }

        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }
    }

    public interface IMongoDBSettings
    {
        public string IssuesCollectionName { get; set; }
        string UserCollectionName { get; set; }

        string ChatColletionName { get; set; }

        public string ReposCollectionName { get; set; }

        public string TicketsCollectionName { get; set; }

        public string OGitAuthCollectionName { get; set; }

        string ConnectionString { get; set; }

        string DatabaseName { get; set; }
    }
}
