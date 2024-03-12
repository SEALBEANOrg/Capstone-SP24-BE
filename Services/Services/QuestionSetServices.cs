using AutoMapper;
using DocumentFormat.OpenXml.VariantTypes;
using ExagenSharedProject;
using Repositories;
using Repositories.Models;
using Services.Interfaces;
using Services.ViewModels;
using Spire.Doc.Fields;
using System.Text.Json;

namespace Services.Services
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
                questionSet.ModifiedOn = DateTime.Now;
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
                    return false;
                }

                var questions = await _unitOfWork.QuestionRepo.FindListByField(question => question.QuestionSetId == questionSet.QuestionSetId, includes => includes.QuestionInExams, include => include.QuestionInPapers);
                var shares = await _unitOfWork.ShareRepo.FindListByField(share => share.QuestionSetId == questionSetId && (share.Type == 0));
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

        public async Task<IEnumerable<OwnQuestionSet>> GetOwnQuestionSet(Guid currentUserId, int? grade, int? subject, int year)
        {
            try
            {
                var questionSets = await _unitOfWork.QuestionSetRepo.FindListByField(questionSet => questionSet.CreatedBy == currentUserId && questionSet.CreatedOn.Year == year, include => include.Subject);
                if (grade != null)
                {
                    questionSets = questionSets.Where(questionSet => questionSet.Grade == grade).ToList();
                }
                if (subject != null)
                {
                    questionSets = questionSets.Where(questionSet => EnumStatus.Subject[(int)subject].ToLower().Contains(questionSet.Subject.Name.ToLower())).ToList();
                }
                var questionSetViewModels = questionSets != null ? _mapper.Map<IEnumerable<OwnQuestionSet>>(questionSets) : null;
                return questionSetViewModels;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở QuestionSetServices - GetOwnQuestionSet: " + e.Message);
            }

            throw new NotImplementedException();
        }

        public async Task<QuestionSetViewModel> GetQuestionByQuestionSetId (Guid questionSetId)
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

                var questions = await _unitOfWork.QuestionRepo.FindListByField(question => question.QuestionSetId == questionSetId);
                questionSetViewModel.Questions = _mapper.Map<List<QuestionViewModel>>(questions);

                var setConfig = new SetConfig { NB = questions.Count(q => q.Difficulty == 0), TH = questions.Count(q => q.Difficulty == 1), VDT = questions.Count(q => q.Difficulty == 2), VDC = questions.Count(q => q.Difficulty == 3) };

                questionSetViewModel.Price = setConfig.NB * 200 + setConfig.TH * 500 + setConfig.VDT * 1000 + setConfig.VDC * 3000;

                return questionSetViewModel;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở QuestionSetServices - GetQuestionByQuestionSetId: " + e.Message);
            }

            throw new NotImplementedException();
        }

        public async Task<IEnumerable<QuestionSetViewModels>> GetQuestionSetBank(int? grade, int? subject, int year, int type)
        {
            var resutl = new List<QuestionSetViewModels>();
            if (type == 0)
            {
                var shares = await _unitOfWork.ShareRepo.FindListByField(share => share.Type == 0 && share.Status == 1);
                var questionSets = await _unitOfWork.QuestionSetRepo.FindListByField(questionset => shares.Select(s => s.QuestionSetId).Distinct().Contains(questionset.QuestionSetId)
                                                                                            && questionset.CreatedOn.Year == year, includes => includes.Subject, includes => includes.Questions);
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
                        NB = qs.Questions.Count(q => q.Difficulty == 0),
                        TH = qs.Questions.Count(q => q.Difficulty == 1),
                        VDT = qs.Questions.Count(q => q.Difficulty == 2),
                        VDC = qs.Questions.Count(q => q.Difficulty == 3)
                    };
                    var qsvm = _mapper.Map<QuestionSetViewModels>(qs);
                    qsvm.Type = 0;
                    qsvm.Price = (config.NB * 200 + config.TH * 500 + config.VDT * 1000 + config.VDC * 3000) / 5; // 1/5 giá gốc nếu là type 0
                    resutl.Add(qsvm);
                }
            }
            else if (type == 2)
            {
                var shares = await _unitOfWork.ShareRepo.FindListByField(share => share.Type == 2 && share.Status == 1);
                var questionSets = await _unitOfWork.QuestionSetRepo.FindListByField(questionset => (shares.Select(s => s.QuestionSetId).Distinct().Contains(questionset.QuestionSetId) || questionset.Status == 2) && questionset.CreatedOn.Year == year, include => include.Subject);
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
                        NB = qs.Questions.Count(q => q.Difficulty == 0),
                        TH = qs.Questions.Count(q => q.Difficulty == 1),
                        VDT = qs.Questions.Count(q => q.Difficulty == 2),
                        VDC = qs.Questions.Count(q => q.Difficulty == 3)
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
                questionSet.Questions = DucumentProcessing.ImportQuestionSet.ImportQuestions(importQuestionSet.File);

                return questionSet;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở QuestionSetServices - GetQuestionSetFromFile: " + e.Message);
            }
        }

        public async Task<bool> SaveQuestionSet(QuestionSetSave questionSetViewModel, Guid currentUserId)
        {
            try
            {
                var currentUser = await _unitOfWork.UserRepo.FindByField(user => user.UserId == currentUserId);

                // add question
                var questions = _mapper.Map<List<Question>>(questionSetViewModel.Questions);
                foreach (var question in questions)
                {
                    question.Grade = questionSetViewModel.Grade;
                    question.SubjectId = questionSetViewModel.SubjectId;

                }

                // add question set
                var questionSet = _mapper.Map<QuestionSet>(questionSetViewModel);
                questionSet.Status = currentUser.UserType == 2 ? 2 : 1;
                
                questionSet.NumOfQuestion = questionSetViewModel.Questions.Count;
                questionSet.Questions = questions;
                
                questionSet.CreatedBy = currentUserId;
                questionSet.CreatedOn = DateTime.Now;
                questionSet.ModifiedBy = currentUserId;
                questionSet.ModifiedOn = DateTime.Now;

                _unitOfWork.QuestionSetRepo.AddAsync(questionSet);
                var result = await _unitOfWork.SaveChangesAsync();

                return result > 0 ? true : false;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở QuestionSetServices - SaveQuestionSet: " + e.Message);
            }
        }



    }
}
