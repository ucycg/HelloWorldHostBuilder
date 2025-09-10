using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HostBuilderPlayground;

public interface IMessageWriter
{
    void WriteMessage();
}

public class ConsoleMessageWriter : IMessageWriter
{

    public void WriteMessage()
    {
        Console.WriteLine("Hello from DI + HostBuilder!");
    }
}

public class App : IHostedService
{
    private readonly IMessageWriter _messageWriter;
    
    public App(IMessageWriter messageWriter)
    {
        _messageWriter = messageWriter;
    }

    void Run()
    {
        _messageWriter.WriteMessage();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Starting App ...");
        Run();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Done with App ...");
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