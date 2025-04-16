namespace Api;

public class DummyJob : IFireAndForgetJob<DummyJobContext>
{
    private readonly ILogger<DummyJob> _logger;

    public DummyJob(ILogger<DummyJob> logger)
    {
        _logger = logger;
    }

    public Task Execute(DummyJobContext jobContext, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received request with id {Id}", jobContext.Id);
        return Task.CompletedTask;
    }
}
