using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerPlatform.Core.Entities.Questions
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;

        public int VulnerabilityId { get; set; }
        public Vulnerability? Vulnerability { get; set; }

        public List<AnswerOption> Options { get; set; } = new();
    }


}
