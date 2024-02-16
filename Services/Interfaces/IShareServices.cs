using Services.ViewModels;

namespace Services.Interfaces
{
    public interface IShareServices
    {
        Task<IEnumerable<ShareViewModels>> GetRequestToShare(int? status, int? grade, int? subject, int? type);
        Task<ShareViewModel> GetRequestToShareById(Guid id);
        Task<bool> ResponseRequestShare(Guid id, ResponseRequest responseRequest, Guid currentUserId);
    }
}
