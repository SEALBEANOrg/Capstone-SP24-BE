using System;
using System.Collections.Generic;

namespace Repositories.Models
{
    public partial class PaperSet
    {
        public PaperSet()
        {
            Exams = new HashSet<Exam>();
            Papers = new HashSet<Paper>();
            SectionPaperSetConfigs = new HashSet<SectionPaperSetConfig>();
        }

        public Guid PaperSetId { get; set; }
        public Guid SubjectId { get; set; }
        public int Grade { get; set; }
        public string Name { get; set; } = null!;
        public bool ShuffleAnswer { get; set; }
        public bool ShuffleQuestion { get; set; }
        public bool SortByDifficulty { get; set; }
        public string KeyS3 { get; set; } = null!;

        public virtual ICollection<Exam> Exams { get; set; }
        public virtual ICollection<Paper> Papers { get; set; }
        public virtual ICollection<SectionPaperSetConfig> SectionPaperSetConfigs { get; set; }
    }
}
