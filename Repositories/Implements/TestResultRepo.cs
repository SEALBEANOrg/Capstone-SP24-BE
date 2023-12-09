using Repositories.Interfaces;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implements
{
    public class TestResultRepo : GenericRepo<TestResult>, ITestResultRepo
    {
        public TestResultRepo(ExagenContext context) : base(context)
        {
        }
    }
}
