using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace HostBuilderPlayground;

public class App : BackgroundService
{
    private readonly IMessageWriter _messageWriter;
    private readonly IHostApplicationLifetime _appLifetime;
    public App(IMessageWriter messageWriter, IHostApplicationLifetime appLifetime)
    {
        _messageWriter = messageWriter;
        _appLifetime = appLifetime;
    }
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(1000, cancellationToken);

        Console.WriteLine("Staring App ...");
        Run();
        Console.WriteLine("Done, Closing App ...");

        _appLifetime.StopApplication();
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