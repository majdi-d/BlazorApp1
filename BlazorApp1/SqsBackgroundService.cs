using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

public class SqsBackgroundService : BackgroundService
{
    private readonly SqsService _sqsService;
    private readonly Func<Task> _onMessageReceived; // Delegate to trigger actions on the Blazor page

    public SqsBackgroundService(SqsService sqsService, Func<Task> onMessageReceived)
    {
        _sqsService = sqsService;
        _onMessageReceived = onMessageReceived;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine($"Polling at {DateTime.UtcNow.ToLongTimeString()}...");
            var message = await _sqsService.FetchMessageAsync();
            if (!string.IsNullOrEmpty(message))
            {
                // Trigger Blazor refresh
                Console.WriteLine($"Invoking refresh");
                await _onMessageReceived.Invoke();
            }

            // Delay before polling again
            await Task.Delay(2000, stoppingToken);
        }
    }
}
