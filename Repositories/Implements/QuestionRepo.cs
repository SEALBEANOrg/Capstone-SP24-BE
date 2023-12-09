using Repositories.Interfaces;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implements
{
    public class QuestionRepo : GenericRepo<Question>, IQuestionRepo
    {
        public QuestionRepo(ExagenContext context) : base(context)
        {
        }
    }
}
