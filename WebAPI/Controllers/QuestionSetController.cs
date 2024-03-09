using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.ViewModels;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Controllers
{
    [Route("api/v0/questionsets")]
    [ApiController]
    [Authorize]
    public class QuestionSetController : ControllerBase
    {
        private readonly IQuestionSetServices _questionSetServices;
        private readonly IUserServices _userServices;

        public QuestionSetController(IQuestionSetServices questionSetServices, IUserServices userServices)
        {
            _questionSetServices = questionSetServices;
            _userServices = userServices;
        }

        [HttpGet("own-questionset")]
        [SwaggerResponse(200, "List of question set", typeof(IEnumerable<OwnQuestionSet>))]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> GetOwnQuestionSet(int? grade, int? subject, [Required]int year)
        {
            try
            {
                var currentUser = await _userServices.GetCurrentUser();
                var result = await _questionSetServices.GetOwnQuestionSet(currentUser, grade, subject, year);

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

        [HttpGet("bank")]
        [SwaggerResponse(200, "List of question set", typeof(IEnumerable<QuestionSetViewModels>))]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> GetQuestionSetByBank(int? grade, int? subject, [Required]int year, int type)
        {
            try
            {
                var result = await _questionSetServices.GetQuestionSetBank(grade, subject, year, type);

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

        [HttpDelete("{questionSetId}")]
        [SwaggerResponse(200, "Is success", typeof(string))]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> DeleteQuestionSet(Guid questionSetId)
        {
            try
            {
                var currentUser = await _userServices.GetCurrentUser();
                var result = await _questionSetServices.DeleteQuestionSet(questionSetId, currentUser);

                if (!result)
                {
                    return NotFound(new
                    {
                        Message = "Xóa thất bại"
                    });
                }

                return Ok("Xóa thành công");
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
        [SwaggerResponse(200, "Question set sample", typeof(QuestionSetViewModel))]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> GetQuestionByQuestionSetId(Guid questionSetId)
        {
            try
            {
                var result = await _questionSetServices.GetQuestionByQuestionSetId(questionSetId);

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

        [HttpPost("import-questionset")]
        [SwaggerResponse(200, "Detail question set from import", typeof(QuestionReturn))]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> GetQuestionSetFromFile([FromForm] ImportQuestionSet importQuestionSet)
        {
            try
            {
                var result = await _questionSetServices.GetQuestionSetFromFile(importQuestionSet);

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

        [HttpPost("save")]
        [SwaggerResponse(200, "Is success", typeof(string))]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> SaveQuestionSet([FromBody] QuestionSetSave questionSetSave)
        {
            try
            {
                var currentUser = await _userServices.GetCurrentUser();
                var result = await _questionSetServices.SaveQuestionSet(questionSetSave, currentUser);

                if (!result)
                {
                    return NotFound(new
                    {
                        Message = "Lưu thất bại"
                    });
                }

                return Ok("Lưu thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpPut("{questionSetId}/change-status")]
        public async Task<IActionResult> ChangeStatusQuestionSet(Guid questionSetId, [FromBody] StatusQuestionSet statusQuestionSet)
        {
            try
            {
                var currentUser = await _userServices.GetCurrentUser();
                var result = await _questionSetServices.ChangeStatusQuestionSet(questionSetId, statusQuestionSet.IsActive, currentUser);

                if (!result)
                {
                    return NotFound(new
                    {
                        Message = "Thay đổi trạng thái thất bại"
                    });
                }

                return Ok("Thay đổi trạng thái thành công");
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
