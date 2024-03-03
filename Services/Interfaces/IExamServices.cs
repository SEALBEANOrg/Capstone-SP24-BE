using Services.ViewModels;

namespace Services.Interfaces
{
    public interface IExamServices
    {
        Task<bool> AddExamByMatrixIntoClass(ExamCreate examCreate, Guid currentUserId);
        Task<bool> CheckPermissionAccessTest(string testCode, string email);
        Task<ExamInfo> GetExamInfo(Guid examId, Guid currentUserId);
        Task<InfoClassInExam> GetInfoOfClassInExam(string testCode, string email);
        Task<IEnumerable<ExamViewModels>> GetOwnExam(Guid currentUserId, int? grade);
        Task<decimal?> SaveResult(ResultToSave resultToSave);
        Task<Response> SendImage(ResultForScanViewModel Image);
    }
}
