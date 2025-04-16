using Hangfire.Client;

namespace Api.Filters;

public class ClientFilter : IClientFilter
{
    private readonly ILogger<ClientFilter> _logger;

    public ClientFilter(ILogger<ClientFilter> logger)
    {
        _logger = logger;
    }

    public void OnCreating(CreatingContext context)
    {
        if (context.Job.Args[0] is not IJobContext jobContext)
        {
            return;
        }

        var jobKey = jobContext.GetJobKey();
        var hashedJobKey = CryptographyHelper.Sha256(jobKey);

        var jobKeyContent = context.Connection.GetAllEntriesFromHash(hashedJobKey);
        if (jobKeyContent is null || jobKeyContent.Count == 0)
        {
            var transaction = context.Connection.CreateWriteTransaction();
            var parameters = new Dictionary<string, string>
            {
                { "Job", context.Job.Type.Name }
            };
            transaction.SetRangeInHash(hashedJobKey, parameters);
            transaction.Commit();

            _logger.LogInformation("Set lock {JobKey} for job", hashedJobKey);
        }
        else
        {
            _logger.LogInformation("Cancelled job using lock {JobKey}", hashedJobKey);
            context.Canceled = true;
        }
    }

    public void OnCreated(CreatedContext context)
    {
        // Empty on purpose
    }
}
