using Services.ViewModels;

namespace Services.Interfaces
{
    public interface IQuestionSetServices
    {
        Task<bool> ChangeStatusQuestionSet(Guid questionSetId, bool isActive, Guid currentUser);
        Task<QuestionSetViewModel> GetQuestionByQuestionSetId(Guid id);
    }
}
