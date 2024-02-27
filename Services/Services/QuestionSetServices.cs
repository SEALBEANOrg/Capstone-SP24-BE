using AutoMapper;
using Repositories;
using Repositories.Models;
using Services.Interfaces;
using Services.ViewModels;
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

        public Task<bool> ChangeStatusQuestionSet(Guid questionSetId, bool isActive, Guid currentUser)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteQuestionSet(Guid questionSetId, Guid currentUser)
        {
            try
            {
                var questionSet = await _unitOfWork.QuestionSetRepo.FindByField(questionSet => questionSet.QuestionSetId == questionSetId);
                if (questionSet == null || questionSet.CreatedBy != currentUser)
                {
                    return false;
                }
                var questionMappings = await _unitOfWork.QuestionMappingRepo.FindListByField(questionMapping => questionMapping.QuestionSetId == questionSetId);
                
                //foreach (var questionMapping in questionMappings)
                //{
                //}

                _unitOfWork.QuestionMappingRepo.RemoveRange(questionMappings);

                _unitOfWork.QuestionSetRepo.Remove(questionSet);
                var result = await _unitOfWork.SaveChangesAsync();

                return result > 0;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở QuestionSetServices - DeleteQuestionSet: " + e.Message);
            }
        }

        public async Task<IEnumerable<OwnQuestionSet>> GetOwnQuestionSet(Guid currentUserId, int? grade, int? subject, int year)
        {
            try
            {
                var questionSets = await _unitOfWork.QuestionSetRepo.FindListByField(questionSet => questionSet.CreatedBy == currentUserId && questionSet.Status == 1 && questionSet.CreatedOn.Year == year);
                if (grade != null)
                {
                    questionSets = questionSets.Where(questionSet => questionSet.Grade == grade).ToList();
                }
                if (subject != null)
                {
                    questionSets = questionSets.Where(questionSet => questionSet.Subject == subject).ToList();
                }
                var questionSetViewModels = questionSets != null ? _mapper.Map<IEnumerable<OwnQuestionSet>>(questionSets) : null;
                return questionSetViewModels;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở QuestionSetServices - GetOwnQuestionSet: " + e.Message);
            }
        }

        public async Task<QuestionSetViewModel> GetQuestionByQuestionSetId (Guid questionSetId)
        {
            try
            {
                var questionSet = await _unitOfWork.QuestionSetRepo.FindByField(questionSet => questionSet.QuestionSetId == questionSetId);
                if (questionSet == null)
                {
                    throw new Exception("Không tìm thấy bộ đề");
                }

                var questionSetViewModel = _mapper.Map<QuestionSetViewModel>(questionSet);
                
                var questionIds = (await _unitOfWork.QuestionMappingRepo.FindListByField(questionMapping => questionMapping.QuestionSetId == questionSetId)).Select(question => question.QuestionId);
                var questions = await _unitOfWork.QuestionRepo.FindListByField(question => questionIds.Contains(question.QuestionId));
                questionSetViewModel.Questions = _mapper.Map<List<QuestionViewModel>>(questions);

                var setConfig = JsonSerializer.Deserialize<SetConfig>(questionSet.SetConfig);
                questionSetViewModel.Price = setConfig.NB * 200 + setConfig.TH * 500 + setConfig.VDT * 1000 + setConfig.VDC * 3000;

                return questionSetViewModel;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở QuestionSetServices - GetQuestionByQuestionSetId: " + e.Message);
            }
        }

        public async Task<QuestionReturn> GetQuestionSetFromFile(ImportQuestionSet importQuestionSet)
        {
            try
            {
                var questionSet = _mapper.Map<QuestionReturn>(importQuestionSet);
                
                questionSet.Questions = DucumentProcessing.ImportQuestionSet.ImportQuestions(importQuestionSet.File);

                return questionSet;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở QuestionSetServices - GetQuestionSetFromFile: " + e.Message);
            }
        }

        public async Task<bool> SaveQuestionSet(QuestionSetSave questionSetViewModel, Guid currentUserId)
        { // chua xong... question co section 
            try
            {
                var currentUser = await _unitOfWork.UserRepo.FindByField(user => user.UserId == currentUserId);
                
                int nb = questionSetViewModel.Questions.Count(question => question.Difficulty == 0);
                int th = questionSetViewModel.Questions.Count(question => question.Difficulty == 1);
                int vdt = questionSetViewModel.Questions.Count(question => question.Difficulty == 2);
                int vdc = questionSetViewModel.Questions.Count(question => question.Difficulty == 3);

                // add question set
                var questionSet = _mapper.Map<QuestionSet>(questionSetViewModel);
                questionSet.Status = 1;
                questionSet.SetConfig = JsonSerializer.Serialize(new SetConfig
                {
                    NB = nb,
                    TH = th,
                    VDT = vdt,
                    VDC = vdc
                });
                questionSet.NumOfQuestion = questionSetViewModel.Questions.Count;
                if (currentUser.UserType == 2)
                {
                    questionSet.SchoolId = currentUser.SchoolId;  // nếu là schooladmin thì sẽ có schoolid trong bộ câu hỏi và câu hỏi
                }
                questionSet.CreatedBy = currentUserId;
                questionSet.CreatedOn = DateTime.Now;
                questionSet.ModifiedBy = currentUserId;
                questionSet.ModifiedOn = DateTime.Now;
                
                _unitOfWork.QuestionSetRepo.AddAsync(questionSet);
                var result = await _unitOfWork.SaveChangesAsync();
                if (result <= 0)
                {
                    return false;
                }

                // add question
                var questions = _mapper.Map<List<Question>>(questionSetViewModel.Questions);
                foreach (var question in questions)
                {
                    question.Grade = questionSet.Grade;
                    question.Subject = questionSet.Subject;
                    if (currentUser.UserType == 2)
                    {
                        question.SchoolId = currentUser.SchoolId;
                    }
                    question.CreatedBy = currentUserId;
                    question.CreatedOn = DateTime.Now;
                    question.ModifiedBy = currentUserId;
                    question.ModifiedOn = DateTime.Now;
                    if (currentUser.UserType == 3) // nếu là expert thì public cho all
                    {
                        question.IsPublic = true;
                    }
                    else
                    {
                        question.IsPublic = false;
                    }
                    question.Status = 1;
                }
                _unitOfWork.QuestionRepo.AddRangeAsync(questions);
                result = await _unitOfWork.SaveChangesAsync();
                if (result <= 0)
                {
                    return false;
                }

                // add question mapping
                var questionMappings = new List<QuestionMapping>();
                foreach (var question in questions)
                {
                    questionMappings.Add(new QuestionMapping
                    {
                        QuestionId = question.QuestionId,
                        QuestionSetId = questionSet.QuestionSetId
                    });
                }
                _unitOfWork.QuestionMappingRepo.AddRangeAsync(questionMappings);
                result = await _unitOfWork.SaveChangesAsync();

                return result > 0 ? true : false;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở QuestionSetServices - SaveQuestionSet: " + e.Message);
            }
        }
    }
}
