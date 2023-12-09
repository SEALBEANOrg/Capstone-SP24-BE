using AutoMapper;
using Repositories;
using Services.Interfaces;

namespace Services.Services
{
    public class QuestionTransactionServices : IQuestionTransactionServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
    
        public QuestionTransactionServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
    
    }
}
