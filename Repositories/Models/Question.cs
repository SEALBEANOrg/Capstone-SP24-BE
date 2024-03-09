using System;
using System.Collections.Generic;

namespace Repositories.Models
{
    public partial class Question
    {
        public Question()
        {
            QuestionInExams = new HashSet<QuestionInExam>();
            QuestionInPapers = new HashSet<QuestionInPaper>();
        }

        public Guid QuestionId { get; set; }
        public Guid? SectionId { get; set; }
        public Guid? QuestionSetId { get; set; }
        public string QuestionPart { get; set; } = null!;
        public string Answer1 { get; set; } = null!;
        public string Answer2 { get; set; } = null!;
        public string Answer3 { get; set; } = null!;
        public string Answer4 { get; set; } = null!;
        public string CorrectAnswer { get; set; } = null!;
        public int Difficulty { get; set; }
        public Guid? SubjectId { get; set; }
        public int Grade { get; set; }

        public virtual QuestionSet? QuestionSet { get; set; }
        public virtual SubjectSection? Section { get; set; }
        public virtual ICollection<QuestionInExam> QuestionInExams { get; set; }

        public virtual ICollection<QuestionInPaper> QuestionInPapers { get; set; }
    }
}
