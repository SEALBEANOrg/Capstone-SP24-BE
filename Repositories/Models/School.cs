using System;
using System.Collections.Generic;

namespace Repositories.Models
{
    public partial class School
    {
        public School()
        {
            StudentClasses = new HashSet<StudentClass>();
            Users = new HashSet<User>();
        }

        public Guid SchoolId { get; set; }
        public Guid ProvinceId { get; set; }
        public Guid AdminId { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public Guid ModifiedBy { get; set; }

        public virtual Province Province { get; set; } = null!;
        public virtual ICollection<StudentClass> StudentClasses { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
