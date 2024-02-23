using Services.ViewModels;

namespace Services.Interfaces
{
    public interface IExamServices
    {
        Task<bool> CheckPermissionAccessTest(string testCode, string email);
        Task<ExamInfo> GetExamInfo(Guid examId, Guid currentUserId);
        Task<InfoClassInExam> GetInfoOfClassInExam(string testCode, string email);
        Task<decimal?> SaveResult(ResultToSave resultToSave);
    }
}
