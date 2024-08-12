using Domain.DomainModel.Interface;
using Infrastructure.Helpers;

namespace API.Services.BackgroundService
{
    public sealed class ScopedProcessingService : IScopedProcessingService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ScopedProcessingService> _logger;
        private readonly IDropShipRepository _dropShipRepository;

        public ScopedProcessingService(IConfiguration configuration, ILogger<ScopedProcessingService> logger, IDropShipRepository dropShipRepository)
        {
            _configuration = configuration;
            _logger = logger;
            _dropShipRepository = dropShipRepository;
        }

        //(IConfiguration configuration, ILogger<ScopedProcessingService> logger,
        //IDropShipRepository dropShipRepository) : IScopedProcessingService
        //{
        //private int _executionCount;
        public async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            string filePath = _configuration.GetSection("AppSettings:BellFlowerFilePath").Value;
            string fileName = _configuration.GetSection("AppSettings:BellFlowerFileName").Value;
            string execTime = _configuration.GetSection("AppSettings:BellFlowerTrigger").Value;

            if (filePath != null && fileName != null && execTime != null)
            {
                LogWriter _log = new LogWriter(_configuration);

                int.TryParse(execTime.Substring(0, 2), out int execHr);
                int.TryParse(execTime.Substring(2, 2), out int execMin);
                _log.LogWrite($"Bell Flower Trigger will run at : {execHr}:{execMin}:00");
                _logger.LogInformation($"Bell Flower Trigger will run at : {execHr}:{execMin}:00");

                while (!stoppingToken.IsCancellationRequested)
                {
                    //++_executionCount;
                    //logger.LogInformation($"{nameof(ScopedProcessingService)} working: Execution count: {_executionCount} Running at: {DateTimeOffset.Now}");
                    //_log.LogWrite($"{nameof(ScopedProcessingService)} working: Execution count: {_executionCount} Running at: {DateTimeOffset.Now}");
                    //logger.LogInformation($"Bell Flower Trigger will run at : {execHr}:{execMin}:00");
                    //_log.LogWrite($"Bell Flower Trigger will run at : {execHr}:{execMin}:00");

                    _dropShipRepository.GenerateBellFlowerDailyReport(execHr, execMin);
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }
    }
}
