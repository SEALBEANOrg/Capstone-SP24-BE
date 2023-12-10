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
        public string iss { get; set; }
        //public string azp { get; set; }
        //public string aud { get; set; }
        public string sub { get; set; }
        public string email { get; set; }
        public bool email_verified { get; set; }
        //public string at_hash { get; set; }
        public string name { get; set; }
        //public string picture { get; set; }
        //public string given_name { get; set; }
        //public string family_name { get; set; }
        //public string locale { get; set; }
        //public string iat { get; set; }
        //public string exp { get; set; }

    }

    public class RefreshLogin
    {
        public string nameid { get; set; }
        public string name { get; set; }
        public string typ { get; set; }
        public string role { get; set; }
        public long nbf { get; set; }
        public long exp { get; set; }
        public long iat { get; set; }
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
