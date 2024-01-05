using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels
{
    public class QuestionViewModels
    {
        public Guid QuestionId { get; set; }
        public Guid? SectionId { get; set; }
        public Guid? SchoolId { get; set; }
        public string QuestionContent { get; set; } = null!;
        public int Difficulty { get; set; }
        public int? Grade { get; set; }
        public int? Subject { get; set; }
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public Guid ModifiedBy { get; set; }
    }

    public class QuestionCreate
    {
        public Guid? SectionId { get; set; }
        public Guid? SchoolId { get; set; }
        public string Content { get; set; } = null!;
        public int Difficulty { get; set; }
        public List<string> Answers { get; set; }
        public int? Grade { get; set; }
        public int? Subject { get; set; }
        public int IsUseToSell { get; set; }
    }

    public class QuestionUpdate
    {
        public Guid QuestionId { get; set; }
        public Guid? SectionId { get; set; }
        public Guid? SchoolId { get; set; }
        public string Content { get; set; } = null!;
        public int Difficulty { get; set; }
        public List<string> Answers { get; set; }
        public int? Grade { get; set; }
        public int? Subject { get; set; }
        public int IsUseToSell { get; set; }
    }

    public class QuestionJson
    {
        public string Content { get; set; } = null!;
        public List<string> Answers { get; set; }
    }
}
