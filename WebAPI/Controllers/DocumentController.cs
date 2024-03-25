using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces.Document;
using Services.Interfaces.User;
using Services.ViewModels;
using Swashbuckle.AspNetCore.Annotations;

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
        //[AllowAnonymous]
        [SwaggerResponse(200, "sample document", typeof(IEnumerable<DocumentViewModels>))]
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
        //[AllowAnonymous]
        [SwaggerResponse(200, "sample document", typeof(DocumentViewModel))]
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
        //[AllowAnonymous]
        [SwaggerResponse(200, "Is success", typeof(string))]
        public async Task<IActionResult> AddDocument([FromForm] DocumentCreate documentCreate)
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

        //[HttpPost("test/create-test-paper")]
        //[AllowAnonymous]
        //[SwaggerResponse(200, "Is success", typeof(File))]
        //public async Task<IActionResult> CreateTestPaper([FromBody] DetailOfPaper detailOfPaper)
        //{
        //    try
        //    {
        //        var currentUserId = await _userServices.GetCurrentUser();
        //        var result = await _documentServices.CreateTestPaper(currentUserId, detailOfPaper);
        //        return File(result, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "ModifiedFile.docx");

        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new
        //        {
        //            Message = ex.Message
        //        });
        //    }
        //}

        [HttpDelete("{documentId}")]
        [Authorize(Roles = "0")]
        //[AllowAnonymous]
        [SwaggerResponse(200, "Is success", typeof(string))]
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
