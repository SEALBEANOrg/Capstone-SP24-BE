using Services.ViewModels;

namespace Services.Interfaces
{
    public interface IUserServices
    {
        Task<UserInfo?> CreateNewUser(UserSignUp userSignUp);
        Task<UserInfo> FindUserByEmail(string email);
        Task<UserInfo> FindUserById(Guid id);
        Task<IEnumerable<UserViewModels>> GetAllUser(string search);
        Task<bool> RequestJoinSchool(Guid schoolId);
        Task<bool> ResponseRequest(Guid userId, bool isAccept);
        Task<Guid> GetCurrentUser();
        Task<IEnumerable<Request>> GetListRequestToMySchool();
        Task<string> RegisterAsync(string email);
        Task<bool> CheckExistInFirebase(string email); 
        Task<UserViewModels> GetProfile();
        Task<bool> UpdateProfile(ProfileUpdate userUpdate);
        Task<UserViewModels> GetUserById(Guid id);
        Task<bool> OutSchool();
        Task<bool> UpdateRoleUser(Guid id, RoleUpdate roleUpdate);
        Task<bool> ChangeStatusOfUser(Guid id, bool isActive);
    }
}
