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

        public async Task<bool> CheckPermissionAccessTest(string testCode, string email)
        {
            //var user = await _unitOfWork.UserRepo.FindByField(user => user.Email == email);
            //var test = await _unitOfWork.TestRepo.FindByField(test => test.TestCode == testCode);
            //if (user.SchoolId != test.SchoolId)
            //{
            //    return false;
            //}

            //return true;

            throw new NotImplementedException();
        }
    }
}
