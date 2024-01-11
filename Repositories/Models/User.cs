using System;
using System.Collections.Generic;

namespace Repositories.Models
{
    public partial class User
    {
        public Guid UserId { get; set; }
        public Guid? SchoolId { get; set; }
        public int UserType { get; set; }
        public string FullName { get; set; } = null!;
        public string? Phone { get; set; }
        public string Email { get; set; } = null!;
        public int Point { get; set; }
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public Guid ModifiedBy { get; set; }

        public virtual School? School { get; set; }
    }
}
