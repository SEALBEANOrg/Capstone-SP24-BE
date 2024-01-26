using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.ViewModels;
using System.Diagnostics;

namespace WebAPI.Controllers
{
    [Route("api/v0/questions")]
    [ApiController]
    [Authorize]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionServices _questionServices;
        private readonly IUserServices _userServices;

        public QuestionController(IQuestionServices questionServices, IUserServices userServices)
        {
            _questionServices = questionServices;
            _userServices = userServices;
        }

        [Authorize(Roles = "1,2,3")]
        [HttpPost("private-bank")]
        public async Task<IActionResult> AddQuestionIntoPrivateBank([FromBody] QuestionCreate questionCreate)
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


        [Authorize(Roles = "1,2,3")]
        [HttpGet("private-bank/{grade}")]
        public async Task<IActionResult> GetAllMyQuestionByGrade(int grade)
        {
            try
            {
                var currentUserId = await _userServices.GetCurrentUser();
                var questions = await _questionServices.GetAllMyQuestionByGrade(grade, currentUserId);

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

        [Authorize(Roles = "1,2,3")]
        [HttpGet("valid-question/{grade}")]
        public async Task<IActionResult> GetAllValidQuestionByGradeForMe(int grade, int? subject, Guid? sectionId)
        {
            try
            {
                var currentUserId = await _userServices.GetCurrentUser();
                var questions = await _questionServices.GetAllValidQuestionByGradeForMe(subject, grade, sectionId, currentUserId);

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

        [Authorize(Roles = "1,2,3")]
        [HttpGet("{grade}/{questionId}")]
        public async Task<IActionResult> GetQuestionByQuestionIdAndGrade(int grade, Guid questionId)
        {
            try
            {
                var currentUserId = await _userServices.GetCurrentUser();
                var question = await _questionServices.GetQuestionByQuestionIdAndGrade(questionId, grade, currentUserId);

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

        [Authorize(Roles = "1,2,3")]
        [HttpPut("{grade}")]
        public async Task<IActionResult> UpdateQuestion([FromBody] QuestionUpdate questionUpdate, int grade)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var currentUser = await _userServices.GetCurrentUser();

                var result = await _questionServices.UpdateQuestion(questionUpdate, currentUser, grade);

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

        [Authorize(Roles = "1,2,3")]
        [HttpDelete("{grade}/{questionId}")]
        public async Task<IActionResult> DeleteQuestion(int grade, Guid questionId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var currentUser = await _userServices.GetCurrentUser();

                var result = await _questionServices.DeleteQuestion(questionId, currentUser, grade);

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
