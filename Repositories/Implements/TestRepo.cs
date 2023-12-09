using Repositories.Interfaces;
using Repositories.Models;

namespace Repositories.Implements
{
    public class TestRepo : GenericRepo<Test>, ITestRepo
    {
        public TestRepo(ExagenContext context) : base(context)
        {
        }
    }
}
