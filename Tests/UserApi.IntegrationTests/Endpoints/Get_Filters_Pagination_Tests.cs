// Endpoints/Usuarios/Get_Filters_Pagination_Tests.cs
using Docker.DotNet.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection; // <- necesario para GetRequiredService/GetServices
using System.Net;
using System.Net.Http.Json;
using System.Text;
using UserApi.IntegrationTests.Setup;

[CollectionDefinition("api")] public class ApiCollection : ICollectionFixture<MySqlContainerFixture> { }

[Collection("api")]
public class Get_Filters_Pagination_Tests
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    static string Payload(string relativePath) => Path.Combine(AppContext.BaseDirectory, "Payloads", relativePath);
    public Get_Filters_Pagination_Tests(MySqlContainerFixture fx)
    {
        _factory = new CustomWebApplicationFactory(fx.ConnectionString);
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task DevuelvePaginaFiltradaPorNombreYProvincia()
    {
        var resp = await _client.GetAsync("/api/usuarios?nombre=Juan&provincia=Córdoba&page=1&pageSize=10");
        resp.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var dto = await resp.Content.ReadFromJsonAsync<PaginationResponse<ObtenerUsuarioDTO>>();
        dto!.Page.Should().Be(1);
        dto.PageSize.Should().Be(10);
        dto.Items.Should().OnlyContain(x => x.Nombre.Contains("Juan", StringComparison.OrdinalIgnoreCase));

    }

    [Fact]
    public async Task Post()
    {
        var endpoints = _factory.Services.GetRequiredService<IEnumerable<EndpointDataSource>>()
              .SelectMany(ds => ds.Endpoints)
              .OfType<RouteEndpoint>()
              .ToList();

        // ponelos en el output o debug
        foreach (var r in endpoints)
            System.Diagnostics.Debug.WriteLine($"ROUTE: {r}");
        var resp = await _client.GetAsync("/api/usuarios?page=1&pageSize=10");
        resp.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var dto = await resp.Content.ReadFromJsonAsync<PaginationResponse<ObtenerUsuarioDTO>>();
        dto!.Page.Should().Be(1);
        dto.PageSize.Should().Be(10);
        dto.Items.Should().OnlyContain(x => x.Nombre.Contains("Juan", StringComparison.OrdinalIgnoreCase));

    }


    [Fact]
    public async Task Agregar_Usuario_Exito()
    {
        var resp = await _client.PostAsync("/api/usuarios", CreateContent("crear_usuario_ok.json"));
        resp.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Crear_Usuario_Email_existente()
    {
        var resp = await _client.PostAsync("/api/usuarios", CreateContent("crear_usuario_email_existente.json"));
        resp.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Crear_Usuario_Error_Request()
    {
        var resp = await _client.PostAsync("/api/usuarios", CreateContent("crear_usuario_error_request.json"));
        resp.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }



    private StringContent CreateContent(string nombreArchivo)
    {

        var jsonPath = Payload(Path.Combine(nombreArchivo));
        var json = File.ReadAllText(jsonPath, Encoding.UTF8);
        return new StringContent(json, Encoding.UTF8, "application/json");

    }
}

public record PaginationResponse<T>(int Page, int PageSize, int Total, IReadOnlyList<T> Items);
public record ObtenerUsuarioDTO(int ID, string Nombre, string Email);
