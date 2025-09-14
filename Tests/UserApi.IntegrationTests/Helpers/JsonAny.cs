using System.Text.Json;
using System.Text.Json.Nodes;

public static class JsonAny
{
    private static readonly JsonSerializerOptions Default = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static T? To<T>(string? value, JsonSerializerOptions? options = null)
    {
        if (value is null) return default;
        if (value is T t) return t;

        options ??= Default;

        var json = JsonSerializer.Serialize<string>(value, options);
        return JsonSerializer.Deserialize<T>(json, options);
    }
}

