using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels
{
    public class SchoolList
    {
        public Guid SchoolId { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string? City { get; set; }
        public string? Province { get; set; }
        public int Status { get; set; }
    }

    public class SchoolViewModels
    {
        public Guid SchoolId { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string? City { get; set; }
        public string? Province { get; set; }
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public Guid ModifiedBy { get; set; }
    }
    public class SchoolForCreateViewModel
    {
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string? City { get; set; }
        public string? Province { get; set; }
    }

    public class SchoolForUpdateViewModel
    {
        public Guid SchoolId { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string? City { get; set; }
        public string? Province { get; set; }
    }

    public class ChangeStatusViewModel
    {
        public int Status { get; set; }
    }

    public class  DropdownSchools
    {
        public Guid SchoolId { get; set; }
        public string SchoolNameIdentity { get; set; } = null!;
    }
}
