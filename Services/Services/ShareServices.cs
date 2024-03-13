using AutoMapper;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.VariantTypes;
using ExagenSharedProject;
using Repositories;
using Repositories.Models;
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

        public async Task<bool> BuyQuestionSet(BuyQuestionSet buyQuestionSet, Guid currentUser)
        {
            try 
            {
                var share = await _unitOfWork.ShareRepo.FindByField(s => s.QuestionSetId == buyQuestionSet.ShareId);
                if (share == null)
                {
                    return false;
                }
                var questions = await _unitOfWork.QuestionRepo.FindListByField(q => q.QuestionSetId == share.QuestionSetId);

                var setConfig = new SetConfig
                {
                    NB = questions.Count(q => q.Difficulty == 0),
                    TH = questions.Count(q => q.Difficulty == 1),
                    VDT = questions.Count(q => q.Difficulty == 2),
                    VDC = questions.Count(q => q.Difficulty == 3)
                };
                int price = (setConfig.NB * 200 + setConfig.TH * 500 + setConfig.VDT * 1000 + setConfig.VDC * 3000) / 5;
                
                var creator = await _unitOfWork.UserRepo.FindByField(u => u.UserId == share.CreatedBy);
                var currentUserInfo = await _unitOfWork.UserRepo.FindByField(u => u.UserId == currentUser);
                if (currentUserInfo.Point < price)
                {
                    throw new Exception("Không đủ điểm");
                }

                // them share
                var shareNew = new Share
                {
                    QuestionSetId = share.QuestionSetId,
                    UserId = currentUser,
                    Type = 0,
                    Status = 1,
                    CreatedBy = currentUser,
                    CreatedOn = DateTime.Now,
                    ModifiedBy = currentUser,
                    ModifiedOn = DateTime.Now
                };
                _unitOfWork.ShareRepo.AddAsync(share);

                // tru tien
                currentUserInfo.Point = currentUserInfo.Point - price;
                currentUserInfo.ModifiedOn = DateTime.Now;

                creator.Point = creator.Point + price;
                creator.ModifiedOn = DateTime.Now;

                _unitOfWork.UserRepo.Update(currentUserInfo);
                _unitOfWork.UserRepo.Update(creator);

                // them transaction
                List<Transaction> transactions = new List<Transaction>()
                {
                    new Transaction
                    {
                        UserId = currentUser,
                        Type = 3,
                        PointValue = -price,
                        CreatedOn = DateTime.Now,
                    },  
                    new Transaction
                    {
                        UserId = share.CreatedBy,
                        Type = 4,
                        PointValue = price,
                        CreatedOn = DateTime.Now,
                    },
                };

                _unitOfWork.TransactionRepo.AddRangeAsync(transactions);

                var result = await _unitOfWork.SaveChangesAsync();

                return result > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi ở ShareServices - BuyQuestionSet: " + ex.Message);
            }
        }

        public async Task<List<ShareInMarket>> GetBoughtList(Guid currentUser, int? grade, int? subjectEnum, int year)
        {
            try
            {
                var shares = await _unitOfWork.ShareRepo.FindListByField(share => share.CreatedOn.Year == year && share.Type == 0 && share.CreatedBy != currentUser && share.UserId == currentUser, includes => includes.QuestionSet);

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

                if (shares == null)
                {
                    return null;
                }

                var result = new List<ShareInMarket>();

                foreach (var s in shares)
                {
                    var config = new SetConfig
                    {
                        NB = s.QuestionSet.Questions.Count(c => c.Difficulty == 0),
                        TH = s.QuestionSet.Questions.Count(c => c.Difficulty == 1),
                        VDT = s.QuestionSet.Questions.Count(c => c.Difficulty == 2),
                        VDC = s.QuestionSet.Questions.Count(c => c.Difficulty == 3),
                    };

                    var shareInMarket = _mapper.Map<ShareInMarket>(s);

                    shareInMarket.Price = (config.NB * 200 + config.TH * 500 + config.VDT * 1000 + config.VDC * 3000) / 5;
                    result.Add(shareInMarket);
                }

                return result;

            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi ở ShareServices - GetBoughtList: " + ex.Message);
            }
        }

        public async Task<List<MySold>> GetSoldList(Guid currentUser, int? grade, int? subjectEnum, int? status, int year)
        {
            try
            {
                var shares = await _unitOfWork.ShareRepo.FindListByField(share => share.CreatedOn.Year == year && share.Type == 0 && share.CreatedBy == currentUser && 
                                                                                    share.User == null, includes => includes.QuestionSet);

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

                var result = new List<MySold>();

                foreach (var s in shares)
                {
                    var config = new SetConfig
                    {
                        NB = s.QuestionSet.Questions.Count(c => c.Difficulty == 0),
                        TH = s.QuestionSet.Questions.Count(c => c.Difficulty == 1),
                        VDT = s.QuestionSet.Questions.Count(c => c.Difficulty == 2),
                        VDC = s.QuestionSet.Questions.Count(c => c.Difficulty == 3),
                    };

                    var shareInMarket = _mapper.Map<MySold>(s);

                    shareInMarket.CountSold = s.Status == 1 ? (await _unitOfWork.ShareRepo.FindListByField(share => share.QuestionSetId == s.QuestionSetId && share.UserId != null)).Count : null;
                    shareInMarket.Price = (config.NB * 200 + config.TH * 500 + config.VDT * 1000 + config.VDC * 3000) / 5;
                    result.Add(shareInMarket);
                }

                return result;

            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi ở ShareServices - GetBoughtList: " + ex.Message);
            }
        }

        public async Task<IEnumerable<ShareInMarket>> GetQuestionSetInMarket(int? grade, int? subjectEnum, int year, Guid currentUserId)
        {
            try
            {
                var shares = await _unitOfWork.ShareRepo.FindListByField(share => share.CreatedOn.Year == year && share.Type == 0 && share.Status == 1 && 
                                                                                share.CreatedBy != currentUserId && share.UserId != currentUserId, includes => includes.QuestionSet);
            
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

                if (shares == null)
                {
                    return null;
                }

                var result = new List<ShareInMarket>();

                foreach (var s in shares)
                {
                    var config = new SetConfig
                    {
                        NB = s.QuestionSet.Questions.Count(c => c.Difficulty == 0),
                        TH = s.QuestionSet.Questions.Count(c => c.Difficulty == 1),
                        VDT = s.QuestionSet.Questions.Count(c => c.Difficulty == 2),
                        VDC = s.QuestionSet.Questions.Count(c => c.Difficulty == 3),
                    };

                    var shareInMarket = _mapper.Map<ShareInMarket>(s);

                    shareInMarket.Price = (config.NB * 200 + config.TH * 500 + config.VDT * 1000 + config.VDC * 3000) / 5;
                    result.Add(shareInMarket);
                }

                return result;

            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi ở ShareServices - GetQuestionSetInMarket: " + ex.Message);
            }
        }

        public async Task<IEnumerable<ShareViewModels>> GetRequestToShare(int? status, int? grade, int? subjectEnum, int? type, int year)
        {
            try
            {
                var shares = await _unitOfWork.ShareRepo.FindListByField(share => share.CreatedOn.Year == year, includes => includes.QuestionSet);
                
                if (type != null)
                {
                    shares = shares.Where(share => share.Type == type).ToList();
                }
                else if (type == 1)
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

                var result = new List<ShareViewModels>();

                foreach (var s in shares)
                {
                    var config = new SetConfig
                    {
                        NB = s.QuestionSet.Questions.Count(c => c.Difficulty == 0),
                        TH = s.QuestionSet.Questions.Count(c => c.Difficulty == 1),
                        VDT = s.QuestionSet.Questions.Count(c => c.Difficulty == 2),
                        VDC = s.QuestionSet.Questions.Count(c => c.Difficulty == 3),
                    };
                    var shareViewModel = _mapper.Map<ShareViewModels>(s);
                    shareViewModel.Price = s.Type == 0 ? (config.NB * 200 + config.TH * 500 + config.VDT *1000 + config.VDC * 3000) / 5 : null;
                    shareViewModel.NameOfQuestionSet = s.QuestionSet.Name;
                    shareViewModel.NameOfSubject = (await _unitOfWork.SubjectRepo.FindByField(subject => subject.SubjectId == s.QuestionSet.SubjectId)).Name;

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

        public async Task<bool> RequestToShare(ShareCreateRequest shareCreate, Guid currentUser)
        {
            try
            {
                var share = new Share
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

        public async Task<bool> ShareIndividual(ShareCreateForIndividual shareIndividual, Guid currentUser)
        {
            try
            {
                var userID = await _unitOfWork.UserRepo.FindListByField(u => shareIndividual.Email.Contains(u.Email));
                if (userID == null)
                {
                    return false;
                }

                var shareResult = new List<Share>();
                foreach (var user in userID)
                {
                    var share = new Share
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
            catch (Exception ex)
            {
                throw new Exception("Lỗi ở ShareServices - ShareIndividual: " + ex.Message);
            }
        }
    }
}
