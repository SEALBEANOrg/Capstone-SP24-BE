using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.ViewModels;
using System.Data;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
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
        public async Task<IActionResult> GetAll()
        {
            var users = await _userServices.GetAllUser();

            return Ok(users);
        }

        [HttpGet("GetUserById/{id}")]
        [Authorize(Roles = "0")] //chỉ 0
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

        [HttpGet("GetProfile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var user = await _userServices.GetProfile();

            return Ok(user);
        }

        [HttpPut("UpdateProfile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UserUpdate userUpdate)
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

        [HttpPut("/OutSchool")]
        [Authorize(Roles = "1,2")]
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

        [HttpPut("/Request/RequestJoinSchool")]
        [Authorize(Roles = "1")]
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

        [HttpPut("/Request/ResponseRequest/{userId}")]
        [Authorize(Roles = "2")]
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

        [HttpGet("GetListRequestToMySchool")]
        [Authorize(Roles = "2")]
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
    }
}
