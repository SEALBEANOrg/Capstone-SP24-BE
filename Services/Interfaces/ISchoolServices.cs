using Services.ViewModels;

namespace Services.Interfaces
{
    public interface ISchoolServices
    {
        Task<bool> AddNewSchool(SchoolForCreateViewModel schoolForCreateViewModel, Guid currentUserId);
        Task<bool> ChangeSchoolAdmin(Guid schoolId, string email, Guid currentUserId);
        Task<bool> ChangeStatusOfSchool(Guid schoolId, int status);
        Task<bool> DeleteSchool(Guid schoolId);
        Task<IEnumerable<StudentClassViewModels>> GetAllClassOfMySchool(Guid currentUserId);
        Task<IEnumerable<SchoolList>> GetAllSchool(string? search, int status);
        Task<IEnumerable<ComboSchool>> GetComboSchool();
        Task<SchoolViewModels> GetInfoMySchool(Guid currentUserId);
        Task<SchoolViewModels> GetSchoolById(Guid schoolId);
        Task<IEnumerable<UserViewModels>> GetTeacherOfMySchool(Guid currentUserId);
        Task<bool> RemoveTeacherFromSchool(Guid teacherId, Guid currentUser);
        Task<bool> UpdateSchool(SchoolForUpdateViewModel schoolForUpdateViewModel);
    }
}
