using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces.Share;
using Services.Interfaces.User;
using Services.ViewModels;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Controllers
{
    [Route("api/v0/shares")]
    [ApiController]
    public class ShareController : ControllerBase
    {
        private readonly IShareServices _shareServices;
        private readonly IMarketServices _marketServices;
        private readonly IUserServices _userServices;

        public ShareController(IShareServices shareServices, IUserServices userServices, IMarketServices marketServices)
        {
            _shareServices = shareServices;
            _userServices = userServices;
            _marketServices = marketServices;
        }

        [HttpGet("requests")]
        [Authorize(Roles = "2")] // Expert
        [SwaggerResponse(200, "List of request to share", typeof(IEnumerable<ShareViewModels>))]
        public async Task<IActionResult> GetRequestToShare(int? status, int? grade, int? subjectEnum, int? type, [Required] string studyYear)
        {
            try
            {
                var result = await _shareServices.GetRequestToShare(status, grade, subjectEnum, type, studyYear);

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


        [HttpPut("response/{shareId}")]
        [Authorize(Roles = "2")] // Expert
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
        [Authorize(Roles = "1")] // Teacher
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
        [Authorize(Roles = "1")] // Teacher
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

                if (result == null)
                {
                    return BadRequest(new
                    {
                        Message = "Chia sẻ thất bại"
                    });
                }

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


        [HttpGet("{questionSetId}")]
        [Authorize(Roles = "1")] // Teacher
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
        [Authorize(Roles = "1")] // Teacher
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
                var result = await _marketServices.BuyQuestionSet(buyQuestionSet, currentUser);

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


        [HttpGet("market")]
        [Authorize(Roles = "1,2")] // Teacher, Expert
        [SwaggerResponse(200, "List of question set in market", typeof(IEnumerable<ShareInMarket>))]
        public async Task<IActionResult> GetQuestionSetInMarket(int? grade, int? subjectEnum, string studyYear)
        {
            try
            {
                var currentUser = await _userServices.GetCurrentUser();
                var result = await _marketServices.GetQuestionSetInMarket(grade, subjectEnum, studyYear, currentUser);

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


        [HttpPut("report/{shareId}")]
        [Authorize(Roles = "1")] // Teacher
        [SwaggerResponse(200, "Is success", typeof(string))]
        public async Task<IActionResult> ReportShare(Guid shareId, NoteReport noteReport)
        {
            try
            {
                var currentUser = await _userServices.GetCurrentUser();
                var result = await _shareServices.ReportShare(shareId, currentUser, noteReport);

                if (!result)
                {
                    return NotFound(new
                    {
                        Message = "Báo cáo thất bại"
                    });
                }

                return Ok("Báo cáo thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }


        [HttpGet("bought-list")]
        [Authorize(Roles = "1")] // Teacher
        [SwaggerResponse(200, "List of bought question set", typeof(IEnumerable<ShareInMarket>))]
        public async Task<IActionResult> GetBoughtList(int? grade, int? subjectEnum, string studyYear)
        {
            try
            {
                var currentUser = await _userServices.GetCurrentUser();
                var result = await _marketServices.GetBoughtList(currentUser, grade, subjectEnum, studyYear);

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


        [HttpGet("sell-list")]
        [Authorize(Roles = "1")] // Teacher
        [SwaggerResponse(200, "List of sell question set", typeof(IEnumerable<ShareInMarket>))]
        public async Task<IActionResult> GetSellList(int? grade, int? subjectEnum, int? status, string studyYear)
        {
            try
            {
                var currentUser = await _userServices.GetCurrentUser();
                var result = await _marketServices.GetSoldList(currentUser, grade, subjectEnum, status, studyYear);

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
