using Repositories.Interfaces;

namespace Repositories
{
    public interface IUnitOfWork
    {
        public IDocumentRepo DocumentRepo { get; }
        public IPaperRepo PaperRepo { get; }
        public IPaperExamRepo PaperExamRepo { get; }
        public ISchoolRepo SchoolRepo { get; }
        public IShareRepo ShareRepo { get; }
        public IStudentRepo StudentRepo { get; }
        public IStudentClassRepo StudentClassRepo { get; }
        public ISubjectSectionRepo SubjectSectionRepo { get; }
        public IQuestionRepo QuestionRepo { get; }
        public IQuestionSetRepo QuestionSetRepo { get; }
        public IQuestionMappingRepo QuestionMappingRepo { get; }
        public IExamRepo ExamRepo { get; }
        public IExamMarkRepo ExamMarkRepo { get; }
        public IUserRepo UserRepo { get; }

        public Task<int> SaveChangesAsync();
    }
}
