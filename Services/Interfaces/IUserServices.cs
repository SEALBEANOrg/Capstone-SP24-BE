using DocumentFormat.OpenXml.Math;
using Services.ViewModels;

namespace Services.Interfaces
{
    public interface IUserServices
    {
        Task<UserInfo?> CreateNewUser(UserSignUp userSignUp);
        Task<UserInfo> FindUserByEmail(string email);
        Task<UserInfo> FindUserById(Guid id);
        Task<IEnumerable<UserViewModels>> GetAllUser(string? search, int? role, int? status);
        Task<Guid> GetCurrentUser();
        Task<string> RegisterAsync(string email);
        Task<bool> CheckExistInFirebase(string email); 
        Task<UserViewModels> GetProfile();
        Task<bool> UpdateProfile(ProfileUpdate userUpdate);
        Task<UserViewModels> GetUserById(Guid id);
        Task<bool> UpdateRoleUser(Guid id, RoleUpdate roleUpdate);
        Task<bool> ChangeStatusOfUser(Guid id, bool isActive);
    }
}
