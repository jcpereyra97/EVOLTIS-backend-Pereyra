// Endpoints/Usuarios/Get_Filters_Pagination_Tests.cs
using Docker.DotNet.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection; // <- necesario para GetRequiredService/GetServices
using System.Net;
using System.Net.Http.Json;
using UserApi.IntegrationTests.Setup;
using UserApplication.Common.Pagination;
using UserApplication.DTOs;
using Xunit.Abstractions;


[Collection("api")]
public class GET_Usuarios_Tests : ApiTestBase
{
    public GET_Usuarios_Tests(MySqlContainerFixture fx, ITestOutputHelper output) : base(fx, output) { }


    /// <summary>
    /// Obtiene exitosamente usuarios filtrados por ciudad
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Obtener_Usuarios_Cordoba_Paginado()
    {
        var ciudad = "Córdoba";
        var resp = await _client.GetAsync($"/api/usuarios?ciudad={ciudad}&page=1&pageSize=2");
        var dto = await resp.Content.ReadFromJsonAsync<PaginationResponse<ObtenerUsuarioDTO>>();
       
        dto.Should().NotBeNull();
        OutputResponse(dto);

        // Valido que efectivamente traiga las ciudades buscada en el filtro
        var domicilios = dto!.Items.SelectMany(u => u.Domicilios ?? Enumerable.Empty<ObtenerDomicilioDTO>());
        domicilios.Should().NotBeEmpty().And.OnlyContain(d => d.Ciudad.Equals(ciudad, StringComparison.OrdinalIgnoreCase));
        OutputLine();
        OutputResponse(domicilios.Select(p => p.Ciudad), "Valores");


        // valido respuesta de estado
        resp.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Obtiene exitosamente usuarios filtrados por nombre
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Obtener_Usuarios_Nombre_Paginado()
    {
        var nombre = "juan";
        var resp = await _client.GetAsync($"/api/usuarios?nombre={nombre}&page=1&pageSize=2");
        var dto = await resp.Content.ReadFromJsonAsync<PaginationResponse<ObtenerUsuarioDTO>>();
        //valido que no sea nulo
        dto.Should().NotBeNull();
        OutputResponse(dto);


        // Valido que efectivamente traiga los nombre buscados en el filtro
        var nombresLst = dto!.Items.Select(p => p.Nombre ?? string.Empty);
        nombresLst.Should().NotBeEmpty().And.OnlyContain(d => d.ToString().Contains(nombre, StringComparison.OrdinalIgnoreCase));
        OutputLine();
        OutputResponse(nombresLst,"Valores");
        // valido respuesta de estado
        resp.EnsureSuccessStatusCode();
    }


    /// <summary>
    /// Obtiene sin exito usuarios filtrados por provincia inexistente
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task No_Obtener_Usuarios_Provincia()
    {
        var ciudad = "Mendoza";
        var resp = await _client.GetAsync($"/api/usuarios?provincia={ciudad}&page=1&pageSize=2");



        var body = await resp.Content.ReadAsStringAsync();
        body.Should().BeNullOrEmpty();
        OutputResponse(body);

        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);

    }


    /// <summary>
    /// Obtiene exitosamente un Usuario por su ID
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Obtener_Usuario_Por_ID()
    {
        //Consulto un usuario para obtener su ID
        var usuarioRq = await _client.GetAsync($"/api/usuarios?page=1&pageSize=2");
        var usuarioRs = await usuarioRq.Content.ReadFromJsonAsync<PaginationResponse<ObtenerUsuarioDTO>>();
        usuarioRs.Should().NotBeNull();
        usuarioRs.Items.Should().NotBeEmpty();
        
        //Obtengo el ID del primer usuario
        var id = usuarioRs.Items.FirstOrDefault().ID;
        var resp = await _client.GetAsync($"/api/usuarios/{id}");
        var body = await resp.Content.ReadFromJsonAsync<ObtenerUsuarioDTO>();

        //No debe ser nulo
        body.Should().NotBeNull();

        //Debe ser el mismo ID buscado
        body.ID.Should().Be(id);    

        OutputResponse(body);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);

    }


    /// <summary>
    /// Obtiene sin exito un Usuario por su ID
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task No_Obtener_Usuario_Por_ID()
    {

        //Creo un numero elevado de usuario para su inexistencia
        var id = 999999;
        var resp = await _client.GetAsync($"/api/usuarios/{id}");
        var body = await resp.Content.ReadFromJsonAsync<ProblemDetails>();

        body.Should().NotBeNull();

        //Valido el mensaje de Usuario no encontrado lanzada por ProblemDetail, response que crea la Excepcion
        body.Detail.Should().NotBeNull().And.Be("Usuario no encontrado.");
        OutputResponse(body);

        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);

    }

}

