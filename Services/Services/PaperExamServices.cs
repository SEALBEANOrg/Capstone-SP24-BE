using AutoMapper;
using Repositories;
using Services.Interfaces;

namespace Services.Services
{
    public class PaperExamServices : IPaperExamServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
    
        public PaperExamServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
    
    }
}
