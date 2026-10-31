using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Razorpay.Api;
using System.Text.Json.Serialization;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;
        private readonly IConfiguration _configuration;

        public PaymentController(PaymentService paymentService, IConfiguration configuration)
        {
            _paymentService = paymentService;
            _configuration = configuration;
        }

        [HttpPost("create-order")]
        public IActionResult CreateOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                Console.WriteLine($"Received create order request: Amount = {request.Amount}");
                var orderId = _paymentService.CreateOrder(request.Amount);
                return Ok(new { id = orderId, amount = request.Amount * 100, currency = "INR" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Create order error: {ex.Message}");
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("test-credentials")]
        public IActionResult TestCredentials()
        {
            try
            {
                var keyId = _configuration["Razorpay:KeyId"] ?? "";
                var keySecret = _configuration["Razorpay:KeySecret"] ?? "";
                
                RazorpayClient client = new RazorpayClient(keyId, keySecret);
                
                // Try to fetch orders to test credentials
                var orders = client.Order.All();
                
                return Ok(new { Success = true, Message = "Credentials are valid" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Error = ex.Message });
            }
        }

        [HttpPost("verify-payment")]
        public IActionResult VerifyPayment([FromBody] VerifyPaymentRequest request)
        {
            try
            {
                var isValid = _paymentService.VerifyPayment(request.OrderId, request.PaymentId, request.Signature);
                
                if (isValid)
                {
                    return Ok(new { Success = true, Message = "Payment verified successfully" });
                }
                else
                {
                    return BadRequest(new { Success = false, Message = "Payment verification failed" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }

    public class CreateOrderRequest
    {
        [JsonRequired]
        public decimal Amount { get; set; }
    }

    public class VerifyPaymentRequest
    {
        public string OrderId { get; set; } = string.Empty;
        public string PaymentId { get; set; } = string.Empty;
        public string Signature { get; set; } = string.Empty;
    }
}