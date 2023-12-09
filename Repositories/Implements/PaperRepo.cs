using Repositories.Interfaces;
using Repositories.Models;

namespace Repositories.Implements
{
    public class PaperRepo : GenericRepo<Paper>, IPaperRepo
    {
        public PaperRepo(ExagenContext context) : base(context)
        {
        }
    }
}
