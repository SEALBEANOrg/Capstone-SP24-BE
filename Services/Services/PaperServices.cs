﻿using AutoMapper;
using Repositories;
using Services.Interfaces;

namespace Services.Services
{
    public class PaperServices : IPaperServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PaperServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


    }
}
