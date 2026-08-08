using System.Text.Json;

namespace Caching;

internal sealed class SystemTextJsonCacheSerializer : ICacheSerializer
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public byte[] Serialize<T>(T value) =>
        JsonSerializer.SerializeToUtf8Bytes(value, SerializerOptions);

    public T? Deserialize<T>(byte[] value) =>
        JsonSerializer.Deserialize<T>(value, SerializerOptions);
}
