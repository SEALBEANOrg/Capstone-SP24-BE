using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Repositories.Models;

namespace Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ExagenContext _context;

        private readonly IDocumentRepo _documentRepo;
        private readonly IExamRepo _examRepo;
        private readonly IExamMarkRepo _examMarkRepo;
        private readonly IPaperRepo _paperRepo;
        private readonly IPaperSetRepo _paperSetRepo;
        private readonly IQuestionInExamRepo _questionInExamRepo;
        private readonly IQuestionInPaperRepo _questionInPaper;
        private readonly IQuestionRepo _questionRepo;
        private readonly IQuestionSetRepo _questionSetRepo;
        private readonly ISectionPaperSetConfigRepo _sectionPaperSetConfigRepo;
        private readonly IShareRepo _shareRepo;
        private readonly IStudentRepo _studentRepo;
        private readonly IStudentClassRepo _studentClassRepo;
        private readonly ISubjectSectionRepo _subjectSectionRepo;
        private readonly ISubjectRepo _subjectRepo;
        private readonly ITransactionRepo _transactionRepo;
        private readonly IUserRepo _userRepo;

        public UnitOfWork(ExagenContext context, IDocumentRepo documentRepo, IExamRepo examRepo, IExamMarkRepo examMarkRepo, IPaperRepo paperRepo, IPaperSetRepo paperSetRepo, IQuestionInExamRepo questionInExamRepo, IQuestionInPaperRepo questionInPaper, IQuestionRepo questionRepo, IQuestionSetRepo questionSetRepo, ISectionPaperSetConfigRepo sectionPaperSetConfigRepo, IShareRepo shareRepo, IStudentRepo studentRepo, IStudentClassRepo studentClassRepo, ISubjectSectionRepo subjectSectionRepo, ISubjectRepo subjectRepo, ITransactionRepo transactionRepo, IUserRepo userRepo)
        {
            _context = context;
            _documentRepo = documentRepo;
            _examRepo = examRepo;
            _examMarkRepo = examMarkRepo;
            _paperRepo = paperRepo;
            _paperSetRepo = paperSetRepo;
            _questionInExamRepo = questionInExamRepo;
            _questionInPaper = questionInPaper;
            _questionRepo = questionRepo;
            _questionSetRepo = questionSetRepo;
            _sectionPaperSetConfigRepo = sectionPaperSetConfigRepo;
            _shareRepo = shareRepo;
            _studentRepo = studentRepo;
            _studentClassRepo = studentClassRepo;
            _subjectSectionRepo = subjectSectionRepo;
            _subjectRepo = subjectRepo;
            _transactionRepo = transactionRepo;
            _userRepo = userRepo;
        }

        public IDocumentRepo DocumentRepo => _documentRepo;
        public IExamRepo ExamRepo => _examRepo;
        public IExamMarkRepo ExamMarkRepo => _examMarkRepo;
        public IPaperRepo PaperRepo => _paperRepo;
        public IPaperSetRepo PaperSetRepo => _paperSetRepo;
        public IQuestionInExamRepo QuestionInExamRepo => _questionInExamRepo;
        public IQuestionInPaperRepo QuestionInPaperRepo => _questionInPaper;
        public IQuestionRepo QuestionRepo => _questionRepo;
        public IQuestionSetRepo QuestionSetRepo => _questionSetRepo;
        public ISectionPaperSetConfigRepo SectionPaperSetConfigRepo => _sectionPaperSetConfigRepo;
        public IShareRepo ShareRepo => _shareRepo;
        public IStudentRepo StudentRepo => _studentRepo;
        public IStudentClassRepo StudentClassRepo => _studentClassRepo;
        public ISubjectSectionRepo SubjectSectionRepo => _subjectSectionRepo;
        public ISubjectRepo SubjectRepo => _subjectRepo;
        public ITransactionRepo TransactionRepo => _transactionRepo;
        public IUserRepo UserRepo => _userRepo;

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();

            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return -1;
        }
    }
}
