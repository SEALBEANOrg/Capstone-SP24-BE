using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.ViewModels;
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.Controllers
{
    [Route("api/v0/subjectsections")]
    [ApiController]
    [Authorize] 
    public class SubjectSectionController : ControllerBase
    {
        private readonly ISubjectSectionServices _subjectSectionServices;
        private readonly IUserServices _userServices;
        private readonly ISubjectServices _subjectServices;

        public SubjectSectionController(ISubjectSectionServices subjectSectionServices, IUserServices userServices, ISubjectServices subjectServices)
        {
            _subjectSectionServices = subjectSectionServices;
            _userServices = userServices;
            _subjectServices = subjectServices;
        }

        [HttpGet]
        [Authorize(Roles = "0,1,2")]
        [SwaggerResponse(200, "List sample section", typeof(IEnumerable<SubjectSectionViewModels>))]
        public async Task<IActionResult> GetAllBySubjectId(Guid? subjectId)
        {
            try
            {
                var subjectSections = await _subjectSectionServices.GetAllBySubjectId(subjectId);

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
        [Authorize(Roles = "0")]
        [SwaggerResponse(200, "Sample section", typeof(SubjectSectionViewModel))]
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
        [Authorize(Roles = "0")]
        [SwaggerResponse(200, "Is success", typeof(string))]
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
        [Authorize(Roles = "0")]
        [SwaggerResponse(200, "Is success", typeof(string))]
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
        [Authorize(Roles = "0")]
        [SwaggerResponse(200, "Is success", typeof(string))]
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

        [HttpGet("nav")]
        [Authorize(Roles = "0,1,2")]
        [SwaggerResponse(200, "List sample section", typeof(IEnumerable<SubjectSectionNav>))]
        public async Task<IActionResult> GetAllBySubjectEnumAndGrade(int grade, int subjectEnum)
        {
            try
            {
                var subject = (await _subjectServices.GetAll(subjectEnum, grade)).First();
                if (subject == null)
                {
                    return BadRequest(new
                    {
                        Message = "Không tìm thấy môn học"
                    });
                }

                var currentUser = await _userServices.GetCurrentUser();
                var subjectSections = await _subjectSectionServices.GetAllBySubjectIdForNav(subject.SubjectId, currentUser);

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
    }
}
