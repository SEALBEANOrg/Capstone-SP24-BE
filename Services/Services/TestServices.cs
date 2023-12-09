using AutoMapper;
using Repositories;
using Services.Interfaces;

namespace Services.Services
{
    public class TestServices : ITestServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TestServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

    }
}
