using Repositories.Interfaces;
using Repositories.Models;

namespace Repositories.Implements
{
    public class SchoolRepo : GenericRepo<School>, ISchoolRepo
    {
        public SchoolRepo(ExagenContext context) : base(context)
        {
        }
    }
}
