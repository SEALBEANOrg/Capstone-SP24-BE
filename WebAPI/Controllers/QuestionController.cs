using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.ViewModels;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionServices _questionServices;
        private readonly IUserServices _userServices;

        [HttpPost("AddQuestion")]
        public async Task<IActionResult> AddQuestion([FromBody] QuestionCreate questionCreate)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var currentUser = await _userServices.GetCurrentUser();

                var result = await _questionServices.AddQuestions(questionCreate, currentUser);

                if (!result)
                {
                    return BadRequest(new
                    {
                        Message = "Add Question thất bại"
                    });
                }

                return Ok("Đã thêm câu hỏi mới");
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }


        [HttpGet("GetAllQuestion")]
        public async Task<IActionResult> GetAllQuestion()
        {
            try
            {
                var questions = await _questionServices.GetAllQuestion();

                return Ok(questions);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpGet("GetQuestionByQuestionId/{questionId}")]
        public async Task<IActionResult> GetQuestionByQuestionId(Guid questionId)
        {
            try
            {
                var question = await _questionServices.GetQuestionByQuestionId(questionId);

                return Ok(question);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpPut("UpdateQuestion")]
        public async Task<IActionResult> UpdateQuestion([FromBody] QuestionUpdate questionUpdate)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var currentUser = await _userServices.GetCurrentUser();

                var result = await _questionServices.UpdateQuestion(questionUpdate, currentUser);

                if (!result)
                {
                    return BadRequest(new
                    {
                        Message = "Update Question thất bại"
                    });
                }

                return Ok("Cập nhật thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpDelete("DeleteQuestion")]
        public async Task<IActionResult> DeleteQuestion(Guid questionId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var currentUser = await _userServices.GetCurrentUser();

                var result = await _questionServices.DeleteQuestion(questionId, currentUser);

                if (!result)
                {
                    return BadRequest(new
                    {
                        Message = "Delete Question thất bại"
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
    }


}
