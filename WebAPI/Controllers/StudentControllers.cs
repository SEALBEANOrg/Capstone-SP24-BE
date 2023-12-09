using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.ViewModels;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentControllers : ControllerBase
    {
        private readonly IStudentServices _studentServices;
        private readonly IUserServices _userServices;

        public StudentControllers(IStudentServices studentServices, IUserServices userServices)
        {
            _studentServices = studentServices;
            _userServices = userServices;
        }

        [HttpGet("GetStudentById/{studentId}")]
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

        [HttpPut("UpdateStudent")]
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

        /// <summary>
        /// Just for school Admin
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        [HttpGet("MoveOutStudent")]
        public async Task<IActionResult> MoveOutStudent(Guid studentId)
        {
            try
            {
                var currentUser = await _userServices.GetCurrentUser();
                var student = await _studentServices.MoveOutStudent(studentId, currentUser);

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
