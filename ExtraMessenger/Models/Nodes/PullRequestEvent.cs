using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExtraMessenger.Models.Nodes
{
    public class PullRequestEvent
    {
        public string Id { get; set; }
        public string Action { get; set; }
        public bool Merged { get; set; }
        public string CreatedAt { get; set; }
        public string PullRequestUrl { get; set; }
        public string BaseBranch { get; set; }
        public string HeadBranch { get; set; }
    }
}
