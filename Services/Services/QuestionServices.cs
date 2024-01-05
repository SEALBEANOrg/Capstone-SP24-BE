using AutoMapper;
using Repositories;
using Repositories.Models;
using Services.Interfaces;
using Services.ViewModels;
using System.Text.Json;

namespace Services.Services
{
    public class QuestionServices : IQuestionServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public QuestionServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> AddQuestions(QuestionCreate questionCreate, Guid currentUser)
        {
            QuestionJson questionJson = new QuestionJson
            {
                Content = questionCreate.Content,
                Answers = questionCreate.Answers,
            };

            try
            {

                if (questionCreate.SectionId != null)
                {
                    var section = await _unitOfWork.SubjectSectionRepo.FindByField(subjectSection => subjectSection.SectionId == questionCreate.SectionId);
                    if (section == null)
                    {
                        throw new Exception("Section không tồn tại");
                    }
                }

                if (questionCreate.SchoolId != null)
                {
                    var subject = await _unitOfWork.SchoolRepo.FindByField(school => school.SchoolId == questionCreate.SchoolId);
                    if (subject == null)
                    {
                        throw new Exception("School không tồn tại");
                    }
                }

                if (questionCreate.IsUseToSell < 1 || questionCreate.IsUseToSell > 2)
                {
                    throw new Exception("Trạng thái không hợp lệ");
                }

                var question = _mapper.Map<Question>(questionCreate);
                question.QuestionContent = JsonSerializer.Serialize((questionJson));
                question.CreatedOn = DateTime.Now;
                question.ModifiedOn = DateTime.Now;
                question.CreatedBy = currentUser;
                question.ModifiedBy = currentUser;
                question.Status = questionCreate.IsUseToSell;

                _unitOfWork.QuestionRepo.AddAsync(question);
                var result = await _unitOfWork.SaveChangesAsync();
                if (result <= 0)
                {
                    return false;
                }

                return true;

            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở QuestionServices - AddQuestion: " + e.Message);
            }
        }

        public async Task<bool> DeleteQuestion(Guid questionId, Guid currentUser)
        {
            try
            {
                var question = await _unitOfWork.QuestionRepo.FindByField(question => question.QuestionId == questionId);
                if (question == null)
                {
                    throw new Exception("Câu hỏi không tồn tại.");
                }

                _unitOfWork.QuestionRepo.Remove(question);

                var result = await _unitOfWork.SaveChangesAsync();

                if (result <= 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở QuestionServices - DeleteQuestion: " + e.Message);
            }
        }

        public async Task<IEnumerable<QuestionViewModels>> GetAllQuestion()
        {
            try
            {
                var questions = await _unitOfWork.QuestionRepo.GetAllAsync();
                if (questions == null)
                {
                    return null;
                }

                var questionViewModels = _mapper.Map<IEnumerable<QuestionViewModels>>(questions);
                return questionViewModels;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở QuestionServices - GetAllQuestion: " + e.Message);
            }
        }

        public async Task<QuestionViewModels> GetQuestionByQuestionId(Guid questionId)
        {
            try
            {
                var question = await _unitOfWork.QuestionRepo.FindByField(question => question.QuestionId == questionId);
                if (question == null)
                {
                    throw new Exception("Câu hỏi không tồn tại.");
                }

                var questionViewModels = _mapper.Map<QuestionViewModels>(question);
                return questionViewModels;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở QuestionServices - GetQuestionById: " + e.Message);
            }
        }

        public async Task<bool> UpdateQuestion(QuestionUpdate questionUpdate, Guid currentUser)
        {

            QuestionJson questionJson = new QuestionJson
            {
                Content = questionUpdate.Content,
                Answers = questionUpdate.Answers,
            };

            try
            {
                var question = await _unitOfWork.QuestionRepo.FindByField(question => question.QuestionId == questionUpdate.QuestionId);
                if (question == null)
                {
                    throw new Exception("Câu hỏi không tồn tại.");
                }

                if (questionUpdate.SectionId != null)
                {
                    var section = await _unitOfWork.SubjectSectionRepo.FindByField(subjectSection => subjectSection.SectionId == questionUpdate.SectionId);
                    if (section == null)
                    {
                        throw new Exception("Section không tồn tại");
                    }
                }

                if (questionUpdate.SchoolId != null)
                {
                    var subject = await _unitOfWork.SchoolRepo.FindByField(school => school.SchoolId == questionUpdate.SchoolId);
                    if (subject == null)
                    {
                        throw new Exception("School không tồn tại");
                    }
                }

                if (questionUpdate.IsUseToSell < 1 || questionUpdate.IsUseToSell > 2)
                {
                    throw new Exception("Trạng thái không hợp lệ");
                }

                question = _mapper.Map(questionUpdate, question);
                question.QuestionContent = JsonSerializer.Serialize((questionJson));
                question.ModifiedOn = DateTime.Now;
                question.ModifiedBy = currentUser;
                question.Status = questionUpdate.IsUseToSell;

                _unitOfWork.QuestionRepo.AddAsync(question);
                var result = await _unitOfWork.SaveChangesAsync();
                if (result <= 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở QuestionServices - UpdateQuestion: " + e.Message);
            }
        }
    }
}
