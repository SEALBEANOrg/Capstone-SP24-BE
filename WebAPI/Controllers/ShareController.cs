using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.ViewModels;

namespace WebAPI.Controllers
{
    [Route("api/v0/shares")]
    [ApiController]
    public class ShareController : ControllerBase
    {
        private readonly IShareServices _shareServices;
        private readonly IUserServices _userServices;

        public ShareController(IShareServices shareServices, IUserServices userServices)
        {
            _shareServices = shareServices;
            _userServices = userServices;
        }

        [HttpGet("requests")]
        public async Task<IActionResult> GetRequestToShare(int? status, int? grade, int? subject, int? type)
        {
            try
            {
                var result = await _shareServices.GetRequestToShare(status, grade, subject, type);

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

        [HttpPut("requests/{id}")]
        public async Task<IActionResult> ResponseRequestShare(Guid id, [FromBody] ResponseRequest responseRequest)
        {
            try
            {
                var currentUser = await _userServices.GetCurrentUser();
                var result = await _shareServices.ResponseRequestShare(id, responseRequest, currentUser);

                if (!result)
                {
                    return NotFound(new
                    {
                        Message = "Phản hồi thất bại"
                    });
                }

                return Ok("Phản hồi thành công");
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
