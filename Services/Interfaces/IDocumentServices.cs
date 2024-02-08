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
        Task<bool> DeleteDocument(Guid documentId);
        Task<DocumentViewModels> GetAllDocument(int? type);
        Task<DocumentViewModel> GetDocumentById(Guid documentId);
    }
}
