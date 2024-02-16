using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Repositories.Models;

namespace Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ExagenContext _context;

        private readonly IDocumentRepo _documentRepo;
        private readonly IPaperRepo _paperRepo;
        private readonly IPaperExamRepo _paperExamRepo;
        private readonly ISchoolRepo _schoolRepo;
        private readonly IShareRepo _shareRepo;
        private readonly IStudentRepo _studentRepo;
        private readonly IStudentClassRepo _studentClassRepo;
        private readonly ISubjectSectionRepo _subjectSectionRepo;
        private readonly IQuestionRepo _questionRepo;
        private readonly IQuestionSetRepo _questionSetRepo;
        private readonly IQuestionMappingRepo _questionMappingRepo;
        private readonly IExamRepo _examRepo;
        private readonly IExamMarkRepo _examMarkRepo;
        private readonly IUserRepo _userRepo;

        public UnitOfWork(ExagenContext context, IDocumentRepo documentRepo, IPaperRepo paperRepo, IQuestionRepo questionRepo,
                            IPaperExamRepo paperExamRepo, ISchoolRepo schoolRepo, IShareRepo shareRepo, IExamMarkRepo examMarkRepo,
                            IStudentRepo studentRepo, IStudentClassRepo studentClassRepo, ISubjectSectionRepo subjectSectionRepo, 
                            IQuestionSetRepo questionSetRepo, IQuestionMappingRepo questionMappingRepo, IExamRepo examRepo, IUserRepo userRepo)
        {
            _context = context;
            _documentRepo = documentRepo;
            _paperRepo = paperRepo;
            _questionRepo = questionRepo;
            _paperExamRepo = paperExamRepo;
            _schoolRepo = schoolRepo;
            _shareRepo = shareRepo;
            _studentRepo = studentRepo;
            _studentClassRepo = studentClassRepo;
            _subjectSectionRepo = subjectSectionRepo;
            _questionSetRepo = questionSetRepo;
            _questionMappingRepo = questionMappingRepo;
            _examRepo = examRepo;
            _examMarkRepo = examMarkRepo;
            _userRepo = userRepo;
        }

        public IDocumentRepo DocumentRepo => _documentRepo;
        public IPaperRepo PaperRepo => _paperRepo;
        public IQuestionRepo QuestionRepo => _questionRepo;
        public ISchoolRepo SchoolRepo => _schoolRepo;
        public IShareRepo ShareRepo => _shareRepo;
        public IStudentRepo StudentRepo => _studentRepo;
        public IStudentClassRepo StudentClassRepo => _studentClassRepo;
        public ISubjectSectionRepo SubjectSectionRepo => _subjectSectionRepo;
        public IQuestionSetRepo QuestionSetRepo => _questionSetRepo;
        public IQuestionMappingRepo QuestionMappingRepo => _questionMappingRepo;
        public IUserRepo UserRepo => _userRepo;
        public IPaperExamRepo PaperExamRepo => _paperExamRepo;
        public IExamRepo ExamRepo => _examRepo;
        public IExamMarkRepo ExamMarkRepo => _examMarkRepo;

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
