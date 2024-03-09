using Services.ViewModels;

namespace Services.Interfaces
{
    public interface IQuestionSetServices
    {
        Task<bool> ChangeStatusQuestionSet(Guid questionSetId, bool isActive, Guid currentUser);
        Task<bool> DeleteQuestionSet(Guid questionSetId, Guid currentUser);
        Task<IEnumerable<OwnQuestionSet>> GetOwnQuestionSet(Guid currentUser, int? grade, int? subject, int year);
        Task<QuestionSetViewModel> GetQuestionByQuestionSetId(Guid id);
        Task<IEnumerable<QuestionSetViewModels>> GetQuestionSetBank(int? grade, int? subject, int year, int type);
        Task<QuestionReturn> GetQuestionSetFromFile(ImportQuestionSet importQuestionSet);
        Task<bool> SaveQuestionSet(QuestionSetSave questionSetViewModel, Guid currentUser);
    }
}
