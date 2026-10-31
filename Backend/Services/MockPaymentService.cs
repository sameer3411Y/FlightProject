namespace Backend.Services
{
    public class MockPaymentService : IPaymentService
    {
        public MockPaymentService()
        {
            Console.WriteLine("MOCK: MockPaymentService initialized for development/testing");
        }

        public string CreateOrder(decimal amount, string currency = "INR")
        {
            try
            {
                Console.WriteLine($"MOCK: Creating payment order with amount: {amount} {currency}");
                
                // Generate a mock order ID
                var mockOrderId = $"order_mock_{Guid.NewGuid().ToString("N")[..10]}";
                
                Console.WriteLine($"MOCK: Payment order created successfully: {mockOrderId}");
                return mockOrderId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MOCK: Payment order creation error: {ex.Message}");
                throw new Exception($"Mock payment order creation failed: {ex.Message}");
            }
        }

        public bool VerifyPayment(string orderId, string paymentId, string signature)
        {
            try
            {
                Console.WriteLine($"MOCK: Verifying payment - OrderId: {orderId}, PaymentId: {paymentId}");
                
                // For mock service, always return true for verification
                // In real implementation, this would verify the signature
                Console.WriteLine("MOCK: Payment verification successful");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MOCK: Payment verification failed: {ex.Message}");
                return false;
            }
        }
    }
}