using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Interfaces.Exam;
using Services.Interfaces.User;
using Services.ViewModels;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Controllers
{
    [Route("api/v0/exams")]
    [ApiController]
    [Authorize]
    public class ExamController : ControllerBase
    {
        private readonly IExamServices _testResultServices;
        private readonly IExamMobileServices _examMobileServices;
        private readonly IExamMarkServices _examMarkServices;
        private readonly IUserServices _userServices;
        
        public ExamController(IExamServices testResultServices, IExamMarkServices examMarkServices, IExamMobileServices examMobileServices, IUserServices userServices)
        {
            _testResultServices = testResultServices;
            _examMobileServices = examMobileServices;
            _examMarkServices = examMarkServices;
            _userServices = userServices;
        }

        //mobile
        [AllowAnonymous]
        [HttpPost("send-image")]
        [SwaggerResponse(200, "sample result", typeof(ResultForScan))]
        public async Task<IActionResult> SendImage([FromBody] ResultForScanViewModel Image)
        {
            try
            {
                if (string.IsNullOrEmpty(Image.image))
                {
                    return BadRequest();
                }
                var result = await _examMobileServices.SendImage(Image);

                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest("Gửi ảnh thất bại");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //mobile
        [AllowAnonymous]
        [HttpPost("save-result")]
        [SwaggerResponse(200, "IsSuccess", typeof(bool))]
        public async Task<IActionResult> SaveResultOfTest([FromBody] ResultToSave resultToSave)
        {
            try
            {
                var result = await _examMobileServices.SaveResult(resultToSave);

                if (result == -2)
                {
                    return NotFound("Mã đề thi không hợp lệ");
                }

                return Ok(result > 0);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        //mobile
        [AllowAnonymous]
        [HttpGet("access-exam/{testCode}/{email}")]
        [SwaggerResponse(200, "sample info of class in exam", typeof(InfoClassInExam))]
        public async Task<IActionResult> GetInfoOfClassInExam(string testCode, string email)
        {
            if (string.IsNullOrEmpty(testCode) || string.IsNullOrEmpty(email))
            {
                return BadRequest();
            }

            try
            {
                var result = await _examMobileServices.GetInfoOfClassInExam(testCode, email);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(new
                {
                    Message = ex.Message
                });

            }
        }

        [HttpGet("own-exam")]
        [SwaggerResponse(200, "sample result", typeof(IEnumerable<ExamViewModels>))]
        [Authorize(Roles = "1,2")] // Teacher, Expert
        public async Task<IActionResult> GetOwnExam(int? grade,[Required] string studyYear)
        {
            try
            {
                var currentUserId = await _userServices.GetCurrentUser();
                var result = await _testResultServices.GetOwnExam(currentUserId, grade, studyYear);

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


        [HttpGet("{examId}")]
        [Authorize(Roles = "1,2")] // Teacher, Expert
        [SwaggerResponse(200, "sample exam info", typeof(ExamInfo))]
        public async Task<IActionResult> GetExamInfo(Guid examId)
        {
            try
            {
                var currentUserId = await _userServices.GetCurrentUser();
                var result = await _testResultServices.GetExamInfo(examId, currentUserId);

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

        [HttpPost("matrix")]
        [Authorize(Roles = "1,2")] // Teacher, Expert
        //[AllowAnonymous]
        [SwaggerResponse(200, "Exam ID", typeof(string))]
        public async Task<IActionResult> AddExamByMatrixIntoClass(ExamCreate examCreate)
        {
            try
            {
                var currentUserId = await _userServices.GetCurrentUser();
                var result = await _testResultServices.AddExamByMatrixIntoClass(examCreate, currentUserId);

                if (result == null)
                {
                    return BadRequest(new
                    {
                        Message = "Thêm thất bại"
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


        [HttpGet("{examId}/resource")]
        [Authorize(Roles = "1,2")] // Teacher, Expert
        [SwaggerResponse(200, "List of sample exam sources", typeof(IEnumerable<ExamSourceViewModel>))]
        public async Task<IActionResult> GetAllExamSource(Guid examId)
        {
            try
            {
                var result = await _testResultServices.GetAllExamSource(examId);

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


        [HttpGet("export-result")]
        [Authorize(Roles = "1,2")] // Teacher, Expert
        //[AllowAnonymous]
        [SwaggerResponse(200, "Export result", typeof(ExportResult))]
        public async Task<IActionResult> ExportResult(Guid examId)
        {
            try
            {
                var result = await _testResultServices.ExportResult(examId);
                return File(result.Bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.FileName);

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }


        [HttpPut("{examId}/calculate-all")]
        [Authorize(Roles = "1,2")] // Teacher, Expert
        [SwaggerResponse(200, "exam info", typeof(ExamInfo))]
        public async Task<IActionResult> CalculateAllMark(Guid examId)
        {
            try
            {
                var currentUserId = await _userServices.GetCurrentUser();
                var result = await _examMarkServices.CalculateAllMark(examId, currentUserId);

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
