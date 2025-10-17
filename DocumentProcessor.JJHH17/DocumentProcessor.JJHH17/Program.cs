using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DocumentProcessor.JJHH17;

public class Program 
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<ScheduledExport.ScheduledExportJob>();
        builder.Services.AddLogging();

        using var host = builder.Build();
        var runHost = host.RunAsync();

        var uiCts = new CancellationTokenSource();
        var uiTask = Task.Run(() => UserInterface.UserInterface.Menu(), uiCts.Token);

        await Task.WhenAny(runHost, uiTask);

        if (uiTask.IsCompleted)
        {
            await host.StopAsync();
        }
        else
        {
            uiCts.Cancel();
            try { await uiTask; } 
            catch (OperationCanceledException) { }

            await runHost;
        }
    }
}