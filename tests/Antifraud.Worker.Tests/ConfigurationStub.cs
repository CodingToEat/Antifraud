using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Antifraud.Worker.Tests;


public class ConfigurationStub : IConfiguration
{
    private readonly Dictionary<string, string> _values = new()
    {
        { "TransactionsApi:BaseUrl", "http://test-api" }
    };

    public string? this[string key]
    {
        get => _values.TryGetValue(key, out var value) ? value : null;
        set => _values[key] = value!;
    }

    public IEnumerable<IConfigurationSection> GetChildren() => Enumerable.Empty<IConfigurationSection>();
    public IChangeToken GetReloadToken() => NullChangeToken.Singleton;

    public IConfigurationSection GetSection(string key)
    {
        return new ConfigurationSectionStub(this, key);
    }
}

public class ConfigurationSectionStub : IConfigurationSection
{
    private readonly IConfiguration _root;
    private readonly string _key;

    public ConfigurationSectionStub(IConfiguration root, string key)
    {
        _root = root;
        _key = key;
    }

    public string this[string key]
    {
        get => _root[$"{_key}:{key}"] ?? "";
        set => throw new NotImplementedException();
    }

    public string Key => _key;
    public string Path => _key;

    public string? Value
    {
        get => _root[_key];
        set => _root[_key] = value;
    }


    public IEnumerable<IConfigurationSection> GetChildren() => Enumerable.Empty<IConfigurationSection>();
    public IChangeToken GetReloadToken() => NullChangeToken.Singleton;
    public IConfigurationSection GetSection(string key) => new ConfigurationSectionStub(_root, $"{_key}:{key}");
}