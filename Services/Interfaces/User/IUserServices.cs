using DocumentFormat.OpenXml.Math;
using Services.ViewModels;

namespace Services.Interfaces.User
{
    public interface IUserServices
    {
        Task<UserInfo?> CreateNewUser(UserSignUp userSignUp);
        Task<Guid> GetCurrentUser();
        Task<IEnumerable<UserViewModels>> GetAllUser(string? search, int? role, int? status);
        Task<UserViewModel> GetProfile();
        Task<UserInfo> FindUserByEmail(string email);
        Task<UserViewModel> GetUserById(Guid id);
        Task<UserInfo> FindUserById(Guid id);

        Task<bool> UpdateProfile(ProfileUpdate userUpdate);
        Task<bool> UpdateRoleUser(Guid id, RoleUpdate roleUpdate);
        Task<bool> ChangeStatusOfUser(Guid id, bool isActive);
    }
}
