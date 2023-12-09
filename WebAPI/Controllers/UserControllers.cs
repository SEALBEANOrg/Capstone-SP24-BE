using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

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
        public async Task<IActionResult> GetAll()
        {
            var users = await _userServices.GetAllUser();

            return Ok(users);
        }

        [HttpPut("/Request/RequestJoinSchool")]
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
