using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HostBuilderPlayground;

public interface IMessageWriter
{
    void WriteMessage();
}

public class LogMessageWriter : IMessageWriter
{
    private readonly ILogger<LogMessageWriter> _logger;
    public LogMessageWriter(ILogger<LogMessageWriter> logger)
    {
        _logger = logger;
    }

    public void WriteMessage()
    {
        _logger.LogInformation("Hello from DI + HostBuilder!");
    }
}

public class App : IHostedService
{
    private readonly IMessageWriter _messageWriter;
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly ILogger<App> _logger;
    
    public App(IMessageWriter messageWriter, IHostApplicationLifetime appLifetime, ILogger<App> logger)
    {
        _messageWriter = messageWriter;
        _appLifetime = appLifetime;
        _logger = logger;
    }

    void Run()
    {
        _messageWriter.WriteMessage();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // register the callback so the TCS completes when the host signals ApplicationStarted
        _appLifetime.ApplicationStarted.Register(() =>
        {
            _logger.LogInformation("Starting App ...");
            Run();
            _logger.LogInformation("Done with App ...");

            _appLifetime.StopApplication();
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        
        return Task.CompletedTask;
    }

}



class Program
{
    static void Main(string[] args)
    {
        IHostBuilder builder = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<IMessageWriter, LogMessageWriter>();
                services.AddHostedService<App>();
            });

        IHost host = builder.Build();
        host.Run();
    }
}