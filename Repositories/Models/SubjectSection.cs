using System;
using System.Collections.Generic;

namespace Repositories.Models
{
    public partial class SubjectSection
    {
        public SubjectSection()
        {
            Questions = new HashSet<Question>();
        }

        public Guid SectionId { get; set; }
        public string? Description { get; set; }
        public int Grade { get; set; }
        public string Name { get; set; } = null!;
        public int Subject { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public Guid ModifiedBy { get; set; }

        public virtual ICollection<Question> Questions { get; set; }
    }
}
