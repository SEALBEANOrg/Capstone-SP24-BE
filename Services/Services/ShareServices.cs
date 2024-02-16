using AutoMapper;
using Repositories;
using Services.Interfaces;
using Services.ViewModels;

namespace Services.Services
{
    public class ShareServices : IShareServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ShareServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ShareViewModels>> GetRequestToShare(int? status, int? grade, int? subject, int? type)
        {
            try
            {
                var questionSetIds = new List<Guid>();
                if (grade != null && subject == null)
                {
                    questionSetIds = (await _unitOfWork.QuestionSetRepo.GetAllAsync(questionSet => questionSet.Grade == grade)).Select(questionSet => questionSet.QuestionSetId).ToList();
                }
                else if (grade == null && subject != null)
                {
                    questionSetIds = (await _unitOfWork.QuestionSetRepo.GetAllAsync(questionSet => questionSet.Subject == subject)).Select(questionSet => questionSet.QuestionSetId).ToList();
                }
                else 
                {
                    questionSetIds = (await _unitOfWork.QuestionSetRepo.GetAllAsync(questionSet => questionSet.Grade == grade && questionSet.Subject == subject)).Select(questionSet => questionSet.QuestionSetId).ToList();
                }

                if ((grade != null || subject != null) && questionSetIds.Count() == 0)
                {
                    return null;
                }

                var shares = await _unitOfWork.ShareRepo.GetAllAsync(x => x.QuestionSetId);
            
                if (status != null)
                {
                    shares = shares.Where(share => share.Status == status).ToList();
                }
                if (type != null)
                {
                    shares = shares.Where(share => share.Type == type).ToList();
                }
                if (questionSetIds.Count() > 0)
                {
                    shares = shares.Where(share => questionSetIds.Contains((Guid)share.QuestionSetId)).ToList();
                }

                if (shares == null)
                {
                    return null;
                }

                var result = _mapper.Map<IEnumerable<ShareViewModels>>(shares);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi ở ShareServices - GetRequestToShare: " + ex.Message);
            }
        }

        public Task<ShareViewModel> GetRequestToShareById(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ResponseRequestShare(Guid id, ResponseRequest responseRequest, Guid currentUserId)
        {
            try 
            {                 
                var share = await _unitOfWork.ShareRepo.FindByField(share => share.ShareId == id);
                if (share == null)
                {
                    return false;
                }

                share.Status = responseRequest.IsAccept ? 1 : 2;
                share.Note = responseRequest.Note;
                share.ModifiedBy = currentUserId;
                share.ModifiedOn = DateTime.Now;
                _unitOfWork.ShareRepo.Update(share);
                var result = await _unitOfWork.SaveChangesAsync();

                return result > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi ở ShareServices - ResponseRequestShare: " + ex.Message);
            }
        }
    }
}
