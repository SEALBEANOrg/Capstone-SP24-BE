using AutoMapper;
using Repositories;
using Repositories.Models;
using Services.Interfaces;
using Services.ViewModels;

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

        public async Task<QuestionSetViewModel> GetQuestionByQuestionSetId (Guid questionSetId)
        {
            try
            {
                var questionSet = await _unitOfWork.QuestionSetRepo.FindByField(questionSet => questionSet.QuestionSetId == questionSetId);
                var questionIds = (await _unitOfWork.QuestionMappingRepo.GetAllAsync(questionMapping => questionMapping.QuestionSetId == questionSetId)).Select(questionMapping => questionMapping.QuestionId);
                var questions = await _unitOfWork.QuestionRepo.GetAllAsync(question => questionIds.Contains(question.QuestionId));
                if (questionSet == null)
                {
                    return null;
                }

                var questionSetViewModel = _mapper.Map<QuestionSetViewModel>(questionSet);

                return questionSetViewModel;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở QuestionSetServices - GetQuestionByQuestionSetId: " + e.Message);
            }
        }
    }
}
