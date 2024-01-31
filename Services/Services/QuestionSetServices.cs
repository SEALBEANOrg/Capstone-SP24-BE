using AutoMapper;
using Repositories;
using Services.Interfaces;

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

        //public async Task<bool> CheckPermissionAccessTest(string testCode, string email)
        //{
        //    var user = await _unitOfWork.UserRepo.FindByField(user => user.Email == email);
        //    if (user == null)
        //    {
        //        return false;
        //    }

        //    int testCodeInt = int.Parse(testCode);
        //    var test = await _unitOfWork.ExamRepo.FindByField(test => test.TestCode == testCodeInt);
        //    if (user == null)
        //    {
        //        return false;
        //    }

        //    bool isShare = false;

        //    var share = await _unitOfWork.ShareRepo.FindByField(share => share.TestId == test.TestId && 
        //                                                                (share.UserId == user.UserId || (user.SchoolId != null && share.SchoolId == user.SchoolId)));
        //    if (test.CreatedBy != user.UserId && isShare)
        //    {
        //        return false;
        //    }

        //    return true;
        //}
    }
}
