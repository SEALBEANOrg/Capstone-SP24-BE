using Services.ViewModels;

namespace Services.Interfaces
{
    public interface IQuestionServices
    {
        Task<bool> AddQuestions(QuestionCreate questionCreate, Guid currentUser);
        Task<bool> DeleteQuestion(Guid questionId, Guid currentUser);
        Task<IEnumerable<QuestionViewModels>> GetAllQuestion();
        Task<QuestionViewModels> GetQuestionByQuestionId(Guid questionId);
        Task<bool> UpdateQuestion(QuestionUpdate questionUpdate, Guid currentUser);




    }
}
