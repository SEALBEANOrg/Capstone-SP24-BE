using AutoMapper;
using FirebaseAdmin.Auth;
using Repositories;
using Services.Interfaces.User;
using Services.ViewModels;

namespace Services.Services.User
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
            var user = _mapper.Map<Repositories.Models.User>(userSignUp);
            user.Point = 200;
            user.Status = 1;
            user.UserType = 1;
            user.CreatedOn = DateTime.Now;
            user.ModifiedOn = DateTime.Now;

            try
            {
                _unitOfWork.UserRepo.AddAsync(user);
                var result = await _unitOfWork.SaveChangesAsync();
                if (result <= 0)
                {
                    return null;
                }

                var transaction = new Repositories.Models.Transaction
                {
                    UserId = user.UserId,
                    PointValue = 200,
                    Type = 5, // đăng nhập lần đầu
                    CreatedOn = DateTime.Now
                };

                _unitOfWork.TransactionRepo.AddAsync(transaction);
                result = await _unitOfWork.SaveChangesAsync();

                var userInfo = _mapper.Map<UserInfo>(user);

                return userInfo;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi ở UserServices - CreateNewUser: " + ex);
            }

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

        public async Task<Guid> GetCurrentUser()
        {
            return _claimsService.GetCurrentUser;
        }

        public async Task<IEnumerable<UserViewModels>> GetAllUser(string? search, int? role, int? status)
        {
            var users = await _unitOfWork.UserRepo.GetAllAsync();

            if (users == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(search))
            {
                users = users.Where(x => x.Email.ToLower().Contains(search.ToLower())).ToList();
            }

            if (role != null)
            {
                users = users.Where(x => x.UserType == role).ToList();
            }

            if (status != null)
            {
                users = users.Where(x => x.Status == status).ToList();
            }

            var userViewModels = _mapper.Map<IEnumerable<UserViewModels>>(users);
            return userViewModels;
        }

        public async Task<UserViewModel> GetProfile()
        {
            try
            {
                var currentUserId = _claimsService.GetCurrentUser;
                var user = await _unitOfWork.UserRepo.FindByField(user => user.UserId == currentUserId);

                if (user == null)
                {
                    return null;
                }

                var userViewModels = _mapper.Map<UserViewModel>(user);
                
                if (user.SchoolId != null)
                {
                    userViewModels.DropdownSchools = new DropdownSchools
                    {
                        SchoolId = (Guid)user.SchoolId,
                        SchoolNameIdentity = $"{user.School.Name} - {user.School.Address}, {user.School.Province}, {user.School.City}"
                    };
                }

                return userViewModels;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở UserServices - GetProfile: " + e.Message);
            }
        }

        public async Task<UserInfo> FindUserByEmail(string email)
        {
            var user = await _unitOfWork.UserRepo.FindByField(user => user.Email == email);
            var userInfo = _mapper.Map<UserInfo>(user);
            return userInfo;
        }

        public async Task<UserViewModel> GetUserById(Guid id)
        {
            try
            {
                var user = await _unitOfWork.UserRepo.FindByField(user => user.UserId == id);

                if (user == null)
                {
                    throw new Exception("Người dùng không tồn tại");
                }

                var userViewModels = _mapper.Map<UserViewModel>(user);
                if (user.SchoolId != null)
                {
                    userViewModels.DropdownSchools = new DropdownSchools
                    {
                        SchoolId = (Guid)user.SchoolId,
                        SchoolNameIdentity = $"{user.School.Name} - {user.School.Address}, {user.School.Province}, {user.School.City}"
                    };
                }

                return userViewModels;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở UserServices - GetUserById: " + e.Message);
            }
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

        public async Task<bool> UpdateProfile(ProfileUpdate userUpdate)
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
                user.SchoolId = userUpdate.SchoolId;
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

        public async Task<bool> UpdateRoleUser(Guid id, RoleUpdate roleUpdate)
        {
            try
            {
                var user = await _unitOfWork.UserRepo.FindByField(user => user.UserId == id);

                if (user == null)
                {
                    return false;
                }

                user.UserType = roleUpdate.UserType;
                user.ModifiedOn = DateTime.Now;
                user.ModifiedBy = _claimsService.GetCurrentUser;

                _unitOfWork.UserRepo.Update(user);
                var result = await _unitOfWork.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở UserServices - UpdateRoleUser: " + e.Message);
            }
        }

        public async Task<bool> ChangeStatusOfUser(Guid id, bool isActive)
        {
            try
            {
                var user = await _unitOfWork.UserRepo.FindByField(user => user.UserId == id);
                if (user == null)
                {
                    return false;
                }

                user.Status = isActive ? 1 : 0;

                user.ModifiedOn = DateTime.Now;
                user.ModifiedBy = _claimsService.GetCurrentUser;

                _unitOfWork.UserRepo.Update(user);
                var result = await _unitOfWork.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở UserServices - ChangeStatusOfUser: " + e.Message);
            }
        }

        public async Task<bool> CheckExistInFirebase(string email)
        {
            try
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
    }
}
