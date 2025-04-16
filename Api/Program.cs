using System.Transactions;
using Api;
using Api.Filters;
using Hangfire;
using Hangfire.Client;
using Hangfire.Server;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddHangfire((serviceProvider, configuration) =>
    {
        configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_180);
        configuration.UseSimpleAssemblyNameTypeSerializer();
        configuration.UseRecommendedSerializerSettings();
        configuration.UseSqlServerStorage("Server=localhost;Database=HangfireTest;User ID=sa;Password=Password12!;TrustServerCertificate=true");

        configuration.UseFilter(serviceProvider.GetRequiredService<IServerFilter>());
        configuration.UseFilter(serviceProvider.GetRequiredService<IClientFilter>());
    });

builder.Services.AddTransient<IServerFilter, ServerFilter>();
builder.Services.AddTransient<IClientFilter, ClientFilter>();

builder.Services.AddHangfireServer();

builder.Services.AddScoped<IFireAndForgetJob<DummyJobContext>, DummyJob>();

var app = builder.Build();

app.MapHangfireDashboard();

app.MapGet(
    "/enqueue",
    ([FromServices] IBackgroundJobClient backgroundJobClient, [FromServices] IFireAndForgetJob<DummyJobContext> dummyJob, CancellationToken cancellationToken) =>
    {
        var id = Guid.NewGuid();

        using var transaction = new TransactionScope();
        backgroundJobClient.Enqueue(() => dummyJob.Execute(new DummyJobContext { Id = id }, cancellationToken));
        backgroundJobClient.Enqueue(() => dummyJob.Execute(new DummyJobContext { Id = id }, cancellationToken));
        backgroundJobClient.Enqueue(() => dummyJob.Execute(new DummyJobContext { Id = id }, cancellationToken));
        backgroundJobClient.Enqueue(() => dummyJob.Execute(new DummyJobContext { Id = id }, cancellationToken));
        backgroundJobClient.Enqueue(() => dummyJob.Execute(new DummyJobContext { Id = id }, cancellationToken));
        transaction.Complete();
    });

app.Run();