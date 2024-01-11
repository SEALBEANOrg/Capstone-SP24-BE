using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels
{
    public class SchoolViewModels
    {
        public Guid SchoolId { get; set; }
        public Guid AdminId { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string? Province { get; set; }
        public int Status { get; set; }
    }

    public class ComboSchool
    {
        public Guid SchoolId { get; set; }
        public string Name { get; set; } = null!;
    }

    public class SchoolForCreateViewModel
    {
        public string AdminEmail { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string? Province { get; set; }
    }

    public class SchoolForUpdateViewModel
    {
        public Guid SchoolId { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string? Province { get; set; }
        public int Status { get; set; }
    }
}
