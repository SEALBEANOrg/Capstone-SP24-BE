using System;
using System.Collections.Generic;

namespace Repositories.Models
{
    public partial class StudentClass
    {
        public StudentClass()
        {
            Exams = new HashSet<Exam>();
            Students = new HashSet<Student>();
        }

        public Guid ClassId { get; set; }
        public string Name { get; set; } = null!;
        public int? TotalStudent { get; set; }
        public int Grade { get; set; }
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public Guid ModifiedBy { get; set; }
        public string? StudyYear { get; set; }

        public virtual ICollection<Exam> Exams { get; set; }
        public virtual ICollection<Student> Students { get; set; }
    }
}
