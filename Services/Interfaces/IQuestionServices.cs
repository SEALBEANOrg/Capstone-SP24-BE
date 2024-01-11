using Services.ViewModels;

namespace Services.Interfaces
{
    public interface IQuestionServices
    {
        Task<bool> AddQuestions(QuestionCreate questionCreate, Guid currentUser);
        Task<bool> DeleteQuestion(Guid questionId, Guid currentUser, int grade);
        Task<IEnumerable<QuestionViewModels>> GetAllMyQuestionByGrade(int grade, Guid currentUserId);
        Task<IEnumerable<QuestionViewModels>> GetAllValidQuestionByGradeForMe(int grade, Guid currentUserId);
        Task<QuestionViewModels> GetQuestionByQuestionIdAndGrade(Guid questionId, int grade, Guid currentUserId);
        Task<IEnumerable<QuestionViewModels>> GetQuestionBySectionIdAndGrade(Guid sectionId, int grade);
        Task<IEnumerable<QuestionViewModels>> GetQuestionBySubjectAndGrade(int subject, int grade);
        Task<bool> UpdateQuestion(QuestionUpdate questionUpdate, Guid currentUser, int grade);




    }
}
