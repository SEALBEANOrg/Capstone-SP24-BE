using Repositories.Interfaces;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implements
{
    public class SchoolRepo : GenericRepo<School>, ISchoolRepo
    {
        public SchoolRepo(ExagenContext context) : base(context)
        {
        }
    }
}
