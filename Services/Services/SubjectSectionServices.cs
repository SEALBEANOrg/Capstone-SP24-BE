using AutoMapper;
using Repositories;
using Services.Interfaces;

namespace Services.Services
{
    public class SubjectSectionServices : ISubjectSectionServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SubjectSectionServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
    }
}
