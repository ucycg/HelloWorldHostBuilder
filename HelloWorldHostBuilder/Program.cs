using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace HostBuilderPlayground;

public class App : IHostedService
{
    private readonly IMessageWriter _messageWriter;
    private readonly IHostApplicationLifetime _appLifetime;
    public App(IMessageWriter messageWriter, IHostApplicationLifetime appLifetime)
    {
        _messageWriter = messageWriter;
        _appLifetime = appLifetime;
    }
        public Task StartAsync(CancellationToken cancellationToken)
    {
        _appLifetime.ApplicationStarted.Register(() =>
        {
            Task.Run(() =>
            {
                try
                {
                    Console.WriteLine("Starting App ...");
                    Run();
                }
                finally
                {
                    _appLifetime.StopApplication();
                }
            });
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
       
        Console.WriteLine("Done, Closing App ...");
        return Task.CompletedTask;
    }

    void Run()
    {
        _messageWriter.WriteMessage();
    }
}

public interface IMessageWriter
{
    void WriteMessage();
}

public class ConsoleMessageWriter : IMessageWriter
{
    public void WriteMessage() => Console.WriteLine("Hello from DI + HostBuilder!");
}

class Program
{
    static void Main(string[] args)
    {
        using IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<IMessageWriter, ConsoleMessageWriter>();
                services.AddHostedService<App>();
            })
            .Build();
        host.Run();
    }
}