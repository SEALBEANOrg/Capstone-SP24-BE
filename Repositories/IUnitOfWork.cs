using Repositories.Interfaces;

namespace Repositories
{
    public interface IUnitOfWork
    {
        public IPaperRepo PaperRepo { get; }
        public IQuestionRepo QuestionRepo { get; }
        public IQuestionTransactionRepo QuestionTransactionRepo { get; }
        public ISchoolRepo SchoolRepo { get; }
        public IShareRepo ShareRepo { get; }
        public IStudentRepo StudentRepo { get; }
        public IStudentClassRepo StudentClassRepo { get; }
        public ISubjectSectionRepo SubjectSectionRepo { get; }
        public ITestRepo TestRepo { get; }
        public ITestResultRepo TestResultRepo { get; }
        public IUserRepo UserRepo { get; }

        public Task<int> SaveChangesAsync();
    }
}
