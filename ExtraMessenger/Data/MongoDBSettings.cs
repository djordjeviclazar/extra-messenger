using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExtraMessenger.Data
{
    public class MongoDBSettings: IMongoDBSettings
    {
        public string UserCollectionName { get; set; }

        public string ChatColletionName { get; set; }

        public string ReposCollectionName { get; set; }

        public string TutorialsCollectionName { get; set; }

        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }
    }

    public interface IMongoDBSettings
    {
        string UserCollectionName { get; set; }

        string ChatColletionName { get; set; }

        public string ReposCollectionName { get; set; }

        public string TutorialsCollectionName { get; set; }

        string ConnectionString { get; set; }

        string DatabaseName { get; set; }
    }
}
