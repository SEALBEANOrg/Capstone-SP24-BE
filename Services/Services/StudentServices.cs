using AutoMapper;
using Repositories;
using Services.Interfaces;
using Services.ViewModels;

namespace Services.Services
{
    public class StudentServices : IStudentServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StudentServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<StudentViewModels> GetStudentById(Guid studentId)
        {
            var student = await _unitOfWork.StudentRepo.FindByField(x => x.StudentId == studentId);
            if (student == null)
            {
                throw new Exception("Học sinh không tồn tại.");
            }

            var studentViewModels = _mapper.Map<StudentViewModels>(student);
            return studentViewModels;
        }

        public async Task<IEnumerable<StudentViewModels>> GetStudentsOfClass(Guid classId, Guid currentUser)
        {
            var studentClass = await _unitOfWork.StudentClassRepo.FindByField(x => x.ClassId == classId);
            var user = await _unitOfWork.UserRepo.FindByField(x => x.UserId == currentUser);

            if (studentClass == null)
            {
                throw new Exception("Lớp không tồn tại.");
            }

            if (studentClass.SchoolId != null && studentClass.SchoolId != user.SchoolId)
            {
                throw new Exception("Lớp này thuộc phận sự của trường, bạn không phải giáo viên trường này.");
            }
            var students = await _unitOfWork.StudentRepo.FindListByField(x => x.ClassId == classId);

            if (students == null)
            {
                return null;
            }

            var studentViewModels = _mapper.Map<IEnumerable<StudentViewModels>>(students);
            return studentViewModels;
        }

        public Task<bool> MoveOutStudent(Guid studentId, Guid currentUser)
        {
            throw new NotImplementedException();
        }

        public async Task<StudentViewModels> UpdateStudent(StudentUpdate studentUpdate, Guid currentUser)
        {
            var student = await _unitOfWork.StudentRepo.FindByField(x => x.StudentId == studentUpdate.StudentId);
            if (student == null)
            {
                throw new Exception("Học sinh không tồn tại.");
            }

            var studentClass = await _unitOfWork.StudentClassRepo.FindByField(x => x.ClassId == student.ClassId);

            if (studentClass.CreatedBy != currentUser)
            {
                throw new Exception("Bạn không phải giáo viên của lớp này.");
            }

            student = _mapper.Map(studentUpdate, student);

            try
            {
                _unitOfWork.StudentRepo.Update(student);
                var result = await _unitOfWork.SaveChangesAsync();
                if (result <= 0)
                {
                    throw new Exception("Cập nhật học sinh thất bại.");
                }
                var studentViewModels = _mapper.Map<StudentViewModels>(student);
                return studentViewModels;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở StudentServices - UpdateStudent: " + e.Message);
            }
        }
    }
}
