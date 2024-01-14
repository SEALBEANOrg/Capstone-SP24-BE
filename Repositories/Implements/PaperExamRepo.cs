using Repositories.Interfaces;
using Repositories.Models;

namespace Repositories.Implements
{
    public class PaperExamRepo : GenericRepo<PaperExam>, IPaperExamRepo
    {
        public PaperExamRepo(ExagenContext context) : base(context)
        {
        }
    }
}
