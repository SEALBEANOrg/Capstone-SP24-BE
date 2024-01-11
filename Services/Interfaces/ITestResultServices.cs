namespace Services.Interfaces
{
    public interface ITestResultServices
    {
        Task<bool> CheckPermissionAccessTest(string testCode, string email);
    }
}
