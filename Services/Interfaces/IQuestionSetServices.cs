using Services.ViewModels;

namespace Services.Interfaces
{
    public interface IQuestionSetServices
    {
        Task<QuestionSetViewModel> GetQuestionByQuestionSetId(Guid id);
    }
}
