using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels
{
    public class UserViewModels
    {
        public Guid UserId { get; set; }
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
    }

    public class Request
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;

    }

    public class ProfileUpdate
    {
        public string FullName { get; set; } = null!;
        public string? Phone { get; set; }
    }

    public class RoleUpdate
    {
        public int UserType { get; set; }   
    }

    public class ActiveUser
    {
        public bool isActive { get; set; }
    }

    public class UserInfo
    {
        public Guid UserId { get; set; }
        public int UserType { get; set; }
        public string FullName { get; set; } = null!;
        public int Status { get; set; }
    }

    public class UserSignUp
    {
        public Guid? Guid { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; } = null!;
    }
}
