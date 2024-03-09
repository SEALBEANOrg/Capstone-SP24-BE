using Repositories.Implements;
using Repositories.Interfaces;
using Repositories.Models;

namespace Repositories
{
    public interface IUnitOfWork
    {
        public IDocumentRepo DocumentRepo { get; }
        public IExamRepo ExamRepo { get; }
        public IExamMarkRepo ExamMarkRepo { get; }
        public IPaperRepo PaperRepo { get; }
        public IPaperSetRepo PaperSetRepo { get; }
        public IQuestionInExamRepo QuestionInExamRepo { get; }
        public IQuestionInPaperRepo QuestionInPaperRepo { get; }
        public IQuestionRepo QuestionRepo { get; }
        public IQuestionSetRepo QuestionSetRepo { get; }
        public ISectionPaperSetConfigRepo SectionPaperSetConfigRepo { get; }
        public IShareRepo ShareRepo { get; }
        public IStudentRepo StudentRepo { get; }
        public IStudentClassRepo StudentClassRepo { get; }
        public ISubjectSectionRepo SubjectSectionRepo { get; }
        public ISubjectRepo SubjectRepo { get; }
        public ITransactionRepo TransactionRepo { get; }
        public IUserRepo UserRepo { get; }

        public Task<int> SaveChangesAsync();
    }
}
