using Repositories.Interfaces;
using Repositories.Models;

namespace Repositories.Implements
{
    public class StudentClassRepo : GenericRepo<StudentClass>, IStudentClassRepo
    {
        public StudentClassRepo(ExagenContext context) : base(context)
        {
        }
    }
}
