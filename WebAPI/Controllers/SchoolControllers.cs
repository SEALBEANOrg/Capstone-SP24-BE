using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolControllers : ControllerBase
    {
        private readonly ISchoolServices _schoolServices;
        private readonly IUserServices _userServices;

        public SchoolControllers(ISchoolServices schoolServices, IUserServices userServices)
        {
            _schoolServices = schoolServices;
            _userServices = userServices;
        }

        /// <summary>
        /// Get Combo School to Select to create new class
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetComboSchool")]
        public async Task<IActionResult> GetComboSchool()
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var studentClasses = await _schoolServices.GetComboSchool();

                return Ok(studentClasses);
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
        /// Just For SchoolAdmin
        /// </summary>
        [HttpGet("GetTeacherOfMySchool")]
        public async Task<IActionResult> GetTeacherOfMySchool()
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var currentUserId = await _userServices.GetCurrentUser();

                var studentClasses = await _schoolServices.GetTeacherOfMySchool(currentUserId);

                return Ok(studentClasses);
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
        /// See all class in my school
        /// </summary>
        [HttpGet("GetAllClassOfMySchool")]
        public async Task<IActionResult> GetAllClassOfMySchool()
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var currentUserId = await _userServices.GetCurrentUser();

                var studentClasses = await _schoolServices.GetAllClassOfMySchool(currentUserId);

                return Ok(studentClasses);
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
        /// Remove Teacher From School
        /// </summary>
        [HttpPut("RemoveTeacherFromSchool/{teacherId}")]
        public async Task<IActionResult> RemoveTeacherFromSchool(Guid teacherId)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var currentUserId = await _userServices.GetCurrentUser();
                var result = await _schoolServices.RemoveTeacherFromSchool(teacherId, currentUserId);
            
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
