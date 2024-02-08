using AutoMapper;
using Repositories;
using Repositories.Models;
using Services.Interfaces;
using Services.ViewModels;

namespace Services.Services
{
    public class DocumentServices : IDocumentServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DocumentServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> AddDocument(DocumentCreate documentCreate, Guid currentUser)
        {
            try {
                var document = _mapper.Map<Document>(documentCreate);
                
                document.CreatedBy = currentUser;           
                document.CreatedOn = DateTime.Now;
            
                _unitOfWork.DocumentRepo.AddAsync(document);
                var result = await _unitOfWork.SaveChangesAsync();
            
                return result > 0;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở DocumentServices - AddDocument: " + e.Message);
            }
        }

        public async Task<bool> DeleteDocument(Guid documentId)
        {
            try
            {
                var document = await _unitOfWork.DocumentRepo.FindByField(document => document.DocumentId == documentId);
                
                if (document == null)
                {
                    return false;
                }

                _unitOfWork.DocumentRepo.Remove(document);
                var result = await _unitOfWork.SaveChangesAsync();

                return result > 0;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở DocumentServices - DeleteDocument: " + e.Message);
            }
        }

        public async Task<DocumentViewModels> GetAllDocument(int? type)
        {
            try
            {
                var documents = await _unitOfWork.DocumentRepo.GetAllAsync();
                documents = type != null ? documents.Where(document => document.Type == type).ToList() : documents;

                if (documents == null)
                {
                    return null;
                }

                var documentViewModels = _mapper.Map<DocumentViewModels>(documents);

                return documentViewModels;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở DocumentServices - GetAllDocument: " + e.Message);
            }
        }

        public async Task<DocumentViewModel> GetDocumentById(Guid documentId)
        {
            try
            {
                var document = await _unitOfWork.DocumentRepo.FindByField(document => document.DocumentId == documentId);

                if (document == null)
                {
                    return null;
                }

                var documentViewModel = _mapper.Map<DocumentViewModel>(document);

                return documentViewModel;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở DocumentServices - GetDocumentById: " + e.Message);
            }
        }
    }
}
