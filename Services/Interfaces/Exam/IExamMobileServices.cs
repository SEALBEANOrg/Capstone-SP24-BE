using Services.ViewModels;

namespace Services.Interfaces.Exam
{
    public interface IExamMobileServices
    {
        Task<bool> CheckPermissionAccessTest(string testCode, string email);
        Task<InfoClassInExam> GetInfoOfClassInExam(string testCode, string email);
        Task<Response> SendImage(ResultForScanViewModel Image);
        Task<bool> SaveResult(ResultToSave resultToSave);

    }
}
