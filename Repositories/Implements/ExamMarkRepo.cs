using Repositories.Interfaces;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implements
{
    public class ExamMarkRepo : GenericRepo<ExamMark>, IExamMarkRepo
    {
        public ExamMarkRepo(ExagenContext context) : base(context)
        {
        }
    }
}
