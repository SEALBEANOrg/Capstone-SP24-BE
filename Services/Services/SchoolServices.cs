using AutoMapper;
using Repositories;
using Repositories.Models;
using Services.Interfaces;
using Services.ViewModels;
using System.Security.Cryptography.X509Certificates;

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

        public async Task<bool> AddNewSchool(SchoolForCreateViewModel schoolForCreateViewModel, Guid currentUserId)
        {
            try
            {
                var adminSchool = await _unitOfWork.UserRepo.FindByField(user => user.Email == schoolForCreateViewModel.AdminEmail);
                if (adminSchool == null)
                {
                    throw new Exception("Không tìm thấy người dùng này.");
                }

                if (adminSchool.Status != 1)
                {
                      throw new Exception("Người dùng này đã ngưng hoạt động hoặc đã thuộc trường khác.");
                }

                if (adminSchool.UserType != 1)
                {
                    throw new Exception("Phải là giáo viên để được nâng cấp thành Admin trường.");
                }

                if (schoolForCreateViewModel.Province != null && schoolForCreateViewModel.Address != null)
                {
                    var schoolExisted = await _unitOfWork.SchoolRepo.FindByField(school => school.Address == schoolForCreateViewModel.Address && school.Province == schoolForCreateViewModel.Province);
                    if (schoolExisted != null)
                    {
                        throw new Exception("Địa chỉ này đã có trường ở tỉnh.");
                    }
                }

                if (schoolForCreateViewModel.Name != null && schoolForCreateViewModel.Province != null)
                {
                    var schoolExisted = await _unitOfWork.SchoolRepo.FindByField(school => school.Name == schoolForCreateViewModel.Name && school.Province == schoolForCreateViewModel.Province);
                    if (schoolExisted != null)
                    {
                        throw new Exception("Tên trường này đã tồn tại ở tỉnh.");
                    }
                }

                var school = _mapper.Map<School>(schoolForCreateViewModel);
                school.AdminId = adminSchool.UserId;
                school.CreatedOn = DateTime.Now;
                school.ModifiedOn = DateTime.Now;
                school.CreatedBy = currentUserId;
                school.ModifiedBy = currentUserId;
                school.Status = 1;

                _unitOfWork.SchoolRepo.AddAsync(school);
                var result = await _unitOfWork.SaveChangesAsync();
                if (result <= 0)
                {
                    return false;
                }

                adminSchool.UserType = 2; // schoolAdmin
                adminSchool.Status = 3; // trong trường thì status = 3
                adminSchool.SchoolId = school.SchoolId;
                adminSchool.ModifiedOn = DateTime.Now;
                adminSchool.ModifiedBy = currentUserId;
                _unitOfWork.UserRepo.Update(adminSchool);
                var result2 = await _unitOfWork.SaveChangesAsync();
                if (result2 <= 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở SchoolServices - AddNewSchool: " + e.Message);
            }
        }

        public async Task<bool> ChangeSchoolAdmin(Guid schoolId, string email, Guid currentUserId)
        {
            try
            {
                var school = await _unitOfWork.SchoolRepo.FindByField(school => school.SchoolId == schoolId);
                var oldAdmin = await _unitOfWork.UserRepo.FindByField(user => user.UserId == school.AdminId);
                var newAdmin = await _unitOfWork.UserRepo.FindByField(user => user.Email == email);
                if (email == oldAdmin.Email)
                {
                    throw new Exception("Người dùng này đã là Admin trường.");
                }

                if (school == null)
                {
                    throw new Exception("Trường học không tồn tại");
                }

                var adminSchool = await _unitOfWork.UserRepo.FindByField(user => user.Email == email);
                if (adminSchool == null)
                {
                    throw new Exception("Không tìm thấy người dùng này.");
                }

                if (adminSchool.Status != 1)
                {
                    throw new Exception("Người dùng này đã ngưng hoạt động hoặc đã thuộc trường khác.");
                }

                if (adminSchool.UserType != 1)
                {
                    throw new Exception("Phải là giáo viên để được nâng cấp thành Admin trường.");
                }

                school.AdminId = newAdmin.UserId;
                school.ModifiedOn = DateTime.Now;
                school.ModifiedBy = currentUserId;

                oldAdmin.UserType = 1;
                oldAdmin.ModifiedOn = DateTime.Now;
                oldAdmin.ModifiedBy = currentUserId;

                newAdmin.UserType = 2;
                newAdmin.Status = 3;
                newAdmin.SchoolId = schoolId;
                newAdmin.ModifiedOn = DateTime.Now;
                newAdmin.ModifiedBy = currentUserId;

                _unitOfWork.SchoolRepo.Update(school);
                _unitOfWork.UserRepo.Update(oldAdmin);
                _unitOfWork.UserRepo.Update(newAdmin);
                var result = await _unitOfWork.SaveChangesAsync();

                return result > 0;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở SchoolServices - ChangeSchoolAdmin: " + e.Message);
            }
        }

        public async Task<bool> DeleteSchool(Guid schoolId)
        {
            try
            {

                var school = await _unitOfWork.SchoolRepo.FindByField(school => school.SchoolId == schoolId);
                if (school == null)
                {
                    throw new Exception("Trường học không tồn tại");
                }

                var users = await _unitOfWork.UserRepo.FindListByField(user => user.SchoolId == schoolId);

                if (users == null)
                {
                    _unitOfWork.SchoolRepo.Remove(school);
                }
                else
                {
                    school.Status = 0;
                    _unitOfWork.SchoolRepo.Update(school);
                }

                var result = await _unitOfWork.SaveChangesAsync();
                
                return result > 0;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở SchoolServices - DeleteSchool: " + e.Message);
            }
        }

        public async Task<IEnumerable<StudentClassViewModels>> GetAllClassOfMySchool(Guid currentUserId)
        {
            var schoolId = (await _unitOfWork.UserRepo.FindByField(user => user.UserId == currentUserId)).SchoolId;
            var classes = await _unitOfWork.StudentClassRepo.FindListByField(studentClass => studentClass.SchoolId == schoolId);
            var studentClassViewModels = _mapper.Map<IEnumerable<StudentClassViewModels>>(classes);
            
            return studentClassViewModels;
        }

        public async Task<IEnumerable<SchoolViewModels>> GetAllSchool()
        {
            var schools = await _unitOfWork.SchoolRepo.GetAllAsync();
            
            if (schools == null)
            {
                return null;
            }

            var schoolViewModels = _mapper.Map<IEnumerable<SchoolViewModels>>(schools);
            return schoolViewModels;
        }

        public async Task<IEnumerable<ComboSchool>> GetComboSchool()
        {
            var schools = await _unitOfWork.SchoolRepo.GetAllAsync();
            if (schools == null)
            {
                return null;
            }
            var comboSchool = new List<ComboSchool>();
            foreach (var item in schools)
            {
                var school = new ComboSchool { Name = item.Name + " - " + item.Province, SchoolId = item.SchoolId };
                comboSchool.Add(school);
            }
            return comboSchool;
        }

        public async Task<SchoolViewModels> GetInfoMySchool(Guid currentUserId)
        {
            var currentUser = await _unitOfWork.UserRepo.FindByField(user => user.UserId == currentUserId);

            if (currentUser.SchoolId == null)
            {
                return null;
            }

            var school = _unitOfWork.SchoolRepo.FindByField(school => school.SchoolId == currentUser.SchoolId);
            
            var schoolViewModels = _mapper.Map<SchoolViewModels>(school);
            return schoolViewModels;

        }

        public async Task<SchoolViewModels> GetSchoolById(Guid schoolId)
        {
            var school = await _unitOfWork.SchoolRepo.FindByField(school => school.SchoolId == schoolId);
            if (school == null)
            {
                throw new Exception("Trường học không tồn tại");
            }

            var schoolViewModels = _mapper.Map<SchoolViewModels>(school);
            return schoolViewModels;
        }

        public async Task<IEnumerable<UserViewModels>> GetTeacherOfMySchool(Guid currentUserId)
        {
            var currentUser = await _unitOfWork.UserRepo.FindByField(user => user.UserId == currentUserId);

            var teachers = await _unitOfWork.UserRepo.FindListByField(user => user.SchoolId == currentUser.SchoolId, user => user.School);

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

        public async Task<bool> UpdateSchool(SchoolForUpdateViewModel schoolForUpdateViewModel)
        {
            try
            {
                var school = await _unitOfWork.SchoolRepo.FindByField(school => school.SchoolId == schoolForUpdateViewModel.SchoolId);
                
                if (school == null)
                {
                    throw new Exception("Trường không tồn tại");
                }

                if (schoolForUpdateViewModel.Province != null && schoolForUpdateViewModel.Address != null 
                    && schoolForUpdateViewModel.Province != school.Province && schoolForUpdateViewModel.Address != school.Address)
                {
                    var schoolExisted = await _unitOfWork.SchoolRepo.FindByField(school => school.Address == schoolForUpdateViewModel.Address && school.Province == schoolForUpdateViewModel.Province);
                    if (schoolExisted != null)
                    {
                        throw new Exception("Địa chỉ này đã có trường ở tỉnh.");
                    }
                }

                if (schoolForUpdateViewModel.Name != null && schoolForUpdateViewModel.Province != null
                    && schoolForUpdateViewModel.Name != school.Name && schoolForUpdateViewModel.Province != school.Province)
                {
                    var schoolExisted = await _unitOfWork.SchoolRepo.FindByField(school => school.Name == schoolForUpdateViewModel.Name && school.Province == schoolForUpdateViewModel.Province);
                    if (schoolExisted != null)
                    {
                        throw new Exception("Tên trường này đã tồn tại ở tỉnh.");
                    }
                }

                school.ModifiedOn = DateTime.Now;
                school.ModifiedBy = schoolForUpdateViewModel.SchoolId; 
                school = _mapper.Map(schoolForUpdateViewModel, school);

                _unitOfWork.SchoolRepo.Update(school);
                var result = await _unitOfWork.SaveChangesAsync();
                
                return result > 0;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở SchoolServices - UpdateSchool: " + e.Message);
            }
        }
    }
}
