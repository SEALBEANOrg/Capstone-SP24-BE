using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.ViewModels;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "1, 2")]
    public class StudentClassControllers : ControllerBase
    {
        private readonly IStudentClassServices _studentClassServices;
        private readonly IStudentServices _studentServices;

        public StudentClassControllers(IStudentClassServices studentClassServices, IStudentServices studentServices)
        {
            _studentClassServices = studentClassServices;
            _studentServices = studentServices;
        }

        [HttpGet("GetClassByCreator/{teacherId}")]
        public async Task<IActionResult> GetAllByCreator(Guid teacherId)
        {
            var studentClasses = await _studentClassServices.GetAllStudentClass(teacherId.ToString());

            return Ok(studentClasses);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var studentClasses = await _studentClassServices.GetAllStudentClass();

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

        [HttpGet("GetStudentsOfClass")]
        public async Task<IActionResult> GetStudentsOfMyClass(Guid classId)
        {
            try
            {
                var currentUser = await _studentClassServices.GetCurrentUser();
                var studentClasses = await _studentServices.GetStudentsOfClass(classId, currentUser);

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

        [HttpGet("GetClassByClassId/{classId}")]
        public async Task<IActionResult> GetById(Guid classId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var studentClass = await _studentClassServices.GetStudentClassById(classId);

                return Ok(studentClass);

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpPost("AddStudentIntoClass")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> AddStudentIntoClass([FromBody] StudentCreate studentClassCreate)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _studentClassServices.AddStudentIntoClass(studentClassCreate);

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

        [HttpDelete("RemoveStudentFromClass/{studentId}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> RemoveStudentFromClass(Guid studentId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _studentClassServices.DeleteStudentFromClass(studentId);

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

        [HttpPost("CreateClass")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> CreateClass([FromBody] StudentClassCreate studentClassCreate)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _studentClassServices.CreateStudentClass(studentClassCreate);

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

        [HttpPut("UpdateClass")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> UpdateClass([FromBody] StudentClassUpdate studentClassUpdate)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _studentClassServices.UpdateStudentClass(studentClassUpdate);

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

        [HttpDelete("DeleteClass/{classId}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> DeleteClass(Guid classId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _studentClassServices.DeleteStudentClass(classId);

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

        [HttpPut("UpdateStatusOfClass/{classId}")]
        public async Task<IActionResult> UpdateStatusOfClass(Guid classId, [FromBody] int status)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _studentClassServices.UpdateStatusOfStudentClass(classId, status);

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

        [HttpPost("ImportExcelToAddStudent/{classId}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> ImportExcelToAddStudent(IFormFile file, Guid classId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _studentClassServices.ImportExcelToAddStudent(classId, file);

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
