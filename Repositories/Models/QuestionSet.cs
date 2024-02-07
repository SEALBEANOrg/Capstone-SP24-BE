using System;
using System.Collections.Generic;

namespace Repositories.Models
{
    public partial class QuestionSet
    {
        public Guid QuestionSetId { get; set; }
        public Guid? SchoolId { get; set; }
        public string Name { get; set; } = null!;
        public int NumOfQuestion { get; set; }
        public string? Description { get; set; }
        public int? Grade { get; set; }
        public int? Subject { get; set; }
        public string SetConfig { get; set; } = null!;
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public Guid ModifiedBy { get; set; }

        public virtual School? School { get; set; }
    }
}
