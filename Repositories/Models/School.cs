using System;
using System.Collections.Generic;

namespace Repositories.Models
{
    public partial class School
    {
        public Guid SchoolId { get; set; }
        public Guid AdminId { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public Guid ModifiedBy { get; set; }
    }
}
