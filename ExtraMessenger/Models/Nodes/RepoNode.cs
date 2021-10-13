using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExtraMessenger.Models.Nodes
{
    public class RepoNode
    {
        [Required]
        public long Id { get; set; }
        public string Language { get; set; }
        public string Name { get; set; }
    }
}
