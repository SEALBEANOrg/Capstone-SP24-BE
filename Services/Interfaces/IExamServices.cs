using Services.ViewModels;

namespace Services.Interfaces
{
    public interface IExamServices
    {
        Task<bool> CheckPermissionAccessTest(string testCode, string email);
        Task<InfoClassInExam> GetInfoOfClassInExam(string testCode, string email);

    }
}
