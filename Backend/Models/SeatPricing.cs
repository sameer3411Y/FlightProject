namespace Backend.Models
{
    public static class SeatPricing
    {
        public static decimal GetSeatPrice(TravelClass travelClass, decimal basePrice)
        {
            return travelClass switch
            {
                TravelClass.Economy => basePrice,
                TravelClass.Business => basePrice * 2.5m,
                TravelClass.First => basePrice * 4.0m,
                _ => basePrice
            };
        }
    }
}