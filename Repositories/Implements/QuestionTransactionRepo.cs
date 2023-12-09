using Repositories.Interfaces;
using Repositories.Models;

namespace Repositories.Implements
{
    public class QuestionTransactionRepo : GenericRepo<QuestionTransaction>, IQuestionTransactionRepo
    {
        public QuestionTransactionRepo(ExagenContext context) : base(context)
        {
        }
    }
}
