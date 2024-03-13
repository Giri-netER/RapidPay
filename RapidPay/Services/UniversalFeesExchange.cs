namespace RapidPay.Services
{
    public class UniversalFeesExchange
    {
        private double _fee;

        public UniversalFeesExchange()
        {
            // Initialize with a default fee
            _fee = 0.5;
        }

        public void UpdateFee()
        {
           
            Random random = new Random();
            double randomDecimal = random.NextDouble(); // Generates a random decimal between 0 and 1
            _fee *= randomDecimal * 2; // Update fee based on random decimal
        }

        public double GetCurrentFee()
        {
            
            return _fee;
        }
    }
}
