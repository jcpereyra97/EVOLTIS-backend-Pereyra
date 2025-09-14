using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using UserApi.IntegrationTests.Setup;
using UserApplication.Common.Pagination;
using UserApplication.DTOs;
using Xunit.Abstractions;

namespace UserApi.IntegrationTests.Endpoints
{
    [Collection("api")]
    public class DELETE_Usuarios_Tests : ApiTestBase
    {

        public DELETE_Usuarios_Tests(MySqlContainerFixture fx, ITestOutputHelper output) : base(fx, output) { }

        /// <summary>
        /// Eliminar un usuario (desactivarlo) en la BD
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Eliminar_Usuario_Exito()
        {
            //Consulto un usuario para obtener su ID
            var usuarioRq = await _client.GetAsync($"/api/usuarios?page=1&pageSize=20");
            var usuarioRs = await usuarioRq.Content.ReadFromJsonAsync<PaginationResponse<ObtenerUsuarioDTO>>();
            usuarioRs.Should().NotBeNull();
            usuarioRs.Items.Should().NotBeEmpty();

            //Obtengo el ultimo id de usuario agregado
            var ultimoUsuario = usuarioRs.Items.LastOrDefault();
            OutputResponse(ultimoUsuario, "Ultimo Usuario");
            var id = ultimoUsuario.ID;
            var resp = await _client.DeleteAsync($"/api/usuarios/{id}");

            //Busco usuario 
            var getUsuarioRq = await _client.GetAsync($"/api/usuarios/{id}");
            var body = await getUsuarioRq.Content.ReadFromJsonAsync<ProblemDetails>();

            body.Should().NotBeNull();

            //Valido el mensaje de Usuario no encontrado lanzada por ProblemDetail, response que crea la Excepcion
            body.Detail.Should().NotBeNull().And.Be("Usuario no encontrado.");
            OutputResponse(body);



            resp.StatusCode.Should().Be(HttpStatusCode.NoContent);

        }

        /// <summary>
        /// Intento de eliminar un usuario inexistente forzando exception
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Eliminar_Usuario_Inexistente()
        {
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
}
