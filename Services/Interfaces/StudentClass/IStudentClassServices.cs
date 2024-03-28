using Microsoft.AspNetCore.Http;
using Services.ViewModels;

namespace Services.Interfaces.StudentClass
{
    public interface IStudentClassServices
    {
        Task<bool> CreateStudentClass(StudentClassCreate studentClassCreate);
        Task<bool> AddStudentIntoClass(Guid classId, StudentCreate studentClassCreate);
        Task<IEnumerable<StudentViewModels>> ImportExcelToAddStudent(Guid classId, IFormFile file);

        Task<IEnumerable<StudentClassViewModels>> GetAllStudentClass(string studyYear, Guid? teacherId = null);
        Task<ClassInfo> GetStudentClassById(Guid id);
        Task<IEnumerable<ComboClass>> GetComboClass(Guid currentUserId, int? grade);

        Task<bool> UpdateStudentClass(Guid classId, StudentClassUpdate studentClassUpdate);
        Task<bool> UpdateStatusOfStudentClass(Guid id, int status);

        Task<bool> DeleteStudentClass(Guid id);
        Task<bool> DeleteStudentFromClass(Guid studentId);

        Task<Guid> GetCurrentUser();
    }
}
