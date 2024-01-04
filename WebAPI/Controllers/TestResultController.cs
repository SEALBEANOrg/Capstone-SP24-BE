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

        [AllowAnonymous]
        [HttpPost("SendImage")]
        public async Task<IActionResult> SendImage([FromBody] string base64Image)
        {
            if (string.IsNullOrEmpty(base64Image))
            {
                return BadRequest();
            }

            //code for call python script
            

            return Ok(new ResultForScan { Base64Image = base64Image, ResultString = "1:A|2:B|3:D|4:|5:B,C|6:A,B,C,D" });
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
    }
}
