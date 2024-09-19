using Microsoft.AspNetCore.Mvc;
using PhotoStudio.Domain.Interfaces;
using System.Threading.Tasks;

namespace PhotoStudio.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentsController(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        // Nova akcija za simulaciju plaćanja
        [HttpPost("simulate-payment")]
        public async Task<IActionResult> SimulatePayment(int bookingId, decimal amount)
        {
            var result = await _paymentRepository.SimulatePayment(bookingId, amount);

            if (result)
            {
                return Ok("Payment simulation successful.");
            }
            else
            {
                return BadRequest("Insufficient advance or booking not found.");
            }
        }
    }
}
