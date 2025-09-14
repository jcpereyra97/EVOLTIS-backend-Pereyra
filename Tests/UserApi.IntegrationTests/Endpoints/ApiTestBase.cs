// Setup/ApiTestBase.cs
using FluentAssertions;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using UserApi.IntegrationTests.Setup;
using UserApplication.Common.Pagination;
using UserApplication.DTOs;
using Xunit;
using Xunit.Abstractions;

[Collection("api")]
public abstract class ApiTestBase : IDisposable
{
    protected readonly CustomWebApplicationFactory _factory;
    protected readonly HttpClient _client;
    protected readonly ITestOutputHelper _output;

    protected ApiTestBase(MySqlContainerFixture fx, ITestOutputHelper output)
    {
        _output = output;
        _factory = new CustomWebApplicationFactory(fx.ConnectionString);
        _client = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    // Payloads/archivo.json debe estar copiado al output (bin)
    protected static StringContent JsonFromFile(string relativePath)
    {
        var full = Path.Combine(AppContext.BaseDirectory, "Payloads", relativePath);
        if (!File.Exists(full)) throw new FileNotFoundException($"No existe payload: {full}");
        var json = File.ReadAllText(full, Encoding.UTF8);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    public void OutputResponse<T>(T? response,string? texto = "")
    {
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
        if (!string.IsNullOrEmpty(texto))
        {
            _output.WriteLine(texto + ":\n" + json);
        }
        else
        {
            _output.WriteLine("Response: \n" + json);
        }
    }

    public void OutputLine() => _output.WriteLine("--------------------------------------------------------------");

    protected static StringContent JsonBody(object body) =>
        new StringContent(JsonSerializer.Serialize(body, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }), Encoding.UTF8, "application/json");

    public virtual void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    public async Task<IReadOnlyList<ObtenerUsuarioDTO>> ObtenerUsuarios()
    {
        //Consulto un usuario para obtener su ID
        var usuarioRq = await _client.GetAsync($"/api/usuarios?page=1&pageSize=20");
        var usuarioRs = await usuarioRq.Content.ReadFromJsonAsync<PaginationResponse<ObtenerUsuarioDTO>>();
        usuarioRs.Should().NotBeNull();
        usuarioRs.Items.Should().NotBeEmpty();

        return usuarioRs.Items;

    }
}
