using System;
using System.Collections.Generic;

namespace Repositories.Models
{
    public partial class Student
    {
        public Guid StudentId { get; set; }
        public Guid ClassId { get; set; }
        public int StudentNo { get; set; }
        public string FullName { get; set; } = null!;
        public int? Grade { get; set; }

        public virtual StudentClass Class { get; set; } = null!;
    }
}
