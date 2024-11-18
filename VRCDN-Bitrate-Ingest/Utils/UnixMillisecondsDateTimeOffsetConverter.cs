using System.Text.Json;
using System.Text.Json.Serialization;

namespace LucHeart.VRCDN.BI.Utils;

public class UnixMillisecondsDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Read the Unix timestamp in milliseconds
        var unixTimeMilliseconds = reader.GetInt64();
        
        // Convert to DateTimeOffset
        return DateTimeOffset.FromUnixTimeMilliseconds(unixTimeMilliseconds);
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        // Convert DateTimeOffset to Unix timestamp in milliseconds
        var unixTimeMilliseconds = value.ToUnixTimeMilliseconds();
        
        // Write the Unix timestamp
        writer.WriteNumberValue(unixTimeMilliseconds);
    }
}
