using Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces.Exam
{
    public interface IExamMarkServices
    {
        Task<ExamInfo> CalculateAllMark(Guid examId, Guid currentUserId);

    }
}
