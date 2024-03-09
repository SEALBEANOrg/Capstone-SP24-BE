using System;
using System.Collections.Generic;

namespace Repositories.Models
{
    public partial class Subject
    {
        public Subject()
        {
            Exams = new HashSet<Exam>();
            QuestionSets = new HashSet<QuestionSet>();
            SubjectSections = new HashSet<SubjectSection>();
        }

        public Guid SubjectId { get; set; }
        public int Grade { get; set; }
        public string Name { get; set; } = null!;
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public Guid ModifiedBy { get; set; }

        public virtual ICollection<Exam> Exams { get; set; }
        public virtual ICollection<QuestionSet> QuestionSets { get; set; }
        public virtual ICollection<SubjectSection> SubjectSections { get; set; }
    }
}
