namespace WorkerDemo;

public class Worker : BackgroundService
{
    private readonly List<Task> _tasks;
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _tasks = new();
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[{time}] Creating tasks...", DateTimeOffset.Now);

        for (int i = 0; i < 20; i++)
        {
            _tasks.Add(Task.Run(() =>
            {
                int iterations = 0;
                for (int crt = 1; crt <= 2000000; crt++)
                {
                    stoppingToken.ThrowIfCancellationRequested();
                    iterations++;

                    Task.Delay(500, stoppingToken);
                }
            }, stoppingToken));
        }

        _logger.LogInformation("[{time}] Tasks created successfully", DateTimeOffset.Now);

        await Task.Run(async () =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("[{time}] Worker list start", DateTimeOffset.Now);

                foreach (var t in _tasks)
                {
                    _logger.LogInformation("Task: {taskId} - Status: {taskStatus}", t.Id, t.Status);
                }

                _logger.LogInformation("[{time}] Worker list end", DateTimeOffset.Now);

                await Task.Delay(2000, stoppingToken);
            }            
        }, stoppingToken);
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{time}] Worker starting...", DateTimeOffset.Now);
        await base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{time}] Worker stopping...", DateTimeOffset.Now);
        return base.StopAsync(cancellationToken);
    }
}
