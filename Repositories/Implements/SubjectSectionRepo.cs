using Repositories.Interfaces;
using Repositories.Models;

namespace Repositories.Implements
{
    public class SubjectSectionRepo : GenericRepo<SubjectSection>, ISubjectSectionRepo
    {
        public SubjectSectionRepo(ExagenContext context) : base(context)
        {
        }
    }
}
