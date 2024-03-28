using Services.ViewModels;

namespace Services.Interfaces
{
    public interface IExamServices
    {
        Task<Guid?> AddExamByMatrixIntoClass(ExamCreate examCreate, Guid currentUserId);
        
        Task<IEnumerable<ExamViewModels>> GetOwnExam(Guid currentUserId, int? grade, string studyYear);
        Task<ExamInfo> GetExamInfo(Guid examId, Guid currentUserId);
        Task<ExamSourceViewModel> GetAllExamSource(Guid examId);

        Task<ExportResult> ExportResult(Guid examId);
    }
}
