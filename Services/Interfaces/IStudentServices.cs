using Services.ViewModels;

namespace Services.Interfaces
{
    public interface IStudentServices
    {
        Task<StudentViewModels> GetStudentById(Guid studentId);
        Task<IEnumerable<StudentViewModels>> GetStudentsOfClass(Guid classId, Guid currentUser);
        Task<bool> MoveOutStudent(Guid studentId, Guid currentUser);
        Task<bool> UpdateStudent(StudentUpdate studentUpdate, Guid currentUser);
    }
}
