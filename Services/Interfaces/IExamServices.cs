using Services.ViewModels;

namespace Services.Interfaces
{
    public interface IExamServices
    {
        Task<Guid?> AddExamByMatrixIntoClass(ExamCreate examCreate, Guid currentUserId);
        Task<bool> CheckPermissionAccessTest(string testCode, string email);
        Task<ExportResult> ExportResult(Guid examId);
        Task<ExamSourceViewModel> GetAllExamSource(Guid examId);
        Task<ExamInfo> GetExamInfo(Guid examId, Guid currentUserId);
        Task<InfoClassInExam> GetInfoOfClassInExam(string testCode, string email);
        Task<IEnumerable<ExamViewModels>> GetOwnExam(Guid currentUserId, int? grade);
        Task<byte[]> GetPaperById(Guid paperId);
        Task<decimal?> SaveResult(ResultToSave resultToSave);
        Task<Response> SendImage(ResultForScanViewModel Image);
    }
}
