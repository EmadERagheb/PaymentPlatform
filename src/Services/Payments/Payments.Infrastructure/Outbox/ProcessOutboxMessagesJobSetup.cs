namespace Payments.Infrastructure.Outbox;

public class ProcessOutboxMessagesJobSetup(IOptions<OutboxOptions> options) : IConfigureOptions<QuartzOptions>
{
    private readonly OutboxOptions _options = options.Value;
    public void Configure(QuartzOptions options)
    {
        const string jobIdentity = nameof(ProcessOutboxMessagesJob);
        options.AddJob<ProcessOutboxMessagesJob>(configure => configure.WithIdentity(jobIdentity))
            .AddTrigger(configure => configure.ForJob(jobIdentity).WithSimpleSchedule(schedule => schedule.WithInterval(TimeSpan.FromSeconds(_options.IntervalInSeconds)).RepeatForever()));
    }
}
