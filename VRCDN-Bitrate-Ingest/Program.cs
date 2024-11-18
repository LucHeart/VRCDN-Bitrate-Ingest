using LucHeart.VRCDN.BI.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus.Client.Collectors;
using Prometheus.Client.DependencyInjection;
using Prometheus.Client.MetricPusher;
using Prometheus.Client.MetricPusher.HostedService;
using Serilog;

namespace LucHeart.VRCDN.BI;

public static class Program {
    public static async Task Main(string[] args)
    {
        var loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}");
        
        // ReSharper disable once RedundantAssignment
        var isDebug = args
            .Any(x => x.Equals("--debug", StringComparison.InvariantCultureIgnoreCase));

#if DEBUG
        isDebug = true;
#endif
        if (isDebug)
        {
            Console.WriteLine("Debug mode enabled");
            loggerConfiguration.MinimumLevel.Verbose();
        }
        
        Log.Logger = loggerConfiguration.CreateLogger();
        
        var builder = Host.CreateDefaultBuilder();
        builder.ConfigureHostConfiguration(config =>
        {
            config.AddJsonFile("config.json", optional: true, reloadOnChange: false);
            config.AddCommandLine(args);
            config.AddEnvironmentVariables();
        });
        
        builder.ConfigureServices((context, services)  =>
        {
            services.AddSerilog();

            services.AddOptions<Config>().Bind(context.Configuration);
            
            services.AddMetricFactory();

            services.AddSingleton<IMetricPusher, MetricPusher>(provider =>
            {
                var config = provider.GetOptions<Config>();
                return new MetricPusher(new MetricPusherOptions
                {
                    Endpoint = config.Prometheus.Endpoint,
                    AdditionalHeaders = config.Prometheus.Headers.Select(x => x),
                    Job = "vrcdn-bitrate-ingest",
                    CollectorRegistry = provider.GetRequiredService<ICollectorRegistry>()
                });
            });
            
            services.AddSingleton<IHostedService, MetricPusherService>(provider => new MetricPusherService(provider.GetRequiredService<IMetricPusher>(), TimeSpan.FromSeconds(2)));
            services.AddHostedService<RecorderManager>();
        });

        var app = builder.Build();
        
        await app.RunAsync();
    }
    



}


