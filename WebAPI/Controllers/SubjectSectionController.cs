using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.ViewModels;

namespace WebAPI.Controllers
{
    [Route("api/v0/subjectsections")]
    [ApiController]
    [Authorize(Roles = "0,1")] //chỉ 0
    public class SubjectSectionController : ControllerBase
    {
        private readonly ISubjectSectionServices _subjectSectionServices;
        private readonly IUserServices _userServices;

        public SubjectSectionController(ISubjectSectionServices subjectSectionServices, IUserServices userServices)
        {
            _subjectSectionServices = subjectSectionServices;
            _userServices = userServices;
        }

        [HttpGet("{subjectId}/{grade}")]
        [Authorize]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllBySubjectIdAndGrade(int subjectId, int grade)
        {
            try
            {
                var subjectSections = await _subjectSectionServices.GetAllBySubjectIdAndGrade(subjectId, grade);

                return Ok(subjectSections);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpGet("{sectionId}")]
        [Authorize]
        public async Task<IActionResult> GetSectionBySectionId(Guid sectionId)
        {
            try
            {
                var section = await _subjectSectionServices.GetSectionBySectionId(sectionId);

                return Ok(section);
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
        public async Task<IActionResult> AddSubjectSection(SubjectSectionCreate subjectSectionCreate)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                
                var currentUser = await _userServices.GetCurrentUser();

                var result = await _subjectSectionServices.AddSubjectSection(subjectSectionCreate, currentUser);

                if (result == false)
                {
                    return BadRequest(new
                    {
                        Message = "Thêm mới chương thất bại"
                    });
                }

                return Ok("Đã thêm chương thành công");
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
        [AllowAnonymous]
        public async Task<IActionResult> UpdateSubjectSection(SubjectSectionUpdate subjectSectionUpdate)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var currentUser = await _userServices.GetCurrentUser();

                var result = await _subjectSectionServices.UpdateSubjectSection(subjectSectionUpdate, currentUser);

                if (result == false)
                {
                    return BadRequest(new
                    {
                        Message = "Cập nhật chương thất bại"
                    });
                }

                return Ok("Đã cập nhật chương thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpDelete("{sectionId}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteSubjectSection(Guid sectionId)
        {
            try
            {
                var result = await _subjectSectionServices.DeleteSubjectSection(sectionId);

                if (result == false)
                {
                    return BadRequest(new
                    {
                        Message = "Xóa chương thất bại"
                    });
                }

                return Ok("Đã xóa chương thành công");
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
