using Cronos;
using Document.Processor.JJHH17.DataExporting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DocumentProcessor.JJHH17.ScheduledExport;

public class ScheduledExport
{

}

public class ScheduledExportJob : BackgroundService
{
    private static string TimeZoneID = "Europe/London";

    // Scheduled to run every day, 09:00 AM (BST / GMT)
    private static readonly CronExpression cron = CronExpression.Parse("0 9 * * *");
    private static readonly TimeZoneInfo Tz = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneID);
    private readonly ILogger<ScheduledExportJob> _logger;

    public ScheduledExportJob(ILogger<ScheduledExportJob> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stopToken)
    {
        _logger.LogInformation("PDF Export is generating");
        
        while (!stopToken.IsCancellationRequested)
        {
            var next = cron.GetNextOccurrence(DateTimeOffset.Now, Tz);
            if (next is null) break;

            var delay = next.Value - DateTimeOffset.Now;
            _logger.LogInformation($"Next PDF Export scheduled at {next})", next.Value);

            try
            {
                await Task.Delay(delay, stopToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }

            try
            {
                DataExport.CreateExportPDF();
                _logger.LogInformation("PDF Export generated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while generating PDF Export");
            }
        }

        _logger.LogInformation("Scheduled PDF Export is stopping");
    }
}