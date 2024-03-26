using System;
using System.Collections.Generic;

namespace Repositories.Models
{
    public partial class School
    {
        public School()
        {
            Users = new HashSet<User>();
        }

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

        public virtual ICollection<User> Users { get; set; }
    }
}
