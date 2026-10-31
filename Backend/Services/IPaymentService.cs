namespace Backend.Services
{
    public interface IPaymentService
    {
        string CreateOrder(decimal amount, string currency = "INR");
        bool VerifyPayment(string orderId, string paymentId, string signature);
    }
}