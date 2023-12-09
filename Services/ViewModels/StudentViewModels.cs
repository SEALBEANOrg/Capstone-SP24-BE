using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels
{
    public class StudentViewModels
    {
        public Guid StudentId { get; set; }
        public Guid ClassId { get; set; }
        public string FullName { get; set; } = null!;

    }

    public class StudentCreate
    {
        public Guid ClassId { get; set; }
        public string FullName { get; set; } = null!;
        
    }

    public class StudentUpdate
    {
        public Guid StudentId { get; set; }
        public string FullName { get; set; } = null!;
    }

    public class StudentMoveOut
    {
        public Guid StudentId { get; set; }
        public Guid ClassId { get; set; }
    }
}
