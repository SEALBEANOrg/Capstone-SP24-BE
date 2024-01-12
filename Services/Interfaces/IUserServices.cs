using Services.ViewModels;

namespace Services.Interfaces
{
    public interface IUserServices
    {
        Task<UserInfo?> CreateNewUser(UserSignUp userSignUp);
        Task<UserInfo> FindUserByEmail(string email);
        Task<UserInfo> FindUserById(Guid id);
        Task<IEnumerable<UserViewModels>> GetAllUser();
        Task<bool> RequestJoinSchool(Guid schoolId);
        Task<bool> ResponseRequest(Guid userId, bool isAccept);
        Task<Guid> GetCurrentUser();
        Task<IEnumerable<Request>> GetListRequestToMySchool();
        Task<string> RegisterAsync(string email);
        Task<bool> CheckExistInFirebase(string email);
        Task<UserViewModels> GetProfile();
        Task<bool> UpdateProfile(UserUpdate userUpdate);
        Task<UserViewModels> GetUserById(Guid id);
        Task<bool> OutSchool();
    }
}
