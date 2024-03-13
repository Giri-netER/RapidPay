namespace RapidPay.Services
{
    public class FeeUpdateService : BackgroundService
    {
        private readonly UniversalFeesExchange _ufe;

        public FeeUpdateService(UniversalFeesExchange ufe)
        {
            _ufe = ufe;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Update fee
                _ufe.UpdateFee();

                // Wait for one hour before updating fee again
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}
