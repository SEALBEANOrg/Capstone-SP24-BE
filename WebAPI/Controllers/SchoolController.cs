using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces.School;
using Services.Interfaces.User;
using Services.ViewModels;
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.Controllers
{
    [Route("api/v0/schools")]
    [ApiController]
    public class SchoolController : ControllerBase
    {
        private readonly ISchoolServices _schoolServices;
        private readonly IUserServices _userServices;

        public SchoolController(ISchoolServices schoolServices, IUserServices userServices)
        {
            _schoolServices = schoolServices;
            _userServices = userServices;
        }


        [HttpGet]
        [Authorize(Roles = "0")] //chỉ 0
        [SwaggerResponse(200, "List of sample schools", typeof(IEnumerable<SchoolList>))]
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
        [SwaggerResponse(200, "Sample school", typeof(SchoolViewModels))]
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


        [HttpGet("dropdown")]
        [Authorize]
        [SwaggerResponse(200, "List of dropdown schools", typeof(IEnumerable<DropdownSchools>))]
        public async Task<IActionResult> GetDropdownSchools()
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var schools = await _schoolServices.GetDropdownSchools();

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


        [HttpPost]
        [Authorize(Roles = "0")]
        [SwaggerResponse(200, "Is success", typeof(string))]
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


        [HttpPut]
        [Authorize(Roles = "0")] //chỉ 0
        [SwaggerResponse(200, "Is success", typeof(string))]
        public async Task<IActionResult> UpdateSchool([FromBody] SchoolForUpdateViewModel schoolForUpdateViewModel)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var currentUserId = await _userServices.GetCurrentUser();

                var result = await _schoolServices.UpdateSchool(schoolForUpdateViewModel, currentUserId);

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


        [HttpPut("{schoolId}/change-status")]
        [Authorize(Roles = "0")]
        [SwaggerResponse(200, "Is success", typeof(string))]
        public async Task<IActionResult> ChangeStatusSchool(Guid schoolId, [FromBody] ChangeStatusViewModel statusSchool)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var currentUserId = await _userServices.GetCurrentUser();

                var result = await _schoolServices.ChangeStatus(schoolId, statusSchool, currentUserId);

                if (result == false)
                {
                    return BadRequest(new
                    {
                        Message = "Thay đổi trạng thái trường học thất bại"
                    });
                }

                return Ok("Đã thay đổi trạng thái trường thành công");
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
        [SwaggerResponse(200, "Is success", typeof(string))]
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

    }
}
