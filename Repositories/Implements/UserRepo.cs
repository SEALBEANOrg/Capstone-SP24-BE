using Repositories.Interfaces;
using Repositories.Models;

namespace Repositories.Implements
{
    public class UserRepo : GenericRepo<User>, IUserRepo
    {
        public UserRepo(ExagenContext context) : base(context)
        {
        }
    }
}
