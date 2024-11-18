namespace LucHeart.VRCDN.BI;

public sealed class Config
{
    public Uri BitrateEndpoint { get; set; } = new Uri("wss://ws.vrcdn.live/bitrate");
    public Dictionary<string, string> Streams { get; set; } = new Dictionary<string, string>();
    
    public required PrometheusConfig Prometheus { get; set; }
    
    public sealed class PrometheusConfig
    {
        public required string Endpoint { get; set; }
        public string Job { get; set; } = "vrcdn-bitrate-ingest";
        public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
    }
}