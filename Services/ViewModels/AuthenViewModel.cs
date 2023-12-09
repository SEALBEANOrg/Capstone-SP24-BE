using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels
{
    public class AuthenViewModel
    {
    }

    public class UserLogin
    {
        public string Email { get; set; }
        public string FullName { get; set; } = null!;

    }

    public class LoginResponse
    {
        public UserInfo UserInfo { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
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
        public string Email { get; set; }
        public string FullName { get; set; } = null!;
    }
}
