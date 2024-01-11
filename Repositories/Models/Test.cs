using System;
using System.Collections.Generic;

namespace Repositories.Models
{
    public partial class Test
    {
        public Guid TestId { get; set; }
        public Guid? SchoolId { get; set; }
        public int TestCode { get; set; }
        public string Name { get; set; } = null!;
        public int TotalPaper { get; set; }
        public int NumOfDifferentPaper { get; set; }
        public int NumOfQuestion { get; set; }
        public string? Description { get; set; }
        public string TestConfig { get; set; } = null!;
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public Guid ModifiedBy { get; set; }

        public virtual School? School { get; set; }
    }
}
