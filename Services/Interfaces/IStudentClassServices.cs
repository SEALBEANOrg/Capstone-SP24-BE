using Microsoft.AspNetCore.Http;
using Services.ViewModels;

namespace Services.Interfaces
{
    public interface IStudentClassServices
    {
        Task<IEnumerable<StudentClassViewModels>> GetAllStudentClass(string teacherId = null);
        Task<StudentClassViewModels> GetStudentClassById(Guid id);
        Task<bool> CreateStudentClass(StudentClassCreate studentClassCreate);
        Task<bool> UpdateStudentClass(Guid classId, StudentClassUpdate studentClassUpdate);
        Task<bool> DeleteStudentClass(Guid id);
        Task<bool> UpdateStatusOfStudentClass(Guid id, int status);
        Task<Guid> GetCurrentUser();
        Task<bool> AddStudentIntoClass(Guid classId, StudentCreate studentClassCreate);
        Task<bool> DeleteStudentFromClass(Guid studentId);
        Task<IEnumerable<StudentViewModels>> ImportExcelToAddStudent(Guid classId, IFormFile file);
    }
}
