using AutoMapper;
using ExagenSharedProject;
using Repositories;
using Repositories.Models;
using Services.Interfaces.QuestionSet;
using Services.ViewModels;

namespace Services.Services.QuestionSet
{
    public class QuestionSetServices : IQuestionSetServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public QuestionSetServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> SaveQuestionSet(QuestionSetSave questionSetViewModel, Guid currentUserId)
        {
            try
            {
                var currentUser = await _unitOfWork.UserRepo.FindByField(user => user.UserId == currentUserId);

                // add question
                var questions = _mapper.Map<List<Repositories.Models.Question>>(questionSetViewModel.Questions);
                foreach (var question in questions)
                {
                    question.Grade = questionSetViewModel.Grade;
                    question.SubjectId = questionSetViewModel.SubjectId;

                }

                // add question set
                var questionSet = _mapper.Map<Repositories.Models.QuestionSet>(questionSetViewModel);
                questionSet.Status = currentUser.UserType == 2 ? 2 : 1;

                questionSet.NumOfQuestion = questionSetViewModel.Questions.Count;
                questionSet.Questions = questions;

                questionSet.CreatedBy = currentUserId;
                questionSet.CreatedOn = DateTime.Now.AddHours(7);
                questionSet.ModifiedBy = currentUserId;
                questionSet.ModifiedOn = DateTime.Now.AddHours(7);

                _unitOfWork.QuestionSetRepo.AddAsync(questionSet);
                var result = await _unitOfWork.SaveChangesAsync();

                return result > 0 ? true : false;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở QuestionSetServices - SaveQuestionSet: " + e.Message);
            }
        }

        public async Task<IEnumerable<SectionUse>> GetMatrixOfQuestionSet(Guid questionSetId)
        {
            try
            {
                var questions = await _unitOfWork.QuestionRepo.FindListByField(question => question.QuestionSetId == questionSetId, include => include.Section);
                var sectionUses = questions.GroupBy(question => new { question.Section.SectionId, question.Difficulty }).Select(group => new SectionUse
                {
                    SectionId = group.Key.SectionId,
                    CHCN = group.Count(),
                    NHD = 0,
                    Use = group.Count()
                });

                return sectionUses;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở QuestionSetServices - GetMatrixOfQuestionSet: " + e.Message);
            }
        }

        public async Task<IEnumerable<OwnQuestionSet>> GetOwnQuestionSet(Guid currentUserId, int? grade, int? subject, string studyYear)
        {
            try
            {
                var boughtQuestionSets = await _unitOfWork.ShareRepo.FindListByField(share => share.UserId == currentUserId);
                //Merge questionSets and boughtQuestionSets by ID of questionSet in boughtQuestionSets
                var questionSetIds = boughtQuestionSets.Select(boughtQuestionSet => boughtQuestionSet.QuestionSetId).ToList();
                var questionSets = await _unitOfWork.QuestionSetRepo.FindListByField(questionSet => (questionSetIds.Contains(questionSet.QuestionSetId) || questionSet.CreatedBy == currentUserId) && questionSet.StudyYear == studyYear, include => include.Subject);
                
                if (grade != null)
                {
                    questionSets = questionSets.Where(questionSet => questionSet.Grade == grade).ToList();
                }
                if (subject != null)
                {
                    questionSets = questionSets.Where(questionSet => EnumStatus.Subject[(int)subject].ToLower().Contains(questionSet.Subject.Name.ToLower())).ToList();
                }

                if (questionSets == null)
                {
                    return null;
                }

                var questionSetViewModels = new List<OwnQuestionSet>();

                foreach (var questionSet in questionSets)
                {
                    var questionSetViewModel = _mapper.Map<OwnQuestionSet>(questionSet);

                    if (subject == null)
                    {
                        var subjectName = (await _unitOfWork.SubjectRepo.FindByField(sub => sub.SubjectId == questionSet.SubjectId)).Name;
                        questionSetViewModel.SubjectEnum = EnumStatus.Subject.FirstOrDefault(s => s.Value.ToLower().Contains(subjectName.ToLower())).Key;
                    }
                    else
                    {
                        questionSetViewModel.SubjectEnum = (int)subject;
                    }

                    // Set the Type property
                    var boughtQuestionSet = boughtQuestionSets.FirstOrDefault(s => s.QuestionSetId == questionSet.QuestionSetId);
                    if (boughtQuestionSet != null) //If the question set is bought, set the Type property to -1 if it is a shared question set, 0 if it is a bought question set
                    {
                        if (boughtQuestionSet.Type == 1)
                        {
                            questionSetViewModel.Type = 2;
                            var createdByUser = await _unitOfWork.UserRepo.FindByField(user => user.UserId == boughtQuestionSet.CreatedBy);
                            questionSetViewModel.NameOfOwner = createdByUser != null ? createdByUser.FullName : null;
                        }
                        else
                        {
                            questionSetViewModel.Type = 0;
                            questionSetViewModel.NameOfOwner = null;
                        }
                    }
                    else //If the question set is not bought, set the Type property to 1
                    {
                        questionSetViewModel.Type = 1;
                    }

                    questionSetViewModels.Add(questionSetViewModel);
                }

                return questionSetViewModels;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở QuestionSetServices - GetOwnQuestionSet: " + e.Message);
            }
        }

        public async Task<IEnumerable<SharedQuestionSet>> GetSharedQuestionSet(Guid currentUserId, int? grade, int? subjectEnum, string studyYear)
        {
            try
            {
                var sharedQuestionSetIds = (await _unitOfWork.ShareRepo.FindListByField(share => (share.UserId == currentUserId && share.Type == 1 && share.Status == 1) || 
                                                                                                (share.Type == 2 && share.Status == 1))).Select(s => s.QuestionSetId).Distinct().ToList();
                var questionSets = await _unitOfWork.QuestionSetRepo.FindListByField(questionSet => sharedQuestionSetIds.Contains(questionSet.QuestionSetId) && questionSet.StudyYear == studyYear, include => include.Subject);

                if (grade != null)
                {
                    questionSets = questionSets.Where(questionSet => questionSet.Grade == grade).ToList();
                }
                if (subjectEnum != null)
                {
                    questionSets = questionSets.Where(questionSet => EnumStatus.Subject[(int)subjectEnum].ToLower().Contains(questionSet.Subject.Name.ToLower())).ToList();
                }
                var questionSetViewModels = questionSets != null ? _mapper.Map<IEnumerable<SharedQuestionSet>>(questionSets.OrderByDescending(o => o.CreatedBy)) : null;
                return questionSetViewModels;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở QuestionSetServices - GetSharedQuestionSet: " + e.Message);
            }
        }

        public async Task<QuestionSetViewModel> GetQuestionByQuestionSetId(Guid questionSetId)
        {
            try
            {
                var questionSet = await _unitOfWork.QuestionSetRepo.FindByField(questionSet => questionSet.QuestionSetId == questionSetId, include => include.Subject);
                if (questionSet == null)
                {
                    throw new Exception("Không tìm thấy bộ câu hỏi");
                }

                var questionSetViewModel = _mapper.Map<QuestionSetViewModel>(questionSet);
                questionSetViewModel.SubjectName = questionSet.Subject.Name;

                var questions = await _unitOfWork.QuestionRepo.FindListByField(
                question => question.QuestionSetId == questionSetId,
                include => include.Section);

                questionSetViewModel.Questions = _mapper.Map<List<QuestionViewModel>>(questions).OrderBy(o => o.QuestionPart).ToList();

                var setConfig = new SetConfig { NB = questions.Count(q => q.Difficulty == OptionSet.Question.Difficulty.NB), TH = questions.Count(q => q.Difficulty == OptionSet.Question.Difficulty.TH), VDT = questions.Count(q => q.Difficulty == OptionSet.Question.Difficulty.VD), VDC = questions.Count(q => q.Difficulty == OptionSet.Question.Difficulty.VDC) };

                questionSetViewModel.Price = (setConfig.NB * 2 + setConfig.TH * 5 + setConfig.VDT * 10 + setConfig.VDC * 30)/5;

                return questionSetViewModel;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở QuestionSetServices - GetQuestionByQuestionSetId: " + e.Message);
            }
        }

        public async Task<IEnumerable<QuestionSetViewModels>> GetQuestionSetBank(int? grade, int? subject, string studyYear, int type)
        {
            var resutl = new List<QuestionSetViewModels>();
            if (type == 0)
            {
                var shares = await _unitOfWork.ShareRepo.FindListByField(share => share.Type == 0 && share.Status == 1);
                var distinctQuestionSetIds = shares.Select(s => s.QuestionSetId).Distinct().ToList();

                var questionSets = await _unitOfWork.QuestionSetRepo.FindListByField(questionset => distinctQuestionSetIds.Contains(questionset.QuestionSetId)
                                                                                            && questionset.StudyYear == studyYear, includes => includes.Subject, includes => includes.Questions);
                if (grade != null)
                {
                    questionSets = questionSets.Where(questionSet => questionSet.Grade == grade).ToList();
                }
                if (subject != null)
                {
                    questionSets = questionSets.Where(questionSet => EnumStatus.Subject[(int)subject].ToLower().Contains(questionSet.Subject.Name.ToLower())).ToList();
                }
                foreach (var qs in questionSets)
                {
                    var config = new SetConfig
                    {
                        NB = qs.Questions.Count(q => q.Difficulty == OptionSet.Question.Difficulty.NB),
                        TH = qs.Questions.Count(q => q.Difficulty == OptionSet.Question.Difficulty.TH),
                        VDT = qs.Questions.Count(q => q.Difficulty == OptionSet.Question.Difficulty.VD),
                        VDC = qs.Questions.Count(q => q.Difficulty == OptionSet.Question.Difficulty.VDC)
                    };
                    var qsvm = _mapper.Map<QuestionSetViewModels>(qs);
                    qsvm.Type = 0;
                    qsvm.Price = (config.NB * 2 + config.TH * 5 + config.VDT * 10 + config.VDC * 30) / 5; // 1/5 giá gốc nếu là type 0
                    resutl.Add(qsvm);
                }
            }
            else if (type == 2)
            {
                var shares = await _unitOfWork.ShareRepo.FindListByField(share => share.Type == 2 && share.Status == 1);

                var distinctQuestionSetIds = shares.Select(s => s.QuestionSetId).Distinct().ToList();
                var questionSets = await _unitOfWork.QuestionSetRepo.FindListByField(questionset => (distinctQuestionSetIds.Contains(questionset.QuestionSetId) || questionset.Status == 2) && questionset.StudyYear == studyYear, include => include.Subject);
                if (grade != null)
                {
                    questionSets = questionSets.Where(questionSet => questionSet.Grade == grade).ToList();
                }
                if (subject != null)
                {
                    questionSets = questionSets.Where(questionSet => EnumStatus.Subject[(int)subject].ToLower().Contains(questionSet.Subject.Name.ToLower())).ToList();
                }
                foreach (var qs in questionSets)
                {
                    var config = new SetConfig
                    {
                        NB = qs.Questions.Count(q => q.Difficulty == OptionSet.Question.Difficulty.NB),
                        TH = qs.Questions.Count(q => q.Difficulty == OptionSet.Question.Difficulty.TH),
                        VDT = qs.Questions.Count(q => q.Difficulty == OptionSet.Question.Difficulty.VD),
                        VDC = qs.Questions.Count(q => q.Difficulty == OptionSet.Question.Difficulty.VDC)
                    };
                    var qsvm = _mapper.Map<QuestionSetViewModels>(qs);
                    qsvm.Type = 2;
                    qsvm.Price = null; // free
                    resutl.Add(qsvm);
                }
            }

            return resutl;
        }

        public async Task<QuestionReturn> GetQuestionSetFromFile(ImportQuestionSet importQuestionSet)
        {
            try
            {
                var questionSet = _mapper.Map<QuestionReturn>(importQuestionSet);
                questionSet.SubjectId = (await _unitOfWork.SubjectRepo.FindByField(subject => EnumStatus.Subject[(int)importQuestionSet.Subject].ToLower().Contains(subject.Name.ToLower()) && subject.Grade == importQuestionSet.Grade)).SubjectId;
                questionSet.Questions = DucumentProcessing.ImportQuestionSetUtils.ImportQuestions(importQuestionSet.File).OrderBy(o => o.QuestionPart).ToList();

                return questionSet;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở QuestionSetServices - GetQuestionSetFromFile: " + e.Message);
            }
        }

        public async Task<bool> ChangeStatusQuestionSet(Guid questionSetId, bool isActive, Guid currentUser)
        {
            try
            {
                var questionSet = await _unitOfWork.QuestionSetRepo.FindByField(questionSet => questionSet.QuestionSetId == questionSetId && questionSet.CreatedBy == currentUser);
                if (questionSet == null)
                {
                    return false;
                }
                questionSet.Status = isActive ? 1 : 0;
                questionSet.ModifiedBy = currentUser;
                questionSet.ModifiedOn = DateTime.Now.AddHours(7);
                _unitOfWork.QuestionSetRepo.Update(questionSet);
                var result = await _unitOfWork.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở QuestionSetServices - ChangeStatusQuestionSet: " + e.Message);
            }
        }

        public async Task<bool> DeleteQuestionSet(Guid questionSetId, Guid currentUser)
        {
            try
            {
                var questionSet = await _unitOfWork.QuestionSetRepo.FindByField(questionSet => questionSet.QuestionSetId == questionSetId && questionSet.CreatedBy == currentUser);
                if (questionSet == null)
                {
                    throw new Exception("Không tìm thấy bộ câu hỏi này hoặc bản không phải chủ sở hữu");
                }

                var questions = await _unitOfWork.QuestionRepo.FindListByField(question => question.QuestionSetId == questionSet.QuestionSetId, includes => includes.QuestionInExams, include => include.QuestionInPapers);
                var shares = await _unitOfWork.ShareRepo.FindListByField(share => share.QuestionSetId == questionSetId && share.Type == 0);
                if (questions.Any(question => question.QuestionInExams.Count > 0) || questions.Any(questions => questions.QuestionInPapers.Count > 0) || shares.Count > 0)
                {
                    throw new Exception("Nội dung này đã được sử dụng, không thể xóa");
                }

                _unitOfWork.QuestionRepo.RemoveRange(questions);
                _unitOfWork.QuestionSetRepo.Remove(questionSet);
                var result = await _unitOfWork.SaveChangesAsync();

                return result > 0;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở QuestionSetServices - DeleteQuestionSet: " + e.Message);
            }

            throw new NotImplementedException();
        }

    }
}
