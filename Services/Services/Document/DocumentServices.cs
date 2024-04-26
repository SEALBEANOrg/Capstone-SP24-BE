using AutoMapper;
using Repositories;
using Services.Interfaces.Document;
using Services.Interfaces.Storage;
using Services.Utilities;
using Services.ViewModels;
using System.Net;

namespace Services.Services.Document
{
    public class DocumentServices : IDocumentServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IS3Services _s3Services;

        public DocumentServices(IUnitOfWork unitOfWork, IMapper mapper, IS3Services s3Services)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _s3Services = s3Services;
        }

        public async Task<bool> AddDocument(DocumentCreate documentCreate, Guid currentUser)
        {
            try
            {
                var document = _mapper.Map<Repositories.Models.Document>(documentCreate);

                var file = documentCreate.FileImport;
                var stream = file.OpenReadStream();
                MemoryStream memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                var keyS3 = "";
                if (documentCreate.Type == 0)
                {
                    keyS3 = "templates/answer-sheet/" + $"{Utils.FormatFileName(documentCreate.Name).Trim()}-{DateTime.Now.AddHours(7).Ticks}.pdf";
                }
                else if (documentCreate.Type == 1)
                {
                    keyS3 = "templates/paper/" + $"{Utils.FormatFileName(documentCreate.Name).Trim()}-{DateTime.Now.AddHours(7).Ticks}.docx";
                }
                var statusCode = await _s3Services.UploadFileIntoS3Async(memoryStream, keyS3);
                if (statusCode == HttpStatusCode.OK)
                {
                    document.KeyS3 = keyS3;
                }
                else
                {
                    return false;
                }
                document.CreatedBy = currentUser;
                document.CreatedOn = DateTime.Now.AddHours(7);

                _unitOfWork.DocumentRepo.AddAsync(document);
                var result = await _unitOfWork.SaveChangesAsync();

                return result > 0;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở DocumentServices - AddDocument: " + e.Message);
            }
        }

        public async Task<IEnumerable<DocumentViewModels>> GetAllDocument(int? type)
        {
            try
            {
                var documents = await _unitOfWork.DocumentRepo.GetAllAsync();
                documents = type != null ? documents.Where(document => document.Type == type).ToList() : documents;

                if (documents == null)
                {
                    return null;
                }

                var documentViewModels = new List<DocumentViewModels>();

                foreach (var document in documents)
                {
                    var documentViewModel = _mapper.Map<DocumentViewModels>(document);
                    documentViewModel.UrlS3 = await _s3Services.GetObjectUrlAsync(document.KeyS3);
                    documentViewModels.Add(documentViewModel);
                }

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
                documentViewModel.UrlS3 = await _s3Services.GetObjectUrlAsync(document.KeyS3);

                return documentViewModel;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở DocumentServices - GetDocumentById: " + e.Message);
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

                if (result > 0)
                {
                    var statusCode = await _s3Services.DeleteObjectAsync(document.KeyS3);
                    return statusCode == HttpStatusCode.NoContent;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở DocumentServices - DeleteDocument: " + e.Message);
            }
        }

        //public async Task<bool> UploadDocument(MemoryStream stream, DocumentCreate documentCreate)
        //{
        //    try
        //    {
        //        var document = new Repositories.Models.Document
        //        {
        //            Name = documentCreate.Name,
        //            Type = documentCreate.Type,
        //            CreatedOn = DateTime.Now.AddHours(7),
        //            CreatedBy = Guid.Parse("00000000-0000-0000-0000-000000000000"),
        //            Description = documentCreate.Description,
        //            Data = stream.ToArray(),
        //        };

        //        _unitOfWork.DocumentRepo.AddAsync(document);
        //        var result = await _unitOfWork.SaveChangesAsync();

        //        return result > 0;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception("Lỗi ở DocumentServices - UploadDocument: " + e.Message);
        //    }
        //}

    }
}