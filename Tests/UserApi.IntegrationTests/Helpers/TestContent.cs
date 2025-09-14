using Org.BouncyCastle.Bcpg.OpenPgp;
using System.Text;
using System.Text.Json;

namespace UserApi.IntegrationTests.Helpers;

public static class TestContent
{
    private static readonly JsonSerializerOptions _jsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private static string PayloadPath(string relativePath)
        => Path.Combine(AppContext.BaseDirectory, "Payloads", relativePath);

    public static StringContent CreateContent(string relativePath)
    {
        var full = PayloadPath(relativePath);
        if (!File.Exists(full))
            throw new FileNotFoundException($"No existe el payload: {full}");

        var json = File.ReadAllText(full, Encoding.UTF8);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    public static string ReadFile(string path)
    {
        var full = PayloadPath(path);
        if (!File.Exists(full))
            throw new FileNotFoundException($"No existe el payload: {full}");

        return File.ReadAllText(full, Encoding.UTF8);
    }
}
