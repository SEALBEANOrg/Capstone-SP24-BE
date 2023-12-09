using AutoMapper;
using Repositories;
using Services.Interfaces;

namespace Services.Services
{
    public class TestResultServices : ITestResultServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TestResultServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

    }
}
