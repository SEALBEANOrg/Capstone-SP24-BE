using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces.Student;
using Services.Interfaces.User;
using Services.ViewModels;
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.Controllers
{
    [Route("api/v0/students")]
    [ApiController]
    [Authorize(Roles = "1")]
    public class StudentControllers : ControllerBase
    {
        private readonly IStudentServices _studentServices;
        private readonly IUserServices _userServices;

        public StudentControllers(IStudentServices studentServices, IUserServices userServices)
        {
            _studentServices = studentServices;
            _userServices = userServices;
        }

        [HttpGet("{studentId}")]
        [SwaggerResponse(200, "Sample student", typeof(StudentViewModels))]
        public async Task<IActionResult> GetStudentById(Guid studentId)
        {
            try
            {
                var student = await _studentServices.GetStudentById(studentId);

                return Ok(student);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpPut]
        [SwaggerResponse(200, "Is success", typeof(bool))]
        public async Task<IActionResult> UpdateStudent(StudentUpdate studentUpdate)
        {
            try
            {
                var currentUser = await _userServices.GetCurrentUser();
                var student = await _studentServices.UpdateStudent(studentUpdate, currentUser);

                return Ok(student);
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
