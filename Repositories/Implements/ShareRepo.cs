using Repositories.Interfaces;
using Repositories.Models;

namespace Repositories.Implements
{
    public class ShareRepo : GenericRepo<Share>, IShareRepo
    {
        public ShareRepo(ExagenContext context) : base(context)
        {
        }
    }
}
