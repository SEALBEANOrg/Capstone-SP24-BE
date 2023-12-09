using Services.ViewModels;

namespace Services.Interfaces
{
    public interface ISchoolServices
    {
        Task<IEnumerable<StudentClassViewModels>> GetAllClassOfMySchool(Guid currentUserId);
        Task<IEnumerable<ComboSchool>> GetComboSchool();
        Task<IEnumerable<UserViewModels>> GetTeacherOfMySchool(Guid currentUserId);
        Task<bool> RemoveTeacherFromSchool(Guid teacherId, Guid currentUser);

    }
}
