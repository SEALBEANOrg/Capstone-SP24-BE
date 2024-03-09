using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.ViewModels;
using Swashbuckle.AspNetCore.Annotations;

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
        [Authorize(Roles = "2")]
        [SwaggerResponse(200, "List of request to share", typeof(IEnumerable<ShareViewModels>))]
        public async Task<IActionResult> GetRequestToShare(int? status, int? grade, int? subjectEnum, int? type, int year)
        {
            try
            {
                var result = await _shareServices.GetRequestToShare(status, grade, subjectEnum, type, year);

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

        [HttpGet]
        [Authorize(Roles = "1")]
        [SwaggerResponse(200, "List of shared question set", typeof(IEnumerable<ShareViewModels>))]

        [HttpPut("response/{shareId}")]
        [Authorize(Roles = "2")]
        [SwaggerResponse(200, "Response request to share", typeof(string))]
        public async Task<IActionResult> ResponseRequestShare(Guid shareId, [FromBody] ResponseRequest responseRequest)
        {
            try
            {
                var currentUser = await _userServices.GetCurrentUser();
                var result = await _shareServices.ResponseRequestShare(shareId, responseRequest, currentUser);

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

        [HttpPost("requests")]
        [Authorize(Roles = "1")]
        [SwaggerResponse(200, "Is success", typeof(string))]
        public async Task<IActionResult> RequestToShare([FromBody] ShareCreateRequest shareCreate)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var currentUser = await _userServices.GetCurrentUser();
                var result = await _shareServices.RequestToShare(shareCreate, currentUser);

                if (!result)
                {
                    return NotFound(new
                    {
                        Message = "Yêu cầu thất bại"
                    });
                }

                return Ok("Yêu cầu thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpPost("individual")]
        [Authorize(Roles = "1")]
        [SwaggerResponse(200, "Is success", typeof(string))]
        public async Task<IActionResult> ShareIndividual([FromBody] ShareCreateForIndividual shareIndividual)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var currentUser = await _userServices.GetCurrentUser();
                var result = await _shareServices.ShareIndividual(shareIndividual, currentUser);

                if (!result)
                {
                    return NotFound(new
                    {
                        Message = "Chia sẻ thất bại"
                    });
                }

                return Ok("Chia sẻ thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpGet("{questionSetId}")]
        [Authorize(Roles = "1")]
        [SwaggerResponse(200, "Question set sample", typeof(List<string>))]
        public async Task<IActionResult> UserEmailOfSharedQuestionSet(Guid questionSetId, int? type)
        {
            try
            {
                var currentUserId = await _userServices.GetCurrentUser();
                var result = await _shareServices.GetUserEmailOfSharedQuestionSet(questionSetId, currentUserId, type);

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

        [HttpPost("buy")]
        [Authorize(Roles = "1")]
        [SwaggerResponse(200, "Is success", typeof(string))]
        public async Task<IActionResult> BuyQuestionSet([FromBody] BuyQuestionSet buyQuestionSet)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var currentUser = await _userServices.GetCurrentUser();
                var result = await _shareServices.BuyQuestionSet(buyQuestionSet, currentUser);

                if (!result)
                {
                    return NotFound(new
                    {
                        Message = "Mua thất bại"
                    });
                }

                return Ok("Mua thành công");
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
