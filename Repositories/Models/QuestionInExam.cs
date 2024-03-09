using System;
using System.Collections.Generic;

namespace Repositories.Models
{
    public partial class QuestionInExam
    {
        public Guid ExamId { get; set; }
        public Guid QuestionId { get; set; }
        public int CorrectCount { get; set; }
        public int UseCount { get; set; }

        public virtual Exam Exam { get; set; } = null!;
        public virtual Question Question { get; set; } = null!;
    }
}
