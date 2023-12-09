using Repositories.Interfaces;
using Repositories.Models;

namespace Repositories.Implements
{
    public class ProvinceRepo : GenericRepo<Province>, IProvinceRepo
    {
        public ProvinceRepo(ExagenContext context) : base(context)
        {
        }
    }
}
