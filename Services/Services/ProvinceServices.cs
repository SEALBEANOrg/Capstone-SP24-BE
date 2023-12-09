using AutoMapper;
using Repositories;
using Services.Interfaces;

namespace Services.Services
{
    public class ProvinceServices : IProvinceServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProvinceServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


    }
}
