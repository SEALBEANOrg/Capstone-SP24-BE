using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.ViewModels;
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.Controllers
{
    [Route("api/v0/subjects")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectServices _subjectServices;
        private readonly IUserServices _userServices;

        public SubjectController(ISubjectServices subjectServices, IUserServices userServices)
        {
            _subjectServices = subjectServices;
            _userServices = userServices;
        }

        [HttpGet]
        [Authorize(Roles = "0,1,2")]
        [SwaggerResponse(200, "List sample subject", typeof(IEnumerable<SubjectViewModels>))]
        public async Task<IActionResult> GetAll(int? subjectEnum, int? grade)
        {
            try
            {
                var subjects = await _subjectServices.GetAll(subjectEnum, grade);

                return Ok(subjects);
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
