using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Prometheus.Client;

namespace LucHeart.VRCDN.BI;

public sealed class RecorderManager : IHostedService
{
    private readonly Config _config;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IMetricFamily<IGauge, ValueTuple<string>> _bitrateMetric;
    private readonly List<VrcdnIngestBitrateRecorder> _recorders = [];
    private readonly ILogger<RecorderManager> _logger;

    public RecorderManager(IOptions<Config> config, IMetricFactory metricFactory, ILoggerFactory loggerFactory)
    {
        _config = config.Value;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<RecorderManager>();

        _bitrateMetric = metricFactory.CreateGauge("vrcdn_ingest_bitrate", "The bitrate of the VRCDN streams ingest",
            "stream_name", true);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await DisposeAndClearAllRecorders();

        foreach (var (key, value) in _config.Streams)
        {
            _recorders.Add(new VrcdnIngestBitrateRecorder(
                value,
                key,
                _bitrateMetric,
                _loggerFactory,
                _config.BitrateEndpoint));
        }

        await Task.WhenAll(_recorders.Select(x => x.Start()));
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await DisposeAndClearAllRecorders();
    }

    private async Task DisposeAndClearAllRecorders()
    {
        await Task.WhenAll(_recorders.Select(x => x.DisposeAsync().AsTask()));
        _recorders.Clear();
    }
}