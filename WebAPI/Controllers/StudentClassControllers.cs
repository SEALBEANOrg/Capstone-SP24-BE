﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces.Student;
using Services.Interfaces.StudentClass;
using Services.Interfaces.User;
using Services.ViewModels;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Controllers
{
    [Route("api/v0/studentclasses")]
    [ApiController]
    [Authorize(Roles = "1, 2")]
    public class StudentClassControllers : ControllerBase
    {
        private readonly IStudentClassServices _studentClassServices;
        private readonly IStudentServices _studentServices;
        private readonly IUserServices _userServices;

        public StudentClassControllers(IUserServices userServices, IStudentClassServices studentClassServices, IStudentServices studentServices)
        {
            _studentClassServices = studentClassServices;
            _studentServices = studentServices;
            _userServices = userServices;
        }

        [HttpGet("own-class")]
        [SwaggerResponse(200, "List of sample student classes", typeof(IEnumerable<StudentClassViewModels>))]
        public async Task<IActionResult> GetAllByCreator([Required] string studyYear)
        {
            var currentUserId = await _studentClassServices.GetCurrentUser();
            var studentClasses = await _studentClassServices.GetAllStudentClass(studyYear, currentUserId);

            return Ok(studentClasses);
        }

        [HttpGet]
        [Authorize(Roles = "2")] // Expert
        [SwaggerResponse(200, "List of sample student classes", typeof(IEnumerable<StudentClassViewModels>))]
        public async Task<IActionResult> GetAll(Guid? teacherId, [Required] string studyYear)
        {
            try
            {
                var studentClasses = await _studentClassServices.GetAllStudentClass(studyYear, teacherId);

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
        

        [HttpGet("{classId}")]
        [SwaggerResponse(200, "Sample student classes", typeof(ClassInfo))]
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


        [HttpGet("{classId}/students")]
        [SwaggerResponse(200, "List of sample student", typeof(IEnumerable<StudentViewModels>))]
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
        

        [HttpPost("{classId}/students")]
        [SwaggerResponse(200, "Is success", typeof(bool))]
        public async Task<IActionResult> AddStudentIntoClass(Guid classId, [FromBody] StudentCreate studentClassCreate)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _studentClassServices.AddStudentIntoClass(classId, studentClassCreate);

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


        [HttpDelete("students/{studentId}")]
        [SwaggerResponse(200, "Is success", typeof(bool))]
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


        [HttpPost]
        [SwaggerResponse(200, "Is success", typeof(bool))]
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


        [HttpPut("{classId}")]
        [SwaggerResponse(200, "Is success", typeof(bool))]
        public async Task<IActionResult> UpdateClass(Guid classId, [FromBody] StudentClassUpdate studentClassUpdate)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _studentClassServices.UpdateStudentClass(classId, studentClassUpdate);

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


        [HttpDelete("{classId}")]
        [SwaggerResponse(200, "Is success", typeof(bool))]
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


        [HttpPut("{classId}/status")]
        [SwaggerResponse(200, "Is success", typeof(bool))]
        public async Task<IActionResult> UpdateStatusOfClass(Guid classId, int status)
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


        [HttpPost("{classId}/import-students")]
        [SwaggerResponse(200, "Is success", typeof(IEnumerable<StudentViewModels>))]
        [AllowAnonymous]
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


        [HttpGet("combo-class")]
        [SwaggerResponse(200, "Combo Class", typeof(IEnumerable<ComboClass>))]
        public async Task<IActionResult> GetComboClass(int? grade)
        {
            try
            {
                var currentUserId = await _userServices.GetCurrentUser();
                var result = await _studentClassServices.GetComboClass(currentUserId, grade);

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
