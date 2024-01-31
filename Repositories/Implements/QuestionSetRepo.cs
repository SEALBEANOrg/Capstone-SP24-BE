using Repositories.Interfaces;
using Repositories.Models;

namespace Repositories.Implements
{
    public class QuestionSetRepo : GenericRepo<QuestionSet>, IQuestionSetRepo
    {
        public QuestionSetRepo(ExagenContext context) : base(context)
        {
        }
    }
}
