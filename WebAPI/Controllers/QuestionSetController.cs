﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces.QuestionSet;
using Services.Interfaces.User;
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
        [Authorize(Roles = "1,2")] // Teacher, Expert
        public async Task<IActionResult> GetOwnQuestionSet(int? grade, int? subject, [Required]string studyYear)
        {
            try
            {
                var currentUser = await _userServices.GetCurrentUser();
                var result = await _questionSetServices.GetOwnQuestionSet(currentUser, grade, subject, studyYear);

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


        [HttpGet("shared")]
        [Authorize(Roles = "1")] // Teacher
        [SwaggerResponse(200, "List of shared question set", typeof(IEnumerable<SharedQuestionSet>))]
        public async Task<IActionResult> GetSharedQuestionSet(int? grade, int? subjectEnum, string studyYear)
        {
            try
            {
                var currentUserId = await _userServices.GetCurrentUser();
                var result = await _questionSetServices.GetSharedQuestionSet(currentUserId, grade, subjectEnum, studyYear);

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
        [Authorize(Roles = "2")] // Expert
        public async Task<IActionResult> GetQuestionSetByBank(int? grade, int? subject, [Required]string studyYear, int type)
        {
            try
            {
                var result = await _questionSetServices.GetQuestionSetBank(grade, subject, studyYear, type);

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
        [Authorize(Roles = "1,2")] // Teacher, Expert
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
        [Authorize(Roles = "1,2")] // Teacher, Expert
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
        [Authorize(Roles = "1,2")] // Teacher, Expert
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
        [Authorize(Roles = "1,2")] // Teacher, Expert 
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
        [SwaggerResponse(200, "Is success", typeof(string))]
        [Authorize(Roles = "1,2")] // Teacher, Expert
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


        [HttpGet("matrix-of-questionset/{questionSetId}")]
        [SwaggerResponse(200, "List of matrix", typeof(IEnumerable<SectionUse>))]
        [Authorize(Roles = "1,2")] // Teacher, Expert
        public async Task<IActionResult> GetMatrixOfQuestionSet(Guid questionSetId)
        {
            try
            {
                var result = await _questionSetServices.GetMatrixOfQuestionSet(questionSetId);

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
