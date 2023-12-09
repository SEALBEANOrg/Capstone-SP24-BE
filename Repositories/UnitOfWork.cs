using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Repositories.Models;

namespace Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ExagenContext _context;

        private readonly IPaperRepo _paperRepo;
        private readonly IProvinceRepo _provinceRepo;
        private readonly IQuestionRepo _questionRepo;
        private readonly IQuestionTransactionRepo _questionTransactionRepo;
        private readonly ISchoolRepo _schoolRepo;
        private readonly IShareRepo _shareRepo;
        private readonly IStudentRepo _studentRepo;
        private readonly IStudentClassRepo _studentClassRepo;
        private readonly ISubjectSectionRepo _subjectSectionRepo;
        private readonly ITestRepo _testRepo;
        private readonly ITestResultRepo _testResultRepo;
        private readonly IUserRepo _userRepo;

        public UnitOfWork(ExagenContext context, IPaperRepo paperRepo, IProvinceRepo provinceRepo, IQuestionRepo questionRepo,
                            IQuestionTransactionRepo questionTransactionRepo, ISchoolRepo schoolRepo, IShareRepo shareRepo, 
                            IStudentRepo studentRepo, IStudentClassRepo studentClassRepo, ISubjectSectionRepo subjectSectionRepo, 
                            ITestRepo testRepo, ITestResultRepo testResultRepo, IUserRepo userRepo)
        {
            _context = context;
            _paperRepo = paperRepo;
            _provinceRepo = provinceRepo;
            _questionRepo = questionRepo;
            _questionTransactionRepo = questionTransactionRepo;
            _schoolRepo = schoolRepo;
            _shareRepo = shareRepo;
            _studentRepo = studentRepo;
            _studentClassRepo = studentClassRepo;
            _subjectSectionRepo = subjectSectionRepo;
            _testRepo = testRepo;
            _testResultRepo = testResultRepo;
            _userRepo = userRepo;
        }

        public IPaperRepo PaperRepo => _paperRepo;
        public IProvinceRepo ProvinceRepo => _provinceRepo;
        public IQuestionRepo QuestionRepo => _questionRepo;
        public IQuestionTransactionRepo QuestionTransactionRepo => _questionTransactionRepo;
        public ISchoolRepo SchoolRepo => _schoolRepo;
        public IShareRepo ShareRepo => _shareRepo;
        public IStudentRepo StudentRepo => _studentRepo;
        public IStudentClassRepo StudentClassRepo => _studentClassRepo;
        public ISubjectSectionRepo SubjectSectionRepo => _subjectSectionRepo;
        public ITestRepo TestRepo => _testRepo;
        public ITestResultRepo TestResultRepo => _testResultRepo;
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
