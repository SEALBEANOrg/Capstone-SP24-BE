using Services.ViewModels;

namespace Services.Interfaces
{
    public interface IUserServices
    {
        Task<UserInfo?> CreateNewUser(UserSignUp userSignUp);
        Task<UserInfo> FindUserByEmail(string email);
        Task<UserInfo> FindUserById(Guid id);
        Task<UserViewModels> GetAllUser();
        Task<bool> RequestJoinSchool(Guid schoolId);
        Task<bool> ResponseRequest(Guid userId, bool isAccept);
        Task<Guid> GetCurrentUser();
        Task<IEnumerable<Request>> GetListRequestToMySchool();
        Task<string> RegisterAsync(string email);
        Task<bool> CheckExistInFirebase(string email);
    }
}
