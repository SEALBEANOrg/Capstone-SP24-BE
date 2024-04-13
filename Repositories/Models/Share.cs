using System;
using System.Collections.Generic;

namespace Repositories.Models
{
    public partial class Share
    {
        public Guid ShareId { get; set; }
        public Guid QuestionSetId { get; set; }
        public Guid? UserId { get; set; }
        public int Type { get; set; }
        public string? Note { get; set; }
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public Guid ModifiedBy { get; set; }
        public string StudyYear { get; set; } = null!;

        public virtual QuestionSet QuestionSet { get; set; } = null!;
        public virtual User? User { get; set; }
    }
}
