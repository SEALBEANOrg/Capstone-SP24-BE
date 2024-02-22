using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.ViewModels;
using Swashbuckle.AspNetCore.Annotations;
using System.Data;
using static Google.Apis.Requests.BatchRequest;

namespace WebAPI.Controllers
{
    [Route("api/v0/users")]
    [ApiController]
    public class UserControllers : ControllerBase
    {
        private readonly IUserServices _userServices;

        public UserControllers(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [HttpGet]
        [Authorize(Roles = "0")] //chỉ 0
        [SwaggerResponse(200, "List of sample users", typeof(IEnumerable<UserViewModels>))]
        public async Task<IActionResult> GetAll(string? search)
        {
            var users = await _userServices.GetAllUser(search);

            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "0,2")] //chỉ 0, 2
        [SwaggerResponse(200, "Sample user", typeof(UserViewModels))]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _userServices.GetUserById(id);

                return Ok(user);
            }
            catch (DataException ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpGet("profile")]
        [Authorize]
        [SwaggerResponse(200, "Sample profile", typeof(UserViewModels))]
        public async Task<IActionResult> GetProfile()
        {
            var user = await _userServices.GetProfile();

            return Ok(user);
        }

        [HttpPut("profile")]
        [Authorize]
        [SwaggerResponse(200, "Is success", typeof(string))]
        public async Task<IActionResult> UpdateProfile([FromBody] ProfileUpdate userUpdate)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _userServices.UpdateProfile(userUpdate);

                if (!result)
                {
                    return BadRequest(new
                    {
                        Message = "Update Profile thất bại"
                    });
                }

                return Ok("Đã cập nhật thông tin cá nhân");
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpPut("{id}/role")]
        [Authorize(Roles = "0")]
        [SwaggerResponse(200, "Is success", typeof(string))]
        public async Task<IActionResult> UpdateRoleUser(Guid id, [FromBody] RoleUpdate roleUpdate)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _userServices.UpdateRoleUser(id, roleUpdate);

                if (!result)
                {
                    return BadRequest(new
                    {
                        Message = "Update Role thất bại"
                    });
                }

                return Ok("Đã cập nhật thông tin cá nhân");
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpPut("/api/v0/schools/out-school")]
        [Authorize(Roles = "1,2")]
        [SwaggerResponse(200, "Is success", typeof(string))]
        public async Task<IActionResult> OutSchool()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _userServices.OutSchool();

                if (!result)
                {
                    return BadRequest(new
                    {
                        Message = "Out School thất bại"
                    });
                }

                return Ok("Đã rời khỏi trường");
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpPut("/api/v0/requests/join-school")]
        [Authorize(Roles = "1")]
        [SwaggerResponse(200, "Is success", typeof(bool))]
        public async Task<IActionResult> RequestJoinSchool([FromBody] Guid schoolId)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _userServices.RequestJoinSchool(schoolId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpPut("/api/v0/requests/response/{userId}")]
        [Authorize(Roles = "2")]
        [SwaggerResponse(200, "Is success", typeof(bool))]
        public async Task<IActionResult> ResponseRequest(Guid userId, [FromBody] bool isAccept)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _userServices.ResponseRequest(userId, isAccept);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpGet("/api/v0/requests")]
        [Authorize(Roles = "2")]
        [SwaggerResponse(200, "List of request", typeof(IEnumerable<Request>))]
        public async Task<IActionResult> GetListRequestToMySchool()
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _userServices.GetListRequestToMySchool();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "0")]
        [SwaggerResponse(200, "Is success", typeof(string))]
        public async Task<IActionResult> ChangeStatusOfUser(Guid id, [FromBody] ActiveUser isActive)
        {
            try
            {
                var result = await _userServices.ChangeStatusOfUser(id, isActive.isActive);
                if (!result)
                {
                    return BadRequest(new
                    {
                        Message = "Change Status thất bại"
                    });
                }

                return Ok("Đã cập nhật trạng thái người dùng");
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

    }
}
