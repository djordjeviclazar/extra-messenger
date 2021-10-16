using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExtraMessenger.DTOs
{
    public class TutorialDTO
    {
        [Required]
        public string Title { get; set; }
        public List<TutorialPart> Parts { get; set; }
        public List<string> Topics { get; set; }
        public string Introduction { get; set; }
        public string Difficulty { get; set; }
    }
}
