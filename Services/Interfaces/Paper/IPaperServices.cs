using OfficeOpenXml;
using Services.ViewModels;

namespace Services.Interfaces.Paper
{
    public interface IPaperServices
    {
        Task<Guid> CreateTestPaper(Guid currentUserId, Guid paperSetId, DetailOfPaper documentCreate, Guid templatePaperId, bool shuffleAnswers, ExcelWorksheet worksheet);
        
        Task<string> GetPaperById(Guid paperId);
    }
}
