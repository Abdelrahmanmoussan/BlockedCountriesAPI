namespace BlockedCountriesAPI.BackgroundServices
{
    public class BlockCleanupService : BackgroundService
    {
        private readonly IBlockedCountryRepository _repository;
        private readonly ILogger<BlockCleanupService> _logger;

        public BlockCleanupService(IBlockedCountryRepository repository, ILogger<BlockCleanupService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Cleaning expired blocks...");
                _repository.RemoveExpiredBlocks();
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }

}
