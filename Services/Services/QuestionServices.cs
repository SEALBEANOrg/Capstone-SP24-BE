using AutoMapper;
using ExagenSharedProject;
using Repositories;
using Repositories.Models;
using Services.Interfaces;
using Services.ViewModels;
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using System.Text.Json;
using static System.Collections.Specialized.BitVector32;

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
            //try
            //{
            //    // check if answers contains correct answer
            //    if (!questionCreate.CorrectAnswers.Any(answer => questionCreate.Answers.Contains(answer)))
            //    {
            //        throw new Exception("Câu trả lời đúng không hợp lệ vì không nằm trong các đáp án được đưa ra.");
            //    } 

            //    QuestionJson questionJson = new QuestionJson
            //    {
            //        Content = questionCreate.Content,
            //        Answers = questionCreate.Answers,
            //        CorrectAnswers = questionCreate.CorrectAnswers
            //    };


            //    if (questionCreate.SectionId != null)
            //    {
            //        var section = await _unitOfWork.SubjectSectionRepo.FindByField(subjectSection => subjectSection.SectionId == questionCreate.SectionId);
            //        if (section == null)
            //        {
            //            throw new Exception("Section không tồn tại");
            //        }
            //    }

            //    if (questionCreate.SchoolId != null)
            //    {
            //        var subject = await _unitOfWork.SchoolRepo.FindByField(school => school.SchoolId == questionCreate.SchoolId);
            //        if (subject == null)
            //        {
            //            throw new Exception("School không tồn tại");
            //        }
            //    }

            //    if (questionCreate.Subject != null && (questionCreate.Subject < 1 || questionCreate.Subject > 3))
            //    {
            //        throw new Exception("Môn học không hợp lệ");
            //    }

            //    var question = _mapper.Map<Question>(questionCreate);
            //    question.QuestionContent = JsonSerializer.Serialize(questionJson, new JsonSerializerOptions
            //    {
            //        WriteIndented = true,
            //        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            //    });
            //    question.CreatedOn = DateTime.Now;
            //    question.ModifiedOn = DateTime.Now;
            //    question.CreatedBy = currentUser;
            //    question.ModifiedBy = currentUser;
            //    question.Status = 0;

            //    _unitOfWork.QuestionRepo.AddAsync(question);
            //    var result = await _unitOfWork.SaveChangesAsync();
            //    if (result <= 0)
            //    {
            //        return false;
            //    }

            //    return true;

            //}
            //catch (Exception e)
            //{
            //    throw new Exception("Lỗi ở QuestionServices - AddQuestion: " + e.Message);
            //}
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteQuestion(Guid questionId, Guid currentUserId, int grade)
        {
            //try
            //{
            //    var question = await _unitOfWork.QuestionRepo.FindByField(question => question.QuestionId == questionId && question.Grade == grade);
            //    if (question == null)
            //    {
            //        throw new Exception("Câu hỏi không tồn tại.");
            //    }

            //    var currentUser = await _unitOfWork.UserRepo.FindByField(user => user.UserId == currentUserId);

            //    var isShare = await CheckPermissionForQuestion(questionId, 3, currentUserId, currentUser.SchoolId);

            //    if (question.CreatedBy != currentUserId &&  !isShare)
            //    {
            //        throw new Exception("Bạn không có quyền xóa câu hỏi này.");
            //    }

            //    _unitOfWork.QuestionRepo.Remove(question);

            //    var result = await _unitOfWork.SaveChangesAsync();

            //    if (result <= 0)
            //    {
            //        return false;
            //    }

            //    return true;
            //}
            //catch (Exception e)
            //{
            //    throw new Exception("Lỗi ở QuestionServices - DeleteQuestion: " + e.Message);
            //}

            throw new NotImplementedException();
        }

        public async Task<IEnumerable<QuestionViewModels>> GetAllMyQuestionByGrade(int grade, Guid currentUserId)
        {
            try
            {
                var questions = await _unitOfWork.QuestionRepo.FindListByField(questions => questions.Grade == grade);
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

        public async Task<IEnumerable<QuestionViewModels>> GetAllValidQuestionByGradeForMe(int? subject, int grade, Guid? sectionId, Guid currentUserId)
        {
            //try
            //{
            //    var currentUser = await _unitOfWork.UserRepo.FindByField(user => user.UserId == currentUserId);

            //    var listQuestionId = await GetQuestionIdIsSharedToRead(currentUserId, currentUser.SchoolId);

            //    var questions = await _unitOfWork.QuestionRepo.FindListByField(question => question.Grade == grade && 
            //                                                                                (listQuestionId.Contains(question.QuestionId) || question.CreatedBy == currentUserId));

            //    if (subject != null)
            //    {
            //        questions = questions.Where(question => question.Subject == subject).ToList();
            //    }

            //    if (sectionId != null)
            //    {
            //        questions = questions.Where(question => question.SectionId == sectionId).ToList();
            //    }

            //    if (questions == null)
            //    {
            //        return null;
            //    }

            //    var questionViewModels = _mapper.Map<IEnumerable<QuestionViewModels>>(questions);
            //    return questionViewModels;

            //}
            //catch (Exception e)
            //{
            //    throw new Exception("Lỗi ở QuestionServices - GetAllValidQuestionByGradeForMe: " + e.Message);
            //}

            throw new NotImplementedException();
        }

        public async Task<QuestionViewModels> GetQuestionByQuestionIdAndGrade(Guid questionId, int grade, Guid currentUserId)
        {
            //try
            //{
            //    var question = await _unitOfWork.QuestionRepo.FindByField(question => question.Grade == grade && question.QuestionId == questionId);
            //    if (question == null)
            //    {
            //        throw new Exception("Câu hỏi không tồn tại.");
            //    }

            //    var currentUser = await _unitOfWork.UserRepo.FindByField(user => user.UserId == currentUserId);
            //    if (!(await CheckPermissionForQuestion(questionId, 0, currentUserId, currentUser.SchoolId)))
            //    {
            //        throw new Exception("Bạn không có quyền xem câu hỏi này.");
            //    }

            //    var questionViewModels = _mapper.Map<QuestionViewModels>(question);
            //    return questionViewModels;
            //}
            //catch (Exception e)
            //{
            //    throw new Exception("Lỗi ở QuestionServices - GetQuestionById: " + e.Message);
            //}

            throw new NotImplementedException();
        }

        public async Task<IEnumerable<QuestionViewModels>> GetQuestionBySectionIdAndGrade(Guid sectionId, int grade)
        {
            try
            {
                var questions = await _unitOfWork.QuestionRepo.FindListByField(question => question.SectionId == sectionId && question.Grade == grade);
                if (questions == null)
                {
                    return null;
                }

                var questionViewModels = _mapper.Map<IEnumerable<QuestionViewModels>>(questions);
                return questionViewModels;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở QuestionServices - GetQuestionBySectionId: " + e.Message);
            }
        }

        public async Task<IEnumerable<QuestionViewModels>> GetQuestionBySubjectAndSectionAndGrade(int grade, int subject, Guid? sectionId)
        {
            //try
            //{
            //    var questions = await _unitOfWork.QuestionRepo.FindListByField(question => question.Subject == subject && question.Grade == grade);
                
            //    if (sectionId != null)
            //    {
            //        questions = questions.Where(question => question.SectionId == sectionId).ToList();
            //    }
                
            //    if (questions == null)
            //    {
            //        return null;
            //    }

            //    var questionViewModels = _mapper.Map<IEnumerable<QuestionViewModels>>(questions);
            //    return questionViewModels;
            //}
            //catch (Exception e)
            //{
            //    throw new Exception("Lỗi ở QuestionServices - GetQuestionBySubject: " + e.Message);
            //}

            throw new NotImplementedException();
        }

        public async Task<bool> UpdateQuestion(QuestionUpdate questionUpdate, Guid currentUser, int grade)
        {

            //try
            //{
            //    if (!questionUpdate.CorrectAnswers.Any(answer => questionUpdate.Answers.Contains(answer)))
            //    {
            //        throw new Exception("Câu trả lời đúng không hợp lệ vì không nằm trong các đáp án được đưa ra.");
            //    }

            //    QuestionJson questionJson = new QuestionJson
            //    {
            //        Content = questionUpdate.Content,
            //        Answers = questionUpdate.Answers,
            //        CorrectAnswers = questionUpdate.CorrectAnswers
            //    };

            //    var question = await _unitOfWork.QuestionRepo.FindByField(question => question.QuestionId == questionUpdate.QuestionId && question.Grade == grade);
            //    if (question == null)
            //    {
            //        throw new Exception("Câu hỏi không tồn tại.");
            //    }

            //    if (questionUpdate.SectionId != null)
            //    {
            //        var section = await _unitOfWork.SubjectSectionRepo.FindByField(subjectSection => subjectSection.SectionId == questionUpdate.SectionId);
            //        if (section == null)
            //        {
            //            throw new Exception("Section không tồn tại");
            //        }
            //    }

            //    if (questionUpdate.SchoolId != null)
            //    {
            //        var subject = await _unitOfWork.SchoolRepo.FindByField(school => school.SchoolId == questionUpdate.SchoolId);
            //        if (subject == null)
            //        {
            //            throw new Exception("School không tồn tại");
            //        }
            //    }


            //    question = _mapper.Map(questionUpdate, question);
            //    question.QuestionContent = JsonSerializer.Serialize((questionJson));
            //    question.ModifiedOn = DateTime.Now;
            //    question.ModifiedBy = currentUser;

            //    _unitOfWork.QuestionRepo.AddAsync(question);
            //    var result = await _unitOfWork.SaveChangesAsync();
            //    if (result <= 0)
            //    {
            //        return false;
            //    }

            //    return true;
            //}
            //catch (Exception e)
            //{
            //    throw new Exception("Lỗi ở QuestionServices - UpdateQuestion: " + e.Message);
            //}
            throw new NotImplementedException();
        }

        private async Task<bool> CheckPermissionForQuestion(Guid questionId, int permissionType, Guid userId, Guid? schoolId)
        {
            //if (schoolId == null)
            //{
            //    var share = await _unitOfWork.ShareRepo.FindByField(share => share.QuestionSetId == questionId && share.PermissionType >= permissionType &&
            //                                                                    share.UserId == userId);
            //    if (share == null)
            //    {
            //        return false;
            //    }

            //    return true;

            //}
            //else
            //{
            //    var share = await _unitOfWork.ShareRepo.FindByField(share => share.QuestionSetId == questionId && share.PermissionType >= permissionType &&
            //                                                                    share.SchoolId == schoolId);
            //    if (share == null)
            //    {
            //        return false;
            //    }

            //    return true;
            //}

            throw new NotImplementedException();
        }
    
        private async Task<IEnumerable<Guid>> GetQuestionIdIsSharedToRead(Guid userId, Guid? schoolId)
        {
            //if (schoolId == null)
            //{
            //    var shares = await _unitOfWork.ShareRepo.FindListByField(share => share.UserId == userId);
            //    if (shares == null)
            //    {
            //        return null;
            //    }

            //    var questionIds = shares.Select(share => share.QuestionSetId);
                
            //    if (questionIds == null)
            //    {
            //        return null;
            //    }

            //    return (IEnumerable<Guid>)questionIds;
            //}
            //else
            //{
            //    var shares = await _unitOfWork.ShareRepo.FindListByField(share => share.SchoolId == schoolId);
            //    if (shares == null)
            //    {
            //        return null;
            //    }

            //    var questionIds = shares.Select(share => share.QuestionSetId);

            //    if (questionIds == null)
            //    {
            //        return null;
            //    }

            //    return (IEnumerable<Guid>)questionIds;
            //}

            throw new NotImplementedException();
        }
    }
}
