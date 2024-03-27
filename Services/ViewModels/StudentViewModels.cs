using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels
{
    public class StudentViewModels
    {
        public Guid ClassId { get; set; }
        public Guid StudentId { get; set; }
        public int StudentNo { get; set; }
        public string FullName { get; set; } = null!;
        public int? Gender { get; set; }
        public DateTime? DoB { get; set; }
        public string? ParentPhoneNumber { get; set; }
    }

    public class StudentInfo 
    {
        public Guid StudentId { get; set; }
        public string FullName { get; set; } = null!;
        public int StudentNo { get; set; }
    }

    public class StudentCreate
    {
        public string FullName { get; set; } = null!;
        public int? Gender { get; set; }
        public DateTime? DoB { get; set; }
        public string? ParentPhoneNumber { get; set; }

    }

    public class StudentUpdate
    {
        public Guid StudentId { get; set; }
        public string FullName { get; set; } = null!;
        public int? Gender { get; set; }
        public DateTime? DoB { get; set; }
        public string? ParentPhoneNumber { get; set; }
    }

    public class StudentMoveOut
    {
        public Guid StudentId { get; set; }
        public Guid ClassId { get; set; }
    }
}
