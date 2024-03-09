using System;
using System.Collections.Generic;

namespace Repositories.Models
{
    public partial class SubjectSection
    {
        public SubjectSection()
        {
            Questions = new HashSet<Question>();
            SectionPaperSetConfigs = new HashSet<SectionPaperSetConfig>();
        }

        public Guid SectionId { get; set; }
        public Guid SubjectId { get; set; }
        public int SectionNo { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public Guid ModifiedBy { get; set; }

        public virtual Subject Subject { get; set; } = null!;
        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<SectionPaperSetConfig> SectionPaperSetConfigs { get; set; }
    }
}
