using AutoMapper;
using ExagenSharedProject;
using Repositories;
using Services.Interfaces.Share;
using Services.ViewModels;

namespace Services.Services.Share
{
    public class MarketServices : IMarketServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MarketServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> BuyQuestionSet(BuyQuestionSet buyQuestionSet, Guid currentUser)
        {
            try
            {
                var share = await _unitOfWork.ShareRepo.FindByField(s => s.ShareId == buyQuestionSet.ShareId);
                if (share == null)
                {
                    return false;
                }
                var questions = await _unitOfWork.QuestionRepo.FindListByField(q => q.QuestionSetId == share.QuestionSetId);

                var setConfig = new SetConfig
                {
                    NB = questions.Count(q => q.Difficulty == OptionSet.Question.Difficulty.NB),
                    TH = questions.Count(q => q.Difficulty == OptionSet.Question.Difficulty.TH),
                    VDT = questions.Count(q => q.Difficulty == OptionSet.Question.Difficulty.VD),
                    VDC = questions.Count(q => q.Difficulty == OptionSet.Question.Difficulty.VDC)
                };
                int price = (setConfig.NB * 2 + setConfig.TH * 5 + setConfig.VDT * 10 + setConfig.VDC * 30) / 5;

                var creator = await _unitOfWork.UserRepo.FindByField(u => u.UserId == share.CreatedBy);
                var currentUserInfo = await _unitOfWork.UserRepo.FindByField(u => u.UserId == currentUser);
                if (currentUserInfo.Point < price)
                {
                    throw new Exception("Không đủ điểm");
                }

                // them share
                var shareNew = new Repositories.Models.Share
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
                _unitOfWork.ShareRepo.AddAsync(shareNew);

                // tru tien
                currentUserInfo.Point = currentUserInfo.Point - price;
                currentUserInfo.ModifiedOn = DateTime.Now;

                creator.Point = creator.Point + price;
                creator.ModifiedOn = DateTime.Now;

                _unitOfWork.UserRepo.Update(currentUserInfo);
                _unitOfWork.UserRepo.Update(creator);

                // them transaction
                List<Repositories.Models.Transaction> transactions = new List<Repositories.Models.Transaction>()
                {
                    new Repositories.Models.Transaction
                    {
                        UserId = currentUser,
                        Type = 3, // mua bộ câu hỏi
                        PointValue = -price,
                        CreatedOn = DateTime.Now,
                    },
                    new Repositories.Models.Transaction
                    {
                        UserId = share.CreatedBy,
                        Type = 4, // bán bộ câu hỏi
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

        public async Task<List<ShareInMarket>> GetBoughtList(Guid currentUser, int? grade, int? subjectEnum, string studyYear)
        {
            try
            {
                var shares = await _unitOfWork.ShareRepo.FindListByField(share => share.StudyYear == studyYear && share.Type == 0 && share.UserId == currentUser, includes => includes.QuestionSet);

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
                    var questions = await _unitOfWork.QuestionRepo.FindListByField(question => question.QuestionSetId == s.QuestionSetId);
                    var config = new SetConfig
                    {
                        NB = questions.Count(c => c.Difficulty == OptionSet.Question.Difficulty.NB),
                        TH = questions.Count(c => c.Difficulty == OptionSet.Question.Difficulty.TH),
                        VDT = questions.Count(c => c.Difficulty == OptionSet.Question.Difficulty.VD),
                        VDC = questions.Count(c => c.Difficulty == OptionSet.Question.Difficulty.VDC),
                    };

                    var shareInMarket = _mapper.Map<ShareInMarket>(s);

                    shareInMarket.Price = (config.NB * 2 + config.TH * 5 + config.VDT * 10 + config.VDC * 30) / 5;
                    result.Add(shareInMarket);
                }

                return result;

            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi ở ShareServices - GetBoughtList: " + ex.Message);
            }
        }

        public async Task<List<MySold>> GetSoldList(Guid currentUser, int? grade, int? subjectEnum, int? status, string studyYear)
        {
            try
            {
                var shares = await _unitOfWork.ShareRepo.FindListByField(share => share.StudyYear == studyYear && share.Type == 0 && share.CreatedBy == currentUser &&
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
                    var questions = await _unitOfWork.QuestionRepo.FindListByField(question => question.QuestionSetId == s.QuestionSetId);
                    var config = new SetConfig
                    {
                        NB = questions.Count(c => c.Difficulty == OptionSet.Question.Difficulty.NB),
                        TH = questions.Count(c => c.Difficulty == OptionSet.Question.Difficulty.TH),
                        VDT = questions.Count(c => c.Difficulty == OptionSet.Question.Difficulty.VD),
                        VDC = questions.Count(c => c.Difficulty == OptionSet.Question.Difficulty.VDC),
                    };

                    var shareInMarket = _mapper.Map<MySold>(s);

                    shareInMarket.CountSold = s.Status == 1 ? (await _unitOfWork.ShareRepo.FindListByField(share => share.QuestionSetId == s.QuestionSetId && share.UserId != null)).Count : null;
                    shareInMarket.Price = (config.NB * 2 + config.TH * 5 + config.VDT * 10 + config.VDC * 30) / 5;
                    result.Add(shareInMarket);
                }

                return result;

            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi ở ShareServices - GetBoughtList: " + ex.Message);
            }
        }

        public async Task<IEnumerable<ShareInMarket>> GetQuestionSetInMarket(int? grade, int? subjectEnum, string studyYear, Guid currentUserId)
        {
            try
            {
                var boughtQuestionSet = await _unitOfWork.ShareRepo.FindListByField(share => share.UserId == currentUserId && share.Type == 0);
                var shares = await _unitOfWork.ShareRepo.FindListByField(share => share.StudyYear == studyYear && share.Type == 0 && share.Status == 1 &&
                                                                                share.CreatedBy != currentUserId && share.UserId == null, includes => includes.QuestionSet);

                if (boughtQuestionSet.Count > 0)
                {
                    shares = shares.Where(share => !boughtQuestionSet.Select(s => s.QuestionSetId).Contains(share.QuestionSetId)).ToList();
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

                if (shares == null)
                {
                    return null;
                }

                // Fetch all users at once
                var users = await _unitOfWork.UserRepo.GetAllAsync();

                var result = new List<ShareInMarket>();

                foreach (var s in shares)
                {
                    var questions = await _unitOfWork.QuestionRepo.FindListByField(question => question.QuestionSetId == s.QuestionSetId);
                    var config = new SetConfig
                    {
                        NB = questions.Count(c => c.Difficulty == OptionSet.Question.Difficulty.NB),
                        TH = questions.Count(c => c.Difficulty == OptionSet.Question.Difficulty.TH),
                        VDT = questions.Count(c => c.Difficulty == OptionSet.Question.Difficulty.VD),
                        VDC = questions.Count(c => c.Difficulty == OptionSet.Question.Difficulty.VDC),
                    };

                    var shareInMarket = _mapper.Map<ShareInMarket>(s);
                    shareInMarket.NameOfSeller = users.First(u => u.UserId == s.CreatedBy).FullName;

                    shareInMarket.Price = (config.NB * 2 + config.TH * 5 + config.VDT * 10 + config.VDC * 30) / 5;
                    result.Add(shareInMarket);
                }

                return result;

            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi ở ShareServices - GetQuestionSetInMarket: " + ex.Message);
            }
        }

    }
}
