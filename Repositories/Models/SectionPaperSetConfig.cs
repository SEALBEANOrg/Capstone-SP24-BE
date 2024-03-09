using System;
using System.Collections.Generic;

namespace Repositories.Models
{
    public partial class SectionPaperSetConfig
    {
        public Guid SectionId { get; set; }
        public Guid PaperSetId { get; set; }
        public int NumberOfUse { get; set; }
        public int Difficulty { get; set; }
        public int NumInPaper { get; set; }

        public virtual PaperSet PaperSet { get; set; } = null!;
        public virtual SubjectSection Section { get; set; } = null!;
    }
}
