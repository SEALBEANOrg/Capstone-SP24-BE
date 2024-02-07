using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.ViewModels;

namespace WebAPI.Controllers
{
    [Route("api/v0/schools")]
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
        [HttpGet("combo-school")]
        [Authorize]
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
        [HttpGet("teachers")]
        [Authorize(Roles = "2")]
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
        [HttpGet("classes")]
        [Authorize(Roles = "1,2")]
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
        [HttpPut("teachers/{teacherId}/move-out-school")]
        [Authorize(Roles = "2")]
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

        [HttpPost]
        [Authorize(Roles = "0")] //chỉ 0 
        public async Task<IActionResult> AddNewSchool([FromBody] SchoolForCreateViewModel schoolForCreateViewModel)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var currentUserId = await _userServices.GetCurrentUser();
                var result = await _schoolServices.AddNewSchool(schoolForCreateViewModel, currentUserId);

                if (result == false)
                {
                    return BadRequest(new
                    {
                        Message = "Thêm mới trường học thất bại"
                    });
                }

                return Ok("Đã thêm trường thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }


        [HttpGet]
        [Authorize(Roles = "0")] //chỉ 0
        public async Task<IActionResult> GetAllSchool(string? search, int status = 1)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var schools = await _schoolServices.GetAllSchool(search, status);

                return Ok(schools);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }


        [HttpGet("{schoolId}")]
        [Authorize(Roles = "0,1,2")] //chỉ 0
        public async Task<IActionResult> GetSchoolById(Guid schoolId)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var school = await _schoolServices.GetSchoolById(schoolId);

                return Ok(school);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        //[HttpGet("GetInfoMySchool")]
        //[Authorize(Roles = "1,2")]
        //public async Task<IActionResult> GetInfoMySchool()
        //{
        //    try
        //    {

        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }

        //        var currentUserId = await _userServices.GetCurrentUser();

        //        var school = await _schoolServices.GetInfoMySchool(currentUserId);

        //        return Ok(school);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new
        //        {
        //            Message = ex.Message
        //        });
        //    }
        //}

        [HttpPut]
        [Authorize(Roles = "0")] //chỉ 0
        public async Task<IActionResult> UpdateSchool([FromBody] SchoolForUpdateViewModel schoolForUpdateViewModel)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _schoolServices.UpdateSchool(schoolForUpdateViewModel);

                if (result == false)
                {
                    return BadRequest(new
                    {
                        Message = "Cập nhật trường học thất bại"
                    });
                }

                return Ok("Đã cập nhật trường thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpDelete("{schoolId}")]
        [Authorize(Roles = "0")] //chỉ 0
        public async Task<IActionResult> DeleteSchool(Guid schoolId)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _schoolServices.DeleteSchool(schoolId);

                if (result == false)
                {
                    return BadRequest(new
                    {
                        Message = "Xóa trường học thất bại"
                    });
                }

                return Ok("Đã xóa trường thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpPut("{schoolId}/admin/{email}")]
        [Authorize(Roles = "0")] //chỉ 0
        public async Task<IActionResult> ChangeSchoolAdmin(Guid schoolId, string email)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var currentUserId = await _userServices.GetCurrentUser();

                var result = await _schoolServices.ChangeSchoolAdmin(schoolId, email, currentUserId);

                if (result == false)
                {
                    return BadRequest(new
                    {
                        Message = "Thay đổi quyền quản trị thất bại"
                    });
                }

                return Ok("Đã thay đổi quyền quản trị thành công");
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
