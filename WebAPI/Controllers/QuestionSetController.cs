using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.ViewModels;

namespace WebAPI.Controllers
{
    [Route("api/v0/questionsets")]
    [ApiController]
    public class QuestionSetController : ControllerBase
    {
        private readonly IQuestionSetServices _questionSetServices;
        private readonly IUserServices _userServices;

        public QuestionSetController(IQuestionSetServices questionSetServices, IUserServices userServices)
        {
            _questionSetServices = questionSetServices;
            _userServices = userServices;
        }

        [HttpGet("{questionSetId}")]
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
