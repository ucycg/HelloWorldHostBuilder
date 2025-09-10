using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HostBuilderPlayground;

public interface IMessageWriter
{
    void WriteMessage();
}

public class ConsoleMessageWriter : IMessageWriter
{

    public void WriteMessage()
    {
        Console.Error.WriteLine("Hello from DI + HostBuilder!");
    }
}

public class App : IHostedService
{
    private readonly IMessageWriter _messageWriter;
    private readonly IHostApplicationLifetime _appLifetime;
    
    public App(IMessageWriter messageWriter, IHostApplicationLifetime appLifetime)
    {
        _messageWriter = messageWriter;
        _appLifetime = appLifetime;
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
            Console.Error.WriteLine("Starting App ...");
            Run();
            Console.Error.WriteLine("Done with App ...");
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
                services.AddSingleton<IMessageWriter, ConsoleMessageWriter>();
                services.AddHostedService<App>();
            });

        IHost host = builder.Build();
        host.Run();
    }
}