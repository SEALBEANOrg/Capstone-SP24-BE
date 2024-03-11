using Services.ViewModels;

namespace Services.Interfaces
{
    public interface IShareServices
    {
        Task<bool> BuyQuestionSet(BuyQuestionSet buyQuestionSet, Guid currentUser);
        Task<IEnumerable<ShareInMarket>> GetQuestionSetInMarket(int? grade, int? subjectEnum, int year, Guid currentUser);
        Task<IEnumerable<ShareViewModels>> GetRequestToShare(int? status, int? grade, int? subjectEnum, int? type, int year);
        Task<ShareViewModel> GetRequestToShareById(Guid id);
        Task<List<string>> GetUserEmailOfSharedQuestionSet(Guid questionSetId, Guid currentUserId, int? type);
        Task<List<ShareInMarket>> GetBoughtList(Guid currentUser, int? grade, int? subjectEnum, int year);
        Task<List<MySold>> GetSoldList(Guid currentUser, int? grade, int? subjectEnum, int? status, int year);
        Task<bool> ReportShare(Guid shareId, Guid currentUser, NoteReport noteReport);
        Task<bool> RequestToShare(ShareCreateRequest shareCreate, Guid currentUser);
        Task<bool> ResponseRequestShare(Guid id, ResponseRequest responseRequest, Guid currentUserId);
        Task<bool> ShareIndividual(ShareCreateForIndividual shareIndividual, Guid currentUser);
    }
}
