using LucHeart.VRCDN.BI.Models;
using LucHeart.VRCDN.BI.Utils;
using LucHeart.WebsocketLibrary;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Prometheus.Client;

namespace LucHeart.VRCDN.BI;

public sealed class VrcdnIngestBitrateRecorder : IAsyncDisposable
{
    private readonly string _streamName;
    private readonly string _vrcdnKeyXored;
    private readonly JsonWebsocketClient<VrcdnBitrate, VrcdnSubscribe> _websocketConnection;
    private readonly ILogger<VrcdnIngestBitrateRecorder> _logger;
    private readonly IGauge _gaugeMetric;

    public VrcdnIngestBitrateRecorder(
        string vrcdnKey,
        string streamName,
        IMetricFamily<IGauge, ValueTuple<string>> gaugeMetric,
        ILoggerFactory loggerFactory,
        Uri bitrateEndpoint
    )
    {
        _streamName = streamName;
        _gaugeMetric = gaugeMetric.WithLabels(streamName);
        _logger = loggerFactory.CreateLogger<VrcdnIngestBitrateRecorder>();
        _vrcdnKeyXored = VrcdnUtils.ConvertKeyToVrcdnAuthThing(Constants.VrcdnXorKey, vrcdnKey);

        _websocketConnection = new JsonWebsocketClient<VrcdnBitrate, VrcdnSubscribe>(
            bitrateEndpoint, new WebsocketClientOptions
            {
                Logger = loggerFactory.CreateLogger<JsonWebsocketClient<VrcdnBitrate, VrcdnSubscribe>>()
            });

        _websocketConnection.OnConnected += WebsocketConnectionOnConnected;

        _websocketConnection.OnMessage += WebsocketConnectionOnMessage;

        _logger.LogTrace("[{StreamName}] Created VrcdnIngestBitrateRecorder", _streamName);
    }

    private async Task WebsocketConnectionOnConnected()
    {
        await _websocketConnection.QueueMessage(new VrcdnSubscribe
        {
            Subscribe = _vrcdnKeyXored
        });
    }

    private Task WebsocketConnectionOnMessage(VrcdnBitrate message)
    {
        _logger.LogTrace("[{StreamName}] Bitrate {Bitrate} at {Timestamp}", _streamName, message.Bitrate,
            message.Timestamp);
        _gaugeMetric.Set(message.Bitrate, message.Timestamp);
        return Task.CompletedTask;
    }

    public async Task Start()
    {
        _logger.LogTrace("[{StreamName}] Starting VrcdnIngestBitrateRecorder", _streamName);
        await _websocketConnection.ConnectAsync();
        _logger.LogDebug("[{StreamName}] Connected WS", _streamName);
    }

    public async Task Stop()
    {
        await _websocketConnection.Stop();
    }

    private bool _disposed;

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;

        await _websocketConnection.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    ~VrcdnIngestBitrateRecorder()
    {
        DisposeAsync().AsTask().Wait();
    }
}