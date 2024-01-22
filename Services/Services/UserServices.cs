using AutoMapper;
using FirebaseAdmin.Auth;
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
            var user = await _unitOfWork.UserRepo.FindByField(user => user.Email == email);
            var userInfo = _mapper.Map<UserInfo>(user);
            return userInfo;
        }

        public async Task<IEnumerable<UserViewModels>> GetAllUser()
        {
            var users = await _unitOfWork.UserRepo.GetAllAsync(x => x.School);
            if (users == null)
            {
                return null;
            }
            var userViewModels = _mapper.Map<IEnumerable<UserViewModels>>(users);
            return userViewModels;
        }

        public async Task<bool> RequestJoinSchool(Guid schoolId)
        {
            //var currentUser = await _unitOfWork.UserRepo.FindByField(x => x.UserId == userId);
            var currentUser = await _unitOfWork.UserRepo.FindByField(user => user.UserId == _claimsService.GetCurrentUser);

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
                var school = await _unitOfWork.SchoolRepo.FindByField(school => school.SchoolId == schoolId);
                
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
            var user = await _unitOfWork.UserRepo.FindByField(user => user.UserId == userId);
            var currentUser = await GetCurrentUser();

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
            user.ModifiedBy = currentUser;

            _unitOfWork.UserRepo.Update(user);
            
            return (await _unitOfWork.SaveChangesAsync()) > 0;
        }

        public async Task<IEnumerable<Request>> GetListRequestToMySchool()
        {
            var currentUserId = _claimsService.GetCurrentUser;
            var currentUser = await _unitOfWork.UserRepo.FindByField(user => user.UserId == currentUserId);
            var requests = await _unitOfWork.UserRepo.FindListByField(user => user.SchoolId == currentUser.SchoolId && user.Status == 2);

            if (requests == null)
            {
                return null;
            }

            var requestViewModels = _mapper.Map<IEnumerable<Request>>(requests);
            return requestViewModels;
        }

        public async Task<UserInfo> FindUserById(Guid guid)
        {
            var user = await _unitOfWork.UserRepo.FindByField(user => user.UserId == guid);

            if (user == null)
            {
                return null;
            }

            return _mapper.Map<UserInfo>(user);
        }

        public async Task<string> RegisterAsync(string email)
        {
            var userArgs = new UserRecordArgs()
            {
                Email = email,

            };
            
            var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(userArgs);

            //add role into firebase
            var claims = new Dictionary<string, object>()
            {
                {"role", "1"}
            };
            await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(userRecord.Uid, claims);

            return userRecord.Uid;
        }

        public async Task<bool> CheckExistInFirebase(string email)
        {   try
            {
                var userRecord = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);

                if (userRecord == null)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public async Task<UserViewModels> GetProfile()
        {
            try
            {
                var currentUserId = _claimsService.GetCurrentUser;
                var user = await _unitOfWork.UserRepo.FindByField(user => user.UserId == currentUserId);

                if (user == null)
                {
                    return null;
                }

                var userViewModels = _mapper.Map<UserViewModels>(user);
                return userViewModels;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở UserServices - GetProfile: " + e.Message);
            }
        }

        public async Task<bool> UpdateProfile(UserUpdate userUpdate)
        {
            try
            {
                var currentUserId = _claimsService.GetCurrentUser;
                var user = await _unitOfWork.UserRepo.FindByField(user => user.UserId == currentUserId);

                if (string.IsNullOrEmpty(userUpdate.FullName))
                {
                    throw new Exception("Họ tên không được để trống");
                }

                bool isExistEmailInOtherUser = false;

                if (userUpdate.Phone != null)
                {
                    var phoneInOtherUser = await _unitOfWork.UserRepo.FindByField(user => user.Phone == userUpdate.Phone && user.UserId != currentUserId);
                    if (phoneInOtherUser != null)
                    {
                        isExistEmailInOtherUser = true;
                    }
                }

                if (isExistEmailInOtherUser)
                {
                    throw new Exception("Số điện thoại đã được đăng ký ở một tài khoản khác");
                }

                user.FullName = userUpdate.FullName;
                user.Phone = userUpdate.Phone;
                user.ModifiedOn = DateTime.Now;
                user.ModifiedBy = currentUserId;

                _unitOfWork.UserRepo.Update(user);
                var result = await _unitOfWork.SaveChangesAsync();
                if (result <= 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở UserServices - UpdateProfile: " + e.Message);
            }
        }

        public async Task<UserViewModels> GetUserById(Guid id)
        {
            try
            {
                var user = await _unitOfWork.UserRepo.FindByField(user => user.UserId == id);

                if (user == null)
                {
                    throw new Exception("Người dùng không tồn tại");
                }

                var userViewModels = _mapper.Map<UserViewModels>(user);
                return userViewModels;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở UserServices - GetUserById: " + e.Message);
            }
        }

        public async Task<bool> OutSchool()
        {
            try 
            {                 
                var currentUserId = _claimsService.GetCurrentUser;
                var user = await _unitOfWork.UserRepo.FindByField(user => user.UserId == currentUserId);
                var school = await _unitOfWork.SchoolRepo.FindByField(school => school.SchoolId == user.SchoolId);
                
                if (school == null)
                {
                    throw new Exception("Bạn không trong trường nào");
                }

                if (user.UserType == 2)
                {
                    var users = await _unitOfWork.UserRepo.FindListByField(user => user.SchoolId == user.SchoolId);
                    if (users.Count() > 1 && school.Status == 1)
                    {
                        throw new Exception("Trường học đang hoạt động và bạn là admin, không thể tự rời trường");
                    }
                }
                user.SchoolId = null;
                user.ModifiedOn = DateTime.Now;
                user.ModifiedBy = currentUserId;
                user.Status = 1;
                _unitOfWork.UserRepo.Update(user);
                var result = await _unitOfWork.SaveChangesAsync();
                
                return result > 0;
            }
            catch(Exception e)
            {
                throw new Exception("Lỗi ở UserServices - OutSchool: " + e.Message);
            }
        }
    }
}
