namespace Api;

public class DummyJobContext : IJobContext
{
    public Guid Id { get; init; }
    public string GetJobKey() => Id.ToString();
}
