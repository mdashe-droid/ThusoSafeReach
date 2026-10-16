using Microsoft.EntityFrameworkCore;
using ThusoSafeReach.Data;

namespace ThusoSafeReach.Services;

public class CheckInMonitor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CheckInMonitor> _logger;

    public CheckInMonitor(
        IServiceScopeFactory scopeFactory,
        ILogger<CheckInMonitor> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var db = scope.ServiceProvider
                    .GetRequiredService<ApplicationDbContext>();

                var overdue = await db.CheckIns
                    .Where(c => c.Status == "Active" &&
                                c.DueAtUtc <= DateTime.UtcNow)
                    .ToListAsync(stoppingToken);

                foreach (var checkIn in overdue)
                {
                    checkIn.Status = "Overdue";
                }

                if (overdue.Count > 0)
                    await db.SaveChangesAsync(stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error while checking overdue safety check-ins.");
            }

            await Task.Delay(
                TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}