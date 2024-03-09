using System;
using System.Collections.Generic;

namespace Repositories.Models
{
    public partial class Paper
    {
        public Paper()
        {
            QuestionInPapers = new HashSet<QuestionInPaper>();
        }

        public Guid PaperId { get; set; }
        public Guid PaperSetId { get; set; }
        public byte[] PaperContent { get; set; } = null!;
        public string PaperAnswer { get; set; } = null!;
        public int PaperCode { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }

        public virtual PaperSet PaperSet { get; set; } = null!;

        public virtual ICollection<QuestionInPaper> QuestionInPapers { get; set; }
    }
}
