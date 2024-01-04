using AutoMapper;
using Repositories;
using Services.Interfaces;
using Services.ViewModels;

namespace Services.Services
{
    public class SchoolServices : ISchoolServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SchoolServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StudentClassViewModels>> GetAllClassOfMySchool(Guid currentUserId)
        {
            var schoolId = (await _unitOfWork.UserRepo.FindByField(user => user.UserId == currentUserId)).SchoolId;
            var classes = await _unitOfWork.StudentClassRepo.FindListByField(studentClass => studentClass.SchoolId == schoolId);
            var studentClassViewModels = _mapper.Map<IEnumerable<StudentClassViewModels>>(classes);
            
            return studentClassViewModels;
        }

        public async Task<IEnumerable<ComboSchool>> GetComboSchool()
        {
            var schools = await _unitOfWork.SchoolRepo.GetAllAsync();
            var comboSchool = _mapper.Map<IEnumerable<ComboSchool>>(schools);
            return comboSchool;
        }

        public async Task<IEnumerable<UserViewModels>> GetTeacherOfMySchool(Guid currentUserId)
        {
            var user = await _unitOfWork.UserRepo.FindByField(user => user.UserId == currentUserId);

            var teachers = await _unitOfWork.UserRepo.FindListByField(user => user.SchoolId == user.SchoolId && x.UserType == 1);

            if (teachers.Count() <= 0)
            {
                return null;
            }   

            var userViewModels = _mapper.Map<IEnumerable<UserViewModels>>(teachers);
            return userViewModels;
        }

        public async Task<bool> RemoveTeacherFromSchool(Guid teacherId, Guid currentUser)
        {
            var teacher = await _unitOfWork.UserRepo.FindByField(user => user.UserId == teacherId);
            if (teacher == null)
            {
                return false;
            }

            var curUser = await _unitOfWork.UserRepo.FindByField(user => user.UserId == currentUser);
            if (teacher.SchoolId != curUser.SchoolId || teacher.Status != 3)
            {
                throw new Exception("Giáo viên này không thuộc phận sự của trường bạn.");
            }

            try
            {
                var school = await _unitOfWork.SchoolRepo.FindByField(school => school.SchoolId == teacher.SchoolId);
                
                teacher.SchoolId = null;
                teacher.ModifiedOn = DateTime.Now;
                teacher.ModifiedBy = currentUser;
                teacher.Status = 1;

                school.ModifiedOn = DateTime.Now;
                school.ModifiedBy = currentUser;

                _unitOfWork.UserRepo.Update(teacher);
                _unitOfWork.SchoolRepo.Update(school);
                var result = await _unitOfWork.SaveChangesAsync();
                if (result <= 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở SchoolServices - RemoveTeacherFromSchool: " + e.Message);
            }
        }
    }
}
