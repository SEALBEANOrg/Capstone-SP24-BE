﻿using Services.ViewModels;

namespace Services.Interfaces.Exam
{
    public interface IExamMobileServices
    {
        Task<bool> CheckPermissionAccessTest(string testCode, string email);
        Task<InfoClassInExam> GetInfoOfClassInExam(string testCode, string email);
        Task<ResultForScan> SendImage(ResultForScanViewModel Image);
        Task<int> SaveResult(ResultToSave resultToSave);

    }
}
