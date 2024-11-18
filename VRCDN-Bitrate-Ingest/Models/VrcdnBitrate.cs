using System.Text.Json.Serialization;
using LucHeart.VRCDN.BI.Utils;

namespace LucHeart.VRCDN.BI.Models;

public sealed class VrcdnBitrate
{
    [JsonPropertyName("timestamp")]
    [JsonConverter(typeof(UnixMillisecondsDateTimeOffsetConverter))]
    public required DateTimeOffset Timestamp { get; set; }
    [JsonPropertyName("bitrate")]
    public required ulong Bitrate { get; set; }
}