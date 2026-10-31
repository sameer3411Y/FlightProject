using Razorpay.Api;
using Microsoft.Extensions.Configuration;

namespace Backend.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly string _keyId = string.Empty;
        private readonly string _keySecret = string.Empty;

        public PaymentService(IConfiguration configuration)
        {
            _keyId = configuration["Razorpay:KeyId"] ?? "";
            _keySecret = configuration["Razorpay:KeySecret"] ?? "";
            
            Console.WriteLine($"Razorpay KeyId: {_keyId}");
            Console.WriteLine($"Razorpay KeySecret: {(_keySecret?.Length > 5 ? _keySecret.Substring(0, 5) + "..." : "[empty]")}...");
        }

        public virtual string CreateOrder(decimal amount, string currency = "INR")
        {
            try
            {
                Console.WriteLine($"Creating Razorpay order with amount: {amount}");
                
                if (string.IsNullOrEmpty(_keyId) || string.IsNullOrEmpty(_keySecret))
                {
                    throw new Exception("Razorpay credentials are missing. Please configure valid KeyId and KeySecret in appsettings.json");
                }

                RazorpayClient client = new RazorpayClient(_keyId, _keySecret);

                Dictionary<string, object> options = new Dictionary<string, object>
                {
                    {"amount", (int)(amount * 100)}, // Amount in paise
                    {"currency", currency},
                    {"receipt", Guid.NewGuid().ToString()}
                };

                Order order = client.Order.Create(options);
                Console.WriteLine($"Razorpay order created: {order["id"]}");
                return order["id"].ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Razorpay error: {ex.Message}");
                throw new Exception($"Payment order creation failed: {ex.Message}");
            }
        }

        public virtual bool VerifyPayment(string orderId, string paymentId, string signature)
        {
            try
            {
                Console.WriteLine($"Verifying Razorpay payment: OrderId={orderId}, PaymentId={paymentId}");
                
                RazorpayClient client = new RazorpayClient(_keyId, _keySecret);
                
                Dictionary<string, string> attributes = new Dictionary<string, string>
                {
                    {"razorpay_order_id", orderId},
                    {"razorpay_payment_id", paymentId},
                    {"razorpay_signature", signature}
                };

                Utils.verifyPaymentSignature(attributes);
                Console.WriteLine("Payment verification successful");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Payment verification failed: {ex.Message}");
                return false;
            }
        }
    }
}