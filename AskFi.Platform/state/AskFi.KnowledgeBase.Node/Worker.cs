using AskFi.Runtime.Platform;

namespace Rabot.ObservationPool.Node;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var persistence = default(IPlatformPersistence);
        var messaging = default(IPlatformMessaging);

        var sequencer = AskFi.Runtime.KnowledgeBaseGossip.Build(persistence, messaging);
        await sequencer.Run(stoppingToken);
    }
}
