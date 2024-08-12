using API.Services.BackgroundService;
using Domain.DomainModel.Interface;
using Google.Protobuf.WellKnownTypes;
using Infrastructure.Helpers;
using System.Threading;

public sealed class BackgroundWorkerService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<BackgroundWorkerService> _logger;

    public BackgroundWorkerService(IServiceScopeFactory serviceScopeFactory, ILogger<BackgroundWorkerService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    private const string ClassName = nameof(BackgroundWorkerService);
    //LogWriter _log = new LogWriter("--->> BackgroundWorkerService <<---");

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"{ClassName} is running.");
        //_log.LogWrite($"{ClassName} is running.");
        await DoWorkAsync(stoppingToken);
    }

    private async Task DoWorkAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"{ClassName} Worker running at: {DateTimeOffset.Now}");
        //_log.LogWrite($"{ClassName} Worker running at: {DateTimeOffset.Now}");

        using (IServiceScope scope = _serviceScopeFactory.CreateScope())
        {
            IScopedProcessingService scopedProcessingService = scope.ServiceProvider.GetRequiredService<IScopedProcessingService>();
            await scopedProcessingService.DoWorkAsync(stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"{ClassName} is stopping.");
        //_log.LogWrite($"{ClassName} is stopping.");
        await base.StopAsync(stoppingToken);
    }
}