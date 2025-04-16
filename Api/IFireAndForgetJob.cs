namespace Api;

public interface IFireAndForgetJob<in TJobContext> where TJobContext : IJobContext
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="jobContext"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task Execute(TJobContext jobContext, CancellationToken cancellationToken);
}
