using LineWebHookAPI.Models.Dto.Yahoo;
using LineWebHookAPI.Models.Services;

namespace LineWebHookAPI.Models.Job;

public class YahooBackgroundService(IServiceProvider sp, IBackgroundJobQueue<YahooLocalJob> queue) : BackgroundService
{
    private IServiceProvider ServiceProvider { get; } = sp;
    private IBackgroundJobQueue<YahooLocalJob> Queue { get; } = queue;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var job in Queue.ReadAllAsync(stoppingToken))
        {
            using var scope = ServiceProvider.CreateScope();
            var yahooService = scope.ServiceProvider.GetRequiredService<IYahooService>();
            await yahooService.PostLocalAsync(job.WebHook, job.GenreCode);
        }
    }
}
