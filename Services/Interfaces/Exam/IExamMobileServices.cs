﻿using Services.ViewModels;

namespace Services.Interfaces.Exam
{
    public interface IExamMobileServices
    {
        Task<InfoClassInExam> GetInfoOfClassInExam(string testCode, string email);
        Task<ResultForScan> SendImage(ResultForScanViewModel Image);
        Task<int> SaveResult(ResultToSave resultToSave);

    }
}
