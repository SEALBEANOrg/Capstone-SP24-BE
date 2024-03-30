using System;
using System.Collections.Generic;

namespace Repositories.Models
{
    public partial class ExamMark
    {
        public Guid ExamMarkId { get; set; }
        public Guid ExamId { get; set; }
        public Guid StudentId { get; set; }
        public int? PaperCode { get; set; }
        public string? AnswersSelected { get; set; }
        public decimal? Mark { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int Status { get; set; }
        public int? StudentCode { get; set; }

        public virtual Exam Exam { get; set; } = null!;
        public virtual Student Student { get; set; } = null!;
    }
}
