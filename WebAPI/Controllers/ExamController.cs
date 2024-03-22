using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Services.Interfaces;
using Services.ViewModels;
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.Controllers
{
    [Route("api/v0/exams")]
    [ApiController]
    [Authorize]
    public class ExamController : ControllerBase
    {
        private readonly IExamServices _testResultServices;
        private readonly IUserServices _userServices;
        private readonly IHttpClientFactory _httpClientFactory;
        
        public ExamController(IExamServices testResultServices, IUserServices userServices, IHttpClientFactory httpClientFactory)
        {
            _testResultServices = testResultServices;
            _userServices = userServices;
            _httpClientFactory = httpClientFactory;
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
                var resp = await _testResultServices.SendImage(Image);

                if (resp != null)
                {
                    return Ok(new ResultForScan { image = resp.image, result = resp.result, paperCode = resp.paper_code, studentNo = resp.student_no});
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
        [SwaggerResponse(200, "Sample mark", typeof(decimal))]
        public async Task<IActionResult> SaveResultOfTest([FromBody] ResultToSave resultToSave)
        {
            try
            {
                var result = await _testResultServices.SaveResult(resultToSave);

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

        //mobile
        [AllowAnonymous]
        [HttpGet("check-permission/{testCode}/{email}")]
        [SwaggerResponse(200, "Permission", typeof(bool))]
        public async Task<IActionResult> CheckPermissionAccessTest(string testCode, string email)
        {
            if (string.IsNullOrEmpty(testCode) || string.IsNullOrEmpty(email))
            {
                return BadRequest();
            }

            var result = await _testResultServices.CheckPermissionAccessTest(testCode, email);

            return Ok(result);
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
                var result = await _testResultServices.GetInfoOfClassInExam(testCode, email);

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
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> GetOwnExam(int? grade)
        {
            try
            {
                var currentUserId = await _userServices.GetCurrentUser();
                var result = await _testResultServices.GetOwnExam(currentUserId, grade);

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
        [Authorize(Roles = "1,2")]
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
        [Authorize(Roles = "1,2")]
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
                    return NotFound(new
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
        [Authorize(Roles = "1,2")]
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

        [HttpGet("paper/{paperId}")]
        [AllowAnonymous]
        [SwaggerResponse(200, "url", typeof(string))]
        public async Task<IActionResult> GetPaperById(Guid paperId)
        {
            try
            {
                string urlS3 = await _testResultServices.GetPaperById(paperId);
                // return file to client side to download
                return Ok(urlS3);

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpGet("expor-result")]
        [Authorize(Roles = "1,2")]
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

    }
}
