using Hangfire.Server;

namespace Api.Filters;

public class ServerFilter : IServerFilter
{
    private readonly ILogger<ServerFilter> _logger;

    public ServerFilter(ILogger<ServerFilter> logger)
    {
        _logger = logger;
    }

    public void OnPerforming(PerformingContext context)
    {
        // Empty on purpose
    }

    public void OnPerformed(PerformedContext context)
    {
        if (context.BackgroundJob.Job.Args[0] is not IJobContext jobContext)
        {
            return;
        }

        var jobKey = jobContext.GetJobKey();
        var hashedJobKey = CryptographyHelper.Sha256(jobKey);

        var transaction = context.Connection.CreateWriteTransaction();
        transaction.RemoveHash(hashedJobKey);
        transaction.Commit();

        _logger.LogInformation("Removed lock {JobKey}", hashedJobKey);
    }
}
