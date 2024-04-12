using Services.ViewModels;

namespace Services.Interfaces.QuestionSet
{
    public interface IQuestionSetServices
    {
        Task<bool> SaveQuestionSet(QuestionSetSave questionSetViewModel, Guid currentUser);
        
        Task<IEnumerable<SectionUse>> GetMatrixOfQuestionSet(Guid questionSetId);
        Task<IEnumerable<OwnQuestionSet>> GetOwnQuestionSet(Guid currentUser, int? grade, int? subject, string studyYear);
        Task<IEnumerable<SharedQuestionSet>> GetSharedQuestionSet(Guid currentUserId, int? grade, int? subjectEnum, int year);
        Task<QuestionSetViewModel> GetQuestionByQuestionSetId(Guid id);
        Task<IEnumerable<QuestionSetViewModels>> GetQuestionSetBank(int? grade, int? subject, string studyYear, int type);
        Task<QuestionReturn> GetQuestionSetFromFile(ImportQuestionSet importQuestionSet);
        
        Task<bool> ChangeStatusQuestionSet(Guid questionSetId, bool isActive, Guid currentUser);
        
        Task<bool> DeleteQuestionSet(Guid questionSetId, Guid currentUser);
    }
}
