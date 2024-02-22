using Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IDocumentServices
    {
        Task<bool> AddDocument(DocumentCreate documentCreate, Guid currentUser);
        Task<byte[]> CreateTestPaper(Guid? examId, Guid currentUserId, DetailOfPaper documentCreate);
        Task<bool> DeleteDocument(Guid documentId);
        Task<IEnumerable<DocumentViewModels>> GetAllDocument(int? type);
        Task<DocumentViewModel> GetDocumentById(Guid documentId);
        Task<bool> UploadDocument(MemoryStream stream, DocumentCreate documentCreate);
    }
}
