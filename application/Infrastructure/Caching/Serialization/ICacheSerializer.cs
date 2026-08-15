namespace ResumeEnhancer.Infrastructure.Caching;

public interface ICacheSerializer
{
    byte[] Serialize<T>(T value);

    T? Deserialize<T>(byte[] value);
}

