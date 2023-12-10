using AutoMapper;
using Repositories;
using Repositories.Models;
using Services.Interfaces;
using Services.ViewModels;

namespace Services.Services
{
    public class UserServices : IUserServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IClaimsService _claimsService;

        public UserServices(IUnitOfWork unitOfWork, IMapper mapper, IClaimsService claimsService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _claimsService = claimsService;
        }

        public async Task<UserInfo?> CreateNewUser(UserSignUp userSignUp)
        {
            var user = _mapper.Map<User>(userSignUp);
            user.Point = 0;
            user.Status = 1;
            user.UserType = 1;
            user.CreatedOn = DateTime.Now;
            user.ModifiedOn = DateTime.Now;
            
            _unitOfWork.UserRepo.AddAsync(user);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                return null;
            }

            var userInfo = _mapper.Map<UserInfo>(user);
            
            return userInfo;
        }

        public async Task<UserInfo> FindUserByEmail(string email)
        {
            var user = await _unitOfWork.UserRepo.FindByField(x => x.Email == email);
            var userInfo = _mapper.Map<UserInfo>(user);
            return userInfo;
        }

        public async Task<UserViewModels> GetAllUser()
        {
            var users = await _unitOfWork.UserRepo.GetAllAsync();
            if (users == null)
            {
                return null;
            }
            var userViewModels = _mapper.Map<UserViewModels>(users);
            return userViewModels;
        }

        public async Task<bool> RequestJoinSchool(Guid schoolId)
        {
            //var currentUser = await _unitOfWork.UserRepo.FindByField(x => x.UserId == userId);
            var currentUser = await _unitOfWork.UserRepo.FindByField(x => x.UserId == _claimsService.GetCurrentUser);

            if (currentUser.Status == 0)
            {
                throw new Exception("Trạng thái tài khoản không khả dụng");
            }
            else if (currentUser.Status == 2)
            {
                throw new Exception("Tài khoản đã có yêu cầu thêm vào trường cần chờ xác thực");
            }
            else if (currentUser.Status == 3)
            {
                throw new Exception("Tài khoản đã có trong trường học");
            }
            else if (currentUser.Status == 1)
            {
                var school = await _unitOfWork.SchoolRepo.FindByField(x => x.SchoolId == schoolId);
                
                if (school == null)
                {
                    throw new Exception("Trường học không tồn tại");
                }

                currentUser.SchoolId = schoolId;
                currentUser.ModifiedOn = DateTime.Now;
                currentUser.ModifiedBy = _claimsService.GetCurrentUser;
                currentUser.Status = 2;

                try
                {
                    _unitOfWork.UserRepo.Update(currentUser);
                    var result = await _unitOfWork.SaveChangesAsync();
                    if (result <= 0)
                    {
                        return false;
                    }
                    return true;
                }
                catch (Exception e)
                {
                    throw new Exception("Lỗi ở UserServices - RequestJoinSchool: " + e.Message);
                }
            }
            return false;
        }

        public async Task<Guid> GetCurrentUser()
        {
            return _claimsService.GetCurrentUser;
        }

        public async Task<bool> ResponseRequest(Guid userId, bool isAccept)
        {
            var user = await _unitOfWork.UserRepo.FindByField(x => x.UserId == userId);
            if (user == null)
            {
                throw new Exception("Tài khoản không tồn tại");
            }

            if (isAccept)
            {
                user.Status = 3;
            }
            else
            {
                user.Status = 1;
                user.SchoolId = null;
            }

            user.ModifiedOn = DateTime.Now;
            user.ModifiedBy = await GetCurrentUser();

            _unitOfWork.UserRepo.Update(user);
            
            return (await _unitOfWork.SaveChangesAsync()) > 0;
        }

        public async Task<IEnumerable<Request>> GetListRequestToMySchool()
        {
            var currentUserId = _claimsService.GetCurrentUser;
            var currentUser = await _unitOfWork.UserRepo.FindByField(x => x.UserId == currentUserId);
            var requests = await _unitOfWork.UserRepo.FindListByField(x => x.SchoolId == currentUser.SchoolId && x.Status == 2);

            if (requests == null)
            {
                return null;
            }

            var requestViewModels = _mapper.Map<IEnumerable<Request>>(requests);
            return requestViewModels;
        }

        public async Task<UserInfo> FindUserById(Guid guid)
        {
            var user = await _unitOfWork.UserRepo.FindByField(x => x.UserId == guid);

            if (user == null)
            {
                return null;
            }

            return _mapper.Map<UserInfo>(user);
        }
    }
}
