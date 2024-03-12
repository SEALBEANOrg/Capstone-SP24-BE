using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.ViewModels;
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.Controllers
{
    [Route("api/v0/transactions")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionServices _transactionServices;
        private readonly IUserServices _userServices;

        public TransactionController(ITransactionServices transactionServices, IUserServices userServices)
        {
            _transactionServices = transactionServices;
            _userServices = userServices;
        }

        [HttpGet]
        [Authorize(Roles = "0")]
        [SwaggerResponse(200, "List of my transactions", typeof(IEnumerable<TransactionViewModels>))]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var transactions = await _transactionServices.GetAll();

                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpGet("mine")]
        [Authorize(Roles = "1")]
        [SwaggerResponse(200, "List of my transactions", typeof(IEnumerable<TransactionViewModels>))]
        public async Task<IActionResult> GetMyTransactions()
        {
            try
            {
                var currentUser = await _userServices.GetCurrentUser();
                var transactions = await _transactionServices.GetMyTransactions(currentUser);

                return Ok(transactions);
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
