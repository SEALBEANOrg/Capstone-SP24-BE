using Repositories.Interfaces;
using Repositories.Models;

namespace Repositories.Implements
{
    public class StudentRepo : GenericRepo<Student>, IStudentRepo
    {
        public StudentRepo(ExagenContext context) : base(context)
        {
        }
    }
}
