using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Repositories.Models;

namespace Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ExagenContext _context;

        private readonly IPaperRepo _paperRepo;
        private readonly IQuestionRepo _questionRepo;
        private readonly IPaperExamRepo _paperExamRepo;
        private readonly ISchoolRepo _schoolRepo;
        private readonly IShareRepo _shareRepo;
        private readonly IStudentRepo _studentRepo;
        private readonly IStudentClassRepo _studentClassRepo;
        private readonly ISubjectSectionRepo _subjectSectionRepo;
        private readonly IQuestionSetRepo _testRepo;
        private readonly IExamRepo _examRepo;
        private readonly IUserRepo _userRepo;

        public UnitOfWork(ExagenContext context, IPaperRepo paperRepo, IQuestionRepo questionRepo,
                            IPaperExamRepo paperExamRepo, ISchoolRepo schoolRepo, IShareRepo shareRepo, 
                            IStudentRepo studentRepo, IStudentClassRepo studentClassRepo, ISubjectSectionRepo subjectSectionRepo, 
                            IQuestionSetRepo testRepo, IExamRepo examRepo, IUserRepo userRepo)
        {
            _context = context;
            _paperRepo = paperRepo;
            _questionRepo = questionRepo;
            _paperExamRepo = paperExamRepo;
            _schoolRepo = schoolRepo;
            _shareRepo = shareRepo;
            _studentRepo = studentRepo;
            _studentClassRepo = studentClassRepo;
            _subjectSectionRepo = subjectSectionRepo;
            _testRepo = testRepo;
            _examRepo = examRepo;
            _userRepo = userRepo;
        }

        public IPaperRepo PaperRepo => _paperRepo;
        public IQuestionRepo QuestionRepo => _questionRepo;
        public ISchoolRepo SchoolRepo => _schoolRepo;
        public IShareRepo ShareRepo => _shareRepo;
        public IStudentRepo StudentRepo => _studentRepo;
        public IStudentClassRepo StudentClassRepo => _studentClassRepo;
        public ISubjectSectionRepo SubjectSectionRepo => _subjectSectionRepo;
        public IQuestionSetRepo TestRepo => _testRepo;
        public IUserRepo UserRepo => _userRepo;
        public IPaperExamRepo PaperExamRepo => _paperExamRepo;
        public IExamRepo ExamRepo => _examRepo;

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
