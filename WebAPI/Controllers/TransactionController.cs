using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces.Transaction;
using Services.Interfaces.User;
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

        [HttpGet("buy-point")]
        //[Authorize]
        [AllowAnonymous]
        [SwaggerResponse(200, "url redirect to momo", typeof(string))]
        public async Task<IActionResult> MomoReturn([FromQuery] TransactionPoint transaction)
        {
            try
            {
                var currentUser = await _userServices.GetCurrentUser();
                var returnUrl = await _transactionServices.CreatePaymentAsync(transaction, currentUser);

                if (returnUrl == null)
                {
                    return BadRequest(new
                    {
                        Message = "Tạo thanh toán thất bại"
                    });
                }

                return Ok(returnUrl);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpGet("momo/callback-redirect")]
        [AllowAnonymous]
        [SwaggerResponse(200, "Is success", typeof(string))]
        public async Task<IActionResult> MomoCallBackRedirect([FromQuery] CallbackViaMomo transaction)
        {
            try
            {
                var result = await _transactionServices.MomoCallBackRedirect(transaction);

                if (result == false)
                {
                    return BadRequest(new
                    {
                        Message = "Thanh toán thất bại"
                    });
                }

                return Ok("Thanh toán thành công");
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
