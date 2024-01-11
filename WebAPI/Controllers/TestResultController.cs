using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.ViewModels;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "1")]
    public class TestResultController : ControllerBase
    {
        private readonly ITestResultServices _testResultServices;

        public TestResultController(ITestResultServices testResultServices)
        {
            _testResultServices = testResultServices;
        }

        [AllowAnonymous]
        [HttpPost("SendImage")]
        public async Task<IActionResult> SendImage([FromBody] ResultForScanViewModel base64Image)
        {
            if (string.IsNullOrEmpty(base64Image.Base64Image))
            {
                return BadRequest();
            }

            //code for call python script
            

            return Ok(new ResultForScan { Base64Image = base64Image.Base64Image, ResultString = "1:A|2:B|3:D|4:|5:B,C|6:A,B,C,D" });
        }

        [AllowAnonymous]
        [HttpPost("SaveResultOfTest")]
        public async Task<IActionResult> SaveResultOfTest([FromBody] string result, string codeExam, Guid testId)
        {
            if (result == null || codeExam == null || testId == null)
            {
                return BadRequest();
            }

            


            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("CheckPermissionAccessTest/{testCode}/{email}")]
        public async Task<IActionResult> CheckPermissionAccessTest(string testCode, string email)
        {
            if (string.IsNullOrEmpty(testCode) || string.IsNullOrEmpty(email))
            {
                return BadRequest();
            }

            var result = await _testResultServices.CheckPermissionAccessTest(testCode, email);

            return Ok(result);
        }

    }
}
