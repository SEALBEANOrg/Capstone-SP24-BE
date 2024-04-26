using AutoMapper;
using Repositories;
using Services.Interfaces.School;
using Services.Services.QuestionSet;
using Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.School
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
                var existed = (await _unitOfWork.SchoolRepo.FindByField(school => school.Name == schoolForCreateViewModel.Name && 
                                                                                school.Province == schoolForCreateViewModel.Province &&
                                                                                school.City == schoolForCreateViewModel.City &&
                                                                                school.Address == schoolForCreateViewModel.Address)) != null;
                if (existed)
                {
                      throw new Exception("Trường đã tồn tại ở địa chỉ này - Không thể thêm mới");
                }

                var school = new Repositories.Models.School
                {
                    SchoolId = Guid.NewGuid(),
                    Name = schoolForCreateViewModel.Name,
                    Address = schoolForCreateViewModel.Address,
                    City = schoolForCreateViewModel.City,
                    Province = schoolForCreateViewModel.Province,
                    Status = 1,
                    CreatedBy = currentUserId,
                    CreatedOn = DateTime.Now.AddHours(7),
                    ModifiedBy = currentUserId,
                    ModifiedOn = DateTime.Now.AddHours(7)
                };

                _unitOfWork.SchoolRepo.AddAsync(school);
                var result = await _unitOfWork.SaveChangesAsync();

                return result > 0;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở SchoolServices - AddNewSchool: " + e.Message);
            }
        }

        public async Task<bool> ChangeStatus(Guid schoolId, ChangeStatusViewModel changeStatusViewModel, Guid currentUserId)
        {
            try
            {
                var school = await _unitOfWork.SchoolRepo.FindByField(school => school.SchoolId == schoolId);
                if (school == null)
                {
                    throw new Exception("Không tìm thấy trường học");
                }

                school.Status = changeStatusViewModel.Status;
                school.ModifiedBy = currentUserId;
                school.ModifiedOn = DateTime.Now.AddHours(7);

                _unitOfWork.SchoolRepo.Update(school);
                var result = await _unitOfWork.SaveChangesAsync();

                return result > 0;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở SchoolServices - ChangeStatus: " + e.Message);
            }
        }

        public async Task<bool> DeleteSchool(Guid schoolId)
        {
            try
            {
                var school = await _unitOfWork.SchoolRepo.FindByField(school => school.SchoolId == schoolId);
                if (school == null)
                {
                    throw new Exception("Không tìm thấy trường học");
                }

                var user = await _unitOfWork.UserRepo.FindListByField(user => user.SchoolId == schoolId);
                if (user.Count > 0)
                {
                    throw new Exception("Trường học đang có người dùng - Không thể xóa");
                }

                _unitOfWork.SchoolRepo.Remove(school);
                var result = await _unitOfWork.SaveChangesAsync();

                return result > 0;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở SchoolServices - DeleteSchool: " + e.Message);
            }
        }

        public async Task<IEnumerable<SchoolList>> GetAllSchool(string? search, int status)
        {
            try 
            {                 
                var schools = await _unitOfWork.SchoolRepo.FindListByField(school => school.Status == status);
            
                if (schools == null)
                {
                    return null;
                }

                if (!string.IsNullOrEmpty(search))
                {
                    schools = schools.Where(school => school.Name.Contains(search) || school.Address.Contains(search) || school.City.Contains(search) || school.Province.Contains(search)).ToList();
                }

                var result = _mapper.Map<IEnumerable<SchoolList>>(schools);
                //Update AddressComposite = Address + City + Province
                foreach (var school in result)
                {
                    school.AddressComposite = $"{school.Address}, {school.City}, {school.Province}";
                }

                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở SchoolServices - GetAllSchool: " + e.Message);
            }
        }

        public async Task<IEnumerable<DropdownSchools>> GetDropdownSchools()
        {
            try
            {
                var schools = await _unitOfWork.SchoolRepo.FindListByField(school => school.Status == 1);

                if (schools == null)
                {
                    return null;
                }

                var result = schools.Select(school => new DropdownSchools
                {
                    SchoolId = school.SchoolId,
                    SchoolNameIdentity = $"{school.Name} - {school.Address}, {school.Province}, {school.City}"
                });

                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở SchoolServices - GetDropdownSchools: " + e.Message);
            }
        }

        public async Task<SchoolViewModels> GetSchoolById(Guid schoolId)
        {
            try
            {
                var school = await _unitOfWork.SchoolRepo.FindByField(school => school.SchoolId == schoolId);

                if (school == null)
                {
                    return null;
                }

                var result = _mapper.Map<SchoolViewModels>(school);

                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở SchoolServices - GetSchoolById: " + e.Message);
            }
        }

        public async Task<bool> UpdateSchool(SchoolForUpdateViewModel schoolForUpdateViewModel, Guid currentUserId)
        {
            try
            {
                var school = await _unitOfWork.SchoolRepo.FindByField(school => school.SchoolId == schoolForUpdateViewModel.SchoolId);
                if (school == null)
                {
                    throw new Exception("Không tìm thấy trường học");
                }

                school.Name = schoolForUpdateViewModel.Name;
                school.Address = schoolForUpdateViewModel.Address;
                school.City = schoolForUpdateViewModel.City;
                school.Province = schoolForUpdateViewModel.Province;
                school.ModifiedBy = currentUserId;
                school.ModifiedOn = DateTime.Now.AddHours(7);

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
