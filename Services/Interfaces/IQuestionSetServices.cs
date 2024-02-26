using Services.ViewModels;

namespace Services.Interfaces
{
    public interface IQuestionSetServices
    {
        Task<bool> ChangeStatusQuestionSet(Guid questionSetId, bool isActive, Guid currentUser);
        Task<bool> DeleteQuestionSet(Guid questionSetId, Guid currentUser);
        Task<IEnumerable<OwnQuestionSet>> GetOwnQuestionSet(Guid currentUser, int? grade, int? subject, int year);
        Task<QuestionSetViewModel> GetQuestionByQuestionSetId(Guid id);
        Task<QuestionReturn> GetQuestionSetFromFile(ImportQuestionSet importQuestionSet);
    }
}
