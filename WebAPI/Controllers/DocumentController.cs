using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Services;
using Services.ViewModels;
using static System.Collections.Specialized.BitVector32;

namespace WebAPI.Controllers
{
    [Route("api/v0/documents")]
    [ApiController]
    [Authorize]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentServices _documentServices;
        private readonly IUserServices _userServices;

        public DocumentController(IDocumentServices documentServices, IUserServices userServices)
        {
            _documentServices = documentServices;
            _userServices = userServices;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDocument(int? type)
        {
            try
            {
                var documents = await _documentServices.GetAllDocument(type);

                return Ok(documents);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                }); 
            }
        }

        [HttpGet("{documentId}")]
        public async Task<IActionResult> GetDocumentById(Guid documentId)
        {
            try
            {
                var document = await _documentServices.GetDocumentById(documentId);

                return Ok(document);
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
        public async Task<IActionResult> AddDocument([FromBody] DocumentCreate documentCreate)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var currentUser = await _userServices.GetCurrentUser();

                var result = await _documentServices.AddDocument(documentCreate, currentUser);

                if (!result)
                {
                    return BadRequest(new
                    {
                        Message = "Add Document thất bại"
                    });
                }

                return Ok("Đã thêm tài liệu mới");
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpDelete("{documentId}")]
        [Authorize(Roles = "0")]
        public async Task<IActionResult> DeleteDocument(Guid documentId)
        {
            try
            {
                var result = await _documentServices.DeleteDocument(documentId);

                if (result == false)
                {
                    return BadRequest(new
                    {
                        Message = "Xóa document thất bại"
                    });
                }

                return Ok("Đã xóa document thành công");
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
