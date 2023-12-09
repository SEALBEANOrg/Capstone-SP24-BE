using AutoMapper;
using Repositories;
using Services.Interfaces;

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


    }
}
