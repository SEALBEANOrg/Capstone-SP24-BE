using AutoMapper;
using ExagenSharedProject;
using Repositories;
using Services.Interfaces.Share;
using Services.ViewModels;

namespace Services.Services.Share
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

        public async Task<bool> ShareIndividual(ShareCreateForIndividual shareIndividual, Guid currentUser)
        {
            try
            {
                var userID = await _unitOfWork.UserRepo.FindListByField(u => shareIndividual.Email.Contains(u.Email));
                if (userID == null)
                {
                    throw new Exception("Người dùng không tồn tại");
                }

                var shared = await _unitOfWork.ShareRepo.FindByField(share => share.QuestionSetId == shareIndividual.QuestionSetId && share.Type == 1 && share.UserId == currentUser);
                if (shared != null && shared.Status == 1)
                {
                    throw new Exception("Bộ câu hỏi đã được chia sẻ cho người dùng này");
                }
                else if (shared != null && shared.Status == 0)
                {
                    shared.Status = 1;
                    shared.ModifiedBy = currentUser;
                    shared.ModifiedOn = DateTime.Now;

                    _unitOfWork.ShareRepo.Update(shared);
                    var result = await _unitOfWork.SaveChangesAsync();

                    return result > 0;
                }
                else
                {
                    var shareResult = new List<Repositories.Models.Share>();
                    foreach (var user in userID)
                    {
                        var share = new Repositories.Models.Share
                        {
                            QuestionSetId = shareIndividual.QuestionSetId,
                            UserId = user.UserId,
                            Type = 1,
                            Status = 1,
                            CreatedBy = currentUser,
                            CreatedOn = DateTime.Now,
                            ModifiedBy = currentUser,
                            ModifiedOn = DateTime.Now
                        };

                        shareResult.Add(share);
                    }

                    _unitOfWork.ShareRepo.AddRangeAsync(shareResult);

                    var result = await _unitOfWork.SaveChangesAsync();

                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi ở ShareServices - ShareIndividual: " + ex.Message);
            }
        }
        
        public async Task<bool> RequestToShare(ShareCreateRequest shareCreate, Guid currentUser)
        {
            try
            {
                var share = new Repositories.Models.Share
                {
                    QuestionSetId = shareCreate.QuestionSetId,
                    Type = shareCreate.Type,
                    Status = 0,
                    CreatedBy = currentUser,
                    CreatedOn = DateTime.Now,
                    ModifiedBy = currentUser,
                    ModifiedOn = DateTime.Now
                };

                _unitOfWork.ShareRepo.AddAsync(share);
                var result = await _unitOfWork.SaveChangesAsync();

                return result > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi ở ShareServices - RequestToShare: " + ex.Message);
            }
        }

        public async Task<IEnumerable<ShareViewModels>> GetRequestToShare(int? status, int? grade, int? subjectEnum, int? type, string studyYear)
        {
            try
            {
                var shares = await _unitOfWork.ShareRepo.FindListByField(share => share.StudyYear == studyYear && share.UserId == null, includes => includes.QuestionSet);

                if (type == 1)
                {
                    throw new Exception("Không thể lấy thông tin về loại chia sẻ này");
                }
                else
                {
                    shares = shares.Where(share => share.Type == 0 || share.Type == 2).ToList();
                }

                if (grade != null && subjectEnum != null)
                {
                    var subjectId = (await _unitOfWork.SubjectRepo.FindListByField(subject => EnumStatus.Subject[(int)subjectEnum].ToLower().Contains(subject.Name) && subject.Grade == grade)).Select(s => s.SubjectId);

                    shares = shares.Where(share => subjectId.ToList().Contains((Guid)share.QuestionSet.SubjectId)).ToList();
                }
                else if (grade == null && subjectEnum != null)
                {
                    var subjectId = (await _unitOfWork.SubjectRepo.FindListByField(subject => EnumStatus.Subject[(int)subjectEnum].ToLower().Contains(subject.Name))).Select(s => s.SubjectId);
                    shares = shares.Where(share => subjectId.ToList().Contains((Guid)share.QuestionSet.SubjectId)).ToList();
                }
                else if (grade != null && subjectEnum == null)
                {
                    shares = shares.Where(share => share.QuestionSet.Grade == grade).ToList();
                }

                if (status != null)
                {
                    shares = shares.Where(share => share.Status == status).ToList();
                }

                if (shares == null)
                {
                    return null;
                }
                var users = await _unitOfWork.UserRepo.GetAllAsync();
                var result = new List<ShareViewModels>();

                foreach (var s in shares)
                {
                    var questions = await _unitOfWork.QuestionRepo.FindListByField(question => question.QuestionSetId == s.QuestionSetId);
                    var config = new SetConfig
                    {
                        NB = questions.Count(c => c.Difficulty == 0),
                        TH = questions.Count(c => c.Difficulty == 1),
                        VDT = questions.Count(c => c.Difficulty == 2),
                        VDC = questions.Count(c => c.Difficulty == 3),
                    };
                    var shareViewModel = _mapper.Map<ShareViewModels>(s);
                    shareViewModel.Price = s.Type == 0 ? (config.NB * 2 + config.TH * 5 + config.VDT * 10 + config.VDC * 30) / 5 : null;
                    shareViewModel.NameOfQuestionSet = s.QuestionSet.Name;
                    shareViewModel.NameOfSubject = (await _unitOfWork.SubjectRepo.FindByField(subject => subject.SubjectId == s.QuestionSet.SubjectId)).Name;
                    shareViewModel.NameOfRequester = users.First(u => u.UserId == s.CreatedBy).FullName;
                    result.Add(shareViewModel);
                }

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

        public async Task<List<string>> GetUserEmailOfSharedQuestionSet(Guid questionSetId, Guid currentUserId, int? type)
        {
            try
            {
                var shares = await _unitOfWork.ShareRepo.FindListByField(share => share.QuestionSetId == questionSetId && share.CreatedBy == currentUserId && share.UserId != null, include => include.User);

                if (shares == null)
                {
                    return null;
                }

                if (type != null)
                {
                    shares = shares.Where(share => share.Type == type).ToList();
                }

                return shares.Select(s => s.User.Email).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi ở ShareServices - GetUserEmailOfSharedQuestionSet: " + ex.Message);
            }
        }

        public async Task<bool> ReportShare(Guid shareId, Guid currentUser, NoteReport noteReport)
        {
            try
            {
                var share = await _unitOfWork.ShareRepo.FindByField(s => s.ShareId == shareId);
                if (share == null)
                {
                    return false;
                }

                var currentUserInfo = await _unitOfWork.UserRepo.FindByField(u => u.UserId == currentUser);

                share.Note += $"{DateTime.Now} - {currentUserInfo.Email} : {noteReport.Note}";

                _unitOfWork.ShareRepo.AddAsync(share);
                var result = await _unitOfWork.SaveChangesAsync();

                return result > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi ở ShareServices - ReportShare: " + ex.Message);
            }
        }

        public async Task<bool> ResponseRequestShare(Guid id, ResponseRequest responseRequest, Guid currentUserId)
        {
            try
            {
                var share = await _unitOfWork.ShareRepo.FindByField(share => share.ShareId == id && share.Status == 0);
                if (share == null)
                {
                    return false;
                }

                share.Status = responseRequest.IsAccept ? 1 : 2;
                share.Note = responseRequest.Note;
                share.ModifiedBy = currentUserId;
                share.ModifiedOn = DateTime.Now;

                if (share.Type == 2 && share.Status == 1)
                {
                    var questionSet = await _unitOfWork.QuestionSetRepo.FindByField(q => q.QuestionSetId == share.QuestionSetId, include => include.Questions);
                    SetConfig setConfig = new SetConfig
                    {
                        NB = questionSet.Questions.Count(q => q.Difficulty == 0),
                        TH = questionSet.Questions.Count(q => q.Difficulty == 1),
                        VDT = questionSet.Questions.Count(q => q.Difficulty == 2),
                        VDC = questionSet.Questions.Count(q => q.Difficulty == 3)
                    };
                    var user = await _unitOfWork.UserRepo.FindByField(u => u.UserId == share.UserId);
                    user.Point += setConfig.NB * 2 + setConfig.TH * 5 + setConfig.VDT * 10 + setConfig.VDC * 30;
                    _unitOfWork.UserRepo.Update(user);

                    var transaction = new Repositories.Models.Transaction
                    {
                        UserId = currentUserId,
                        Type = 6, //public bo cau hoi
                        PointValue = setConfig.NB * 2 + setConfig.TH * 5 + setConfig.VDT * 10 + setConfig.VDC * 30,
                        CreatedOn = DateTime.Now
                    };
                    _unitOfWork.TransactionRepo.AddAsync(transaction);
                }

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
