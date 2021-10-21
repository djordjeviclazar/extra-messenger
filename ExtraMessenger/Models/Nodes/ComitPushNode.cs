using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExtraMessenger.Models.Nodes
{
    public class ComitPushNode
    {
        public string Sha { get; set; }
        public string Message { get; set; }
        public string Author { get; set; }
        public string Push_Id { get; set; }
    }
}
