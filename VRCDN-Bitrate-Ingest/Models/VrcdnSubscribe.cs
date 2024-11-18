using System.Text.Json.Serialization;

namespace LucHeart.VRCDN.BI.Models;

public sealed class VrcdnSubscribe
{
    [JsonPropertyName("subscribe")]
    public required string Subscribe { get; set; }
}