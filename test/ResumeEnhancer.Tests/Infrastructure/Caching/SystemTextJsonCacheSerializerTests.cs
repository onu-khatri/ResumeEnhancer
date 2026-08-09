using Shouldly;
using Caching;

namespace ResumeEnhancer.Tests.Infrastructure.Caching;

public sealed class SystemTextJsonCacheSerializerTests
{
    [Fact]
    public void Deserialize_SerializedRecord_RoundTripsUsingWebJsonDefaults()
    {
        var serializer = new SystemTextJsonCacheSerializer();
        var value = new CachedPerson("Ada", 42);

        var bytes = serializer.Serialize(value);
        var deserialized = serializer.Deserialize<CachedPerson>(bytes);

        deserialized.ShouldBe(value);
    }

    private sealed record CachedPerson(string Name, int Score);
}
