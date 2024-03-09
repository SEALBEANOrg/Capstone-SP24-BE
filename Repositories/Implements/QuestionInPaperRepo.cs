using Repositories.Interfaces;
using Repositories.Models;

namespace Repositories.Implements
{
    public class QuestionInPaperRepo : GenericRepo<QuestionInPaper>, IQuestionInPaperRepo
    {
        public QuestionInPaperRepo(ExagenContext context) : base(context)
        {
        }
    }
}
