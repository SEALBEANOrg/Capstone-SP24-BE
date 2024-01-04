using System;
using System.Collections.Generic;

namespace Repositories.Models
{
    public partial class Question
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
}
