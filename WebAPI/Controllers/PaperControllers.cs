using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces.Paper;
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.Controllers
{
    [Route("api/v0/papers")]
    [ApiController]
    public class PaperControllers : ControllerBase
    {
        private readonly IPaperServices _paperServices;

        public PaperControllers(IPaperServices paperServices)
        {
            _paperServices = paperServices;
        }

        [HttpGet("{paperId}")]
        [AllowAnonymous]
        [SwaggerResponse(200, "url", typeof(string))]
        public async Task<IActionResult> GetPaperById(Guid paperId)
        {
            try
            {
                string urlS3 = await _paperServices.GetPaperById(paperId);
                return Ok(urlS3);
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
