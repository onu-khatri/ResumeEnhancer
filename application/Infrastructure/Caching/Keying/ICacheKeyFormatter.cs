namespace Caching;

internal interface ICacheKeyFormatter
{
    string Format(string key);
}
