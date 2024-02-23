using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using OfficeOpenXml;
using Repositories;
using Repositories.Models;
using Services.Interfaces;
using Services.ViewModels;
using System.Net.WebSockets;

namespace Services.Services
{
    public class StudentClassServices : IStudentClassServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IClaimsService _claimsService;

        public StudentClassServices(IUnitOfWork unitOfWork, IMapper mapper, IClaimsService claimsService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _claimsService = claimsService;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async Task<bool> CreateStudentClass(StudentClassCreate studentClassCreate)
        {
            var currentUserId = await GetCurrentUser();
            var user = await _unitOfWork.UserRepo.FindByField(user => user.UserId == currentUserId);

            if (studentClassCreate.Grade != null && (studentClassCreate.Grade < 0 || studentClassCreate.Grade > 12))
            {
                throw new Exception("Khối không hợp lệ");
            }

            var studentClass = _mapper.Map<StudentClass>(studentClassCreate);
            studentClass.Status = 1;
            studentClass.SchoolId = user.SchoolId;
            studentClass.CreatedBy = currentUserId;
            studentClass.ModifiedBy = currentUserId;
            studentClass.CreatedOn = DateTime.Now;
            studentClass.ModifiedOn = DateTime.Now;

            try
            {
                studentClass = await _unitOfWork.StudentClassRepo.AddAsync(studentClass);

                if (studentClass.SchoolId != null)
                {
                    var school = await _unitOfWork.SchoolRepo.FindByField(school => school.SchoolId == studentClass.SchoolId);

                    if (school == null)
                    {
                        throw new Exception("Trường học không tồn tại");
                    }


                    school.ModifiedOn = DateTime.Now;
                    school.ModifiedBy = currentUserId;

                    _unitOfWork.SchoolRepo.Update(school);
                }

                var result = await _unitOfWork.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở StudentClassServices - CreateStudentClass: " + e.Message);
            }
        }

        public async Task<bool> DeleteStudentClass(Guid id)
        {
            var studentClass = await _unitOfWork.StudentClassRepo.FindByField(studentClass => studentClass.ClassId == id);
            if (studentClass == null)
            {
                throw new Exception("Lớp học không tồn tại");
            }

            if (studentClass.Status == 0)
            {
                throw new Exception("Lớp đã ngừng hoạt động");
            }

            var currentUser = await GetCurrentUser();

            if (studentClass.CreatedBy != currentUser)
            {
                throw new Exception("Bạn không có quyền xóa học sinh khỏi lớp học này");
            }

            try
            {
                _unitOfWork.StudentClassRepo.Remove(studentClass);

                if (studentClass.SchoolId != null)
                {
                    var school = await _unitOfWork.SchoolRepo.FindByField(school => school.SchoolId == studentClass.SchoolId);
                    school.ModifiedOn = DateTime.Now;
                    school.ModifiedBy = currentUser;

                    _unitOfWork.SchoolRepo.Update(school);
                }

                var result = await _unitOfWork.SaveChangesAsync();
                if (result <= 0)
                {
                    throw new Exception("Lỗi trong quá trình lưu thay đổi");
                }
                return true;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở StudentClassServices - DeleteStudentClass: " + e.Message);
            }
            
        }

        public async Task<IEnumerable<StudentClassViewModels>> GetAllStudentClass(Guid? teacherId = null)
        {
            var studentClasses = await _unitOfWork.StudentClassRepo.GetAllAsync();

            if (teacherId != null)
            {
                studentClasses = studentClasses.Where(studentClass => studentClass.CreatedBy == teacherId).ToList();
            }

            var studentClassViewModels = _mapper.Map<IEnumerable<StudentClassViewModels>>(studentClasses);
            return studentClassViewModels;
        }

        public async Task<ClassInfo> GetStudentClassById(Guid id)
        {
            var studentClass = await _unitOfWork.StudentClassRepo.FindByField(studentClass => studentClass.ClassId == id);
            var students = await _unitOfWork.StudentRepo.FindListByField(student => student.ClassId == id);
            var studentInfo =  students != null ? _mapper.Map<List<StudentInfo>>(students) : null;

            var examOfClass = await _unitOfWork.ExamRepo.FindListByField(examOfClass => examOfClass.ClassId == id);
            List<ExamViewModels> examInfo = examOfClass != null ? new List<ExamViewModels>() : null;

            if (examOfClass != null)
            {
                foreach (var exam in examOfClass)
                {
                    var examViewModel = _mapper.Map<ExamViewModels>(exam);
                    examViewModel.ClassName = studentClass.Name;
                    var examMark = await _unitOfWork.ExamMarkRepo.FindListByField(examMark => examMark.ExamId == exam.ExamId);
                    var count = examMark.Count(examMark => examMark.Mark != null);
                    examViewModel.HasMark = count +  "/" + examMark.Count;
                    examInfo.Add(examViewModel);
                }
            }
    
            var classInfo = new ClassInfo
            {
                ExamViews  = examInfo,
                Students = studentInfo,
            };

            return classInfo;
        }

        public async Task<bool> UpdateStatusOfStudentClass(Guid id, int status)
        {
            var studentClass = await _unitOfWork.StudentClassRepo.FindByField(studentClass => studentClass.ClassId == id);
            if (studentClass == null)
            {
                return false;
            }

            studentClass.Status = status;
            studentClass.ModifiedOn = DateTime.Now;
            studentClass.ModifiedBy = await GetCurrentUser();

            try
            {
                _unitOfWork.StudentClassRepo.Update(studentClass);
                var result = await _unitOfWork.SaveChangesAsync();
                if (result <= 0)
                {
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở StudentClassServices - UpdateStatusOfStudentClass: " + e.Message);
            }
        }

        public async Task<bool> UpdateStudentClass(Guid classId, StudentClassUpdate studentClassUpdate)
        {
            var studentClass = await _unitOfWork.StudentClassRepo.FindByField(studentClass => studentClass.ClassId == classId);
            if (studentClass == null)
            {
                throw new Exception("Lớp học không tồn tại");
            }

            if (studentClass.Status == 0)
            {
                throw new Exception("Lớp đã ngừng hoạt động");
            }

            var currentUser = await GetCurrentUser();

            if (studentClass.CreatedBy != currentUser)
            {
                throw new Exception("Bạn không có quyền chỉnh sửa lớp học này");
            }

            studentClass.ModifiedBy = currentUser;
            studentClass.ModifiedOn = DateTime.Now;

            studentClass = _mapper.Map(studentClassUpdate, studentClass);
            studentClass.ClassId = classId; 

            try 
            {
                _unitOfWork.StudentClassRepo.Update(studentClass);
                var result = await _unitOfWork.SaveChangesAsync();
                
                return result > 0;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở StudentClassServices - UpdateStudentClass: " + e.Message);
            } 
        }

        public async Task<Guid> GetCurrentUser()
        {
            return _claimsService.GetCurrentUser;
        }

        public async Task<bool> AddStudentIntoClass(Guid classId, StudentCreate studentClassCreate)
        {
            var student = _mapper.Map<Student>(studentClassCreate);
            student.ClassId = classId;

            var studentClass = await _unitOfWork.StudentClassRepo.FindByField(studentClass => studentClass.ClassId == classId);
            
            if (studentClass == null)
            {
                throw new Exception("Lớp học không tồn tại");
            }

            if (studentClass.Status == 0)
            {
                throw new Exception("Lớp đã ngừng hoạt động");
            }

            var currentUser = await GetCurrentUser();

            if (studentClass.CreatedBy != currentUser)
            {
                throw new Exception("Bạn không có quyền thêm học sinh vào lớp học này");
            }

            studentClass.ModifiedOn = DateTime.Now;
            studentClass.ModifiedBy = currentUser;

            try
            {
                _unitOfWork.StudentRepo.AddAsync(student);
                _unitOfWork.StudentClassRepo.Update(studentClass);

                var result = await _unitOfWork.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở StudentClassServices - AddStudentIntoClass: " + e.Message);
            }
        }

        public async Task<bool> DeleteStudentFromClass(Guid studentId)
        {
            var student = await _unitOfWork.StudentRepo.FindByField(student => student.StudentId == studentId);
            if (student == null)
            {
                throw new Exception("Học sinh không tồn tại");
            }

            var studentClass = await _unitOfWork.StudentClassRepo.FindByField(studentClass => studentClass.ClassId == student.ClassId);
            if (studentClass == null)
            {
                throw new Exception("Lớp học không tồn tại");
            }

            var currentUser = await GetCurrentUser();

            if (studentClass.CreatedBy != currentUser)
            {
                throw new Exception("Bạn không có quyền xóa học sinh khỏi lớp học này");
            }

            studentClass.ModifiedOn = DateTime.Now;
            studentClass.ModifiedBy = currentUser;

            try
            {
                _unitOfWork.StudentRepo.Remove(student);
                _unitOfWork.StudentClassRepo.Update(studentClass);

                var result = await _unitOfWork.SaveChangesAsync();
                if (result <= 0)
                {
                    throw new Exception("Lỗi trong quá trình lưu lớp học");
                }

                return true;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở StudentClassServices - DeleteStudentFromClass: " + e.Message);
            }
        }

        public async Task<IEnumerable<StudentViewModels>> ImportExcelToAddStudent(Guid classId, IFormFile file)
        {
            
            try
            {
                var studentClass = await _unitOfWork.StudentClassRepo.FindByField(studentClass => studentClass.ClassId == classId);

                if (studentClass == null)
                {
                    throw new Exception("Lớp không tồn tại.");
                }

                if (studentClass.Status == 0)
                {
                    throw new Exception("Lớp đã ngừng hoạt động");
                }

                var list = new List<Student>();

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;
                        var columnCount = worksheet.Dimension.Columns;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            string name = "";

                            for (int column = 1; column <= columnCount; column++)
                            {
                                name += worksheet.Cells[row, column].Value.ToString().Trim() + " ";
                            }

                            list.Add(new Student { ClassId = classId, FullName = name.Trim() });
                        }
                    }

                }

                foreach (var item in list)
                {
                    _unitOfWork.StudentRepo.AddAsync(item);
                }

                var result = await _unitOfWork.SaveChangesAsync();
                var studentViewModels = _mapper.Map<IEnumerable<StudentViewModels>>(list);
                return studentViewModels;

            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi ở StudentClassServices - ImportExcelToAddStudent: " + ex.Message);
            }
        }

       
    }
}
