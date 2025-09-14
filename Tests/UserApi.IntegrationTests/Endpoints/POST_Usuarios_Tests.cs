using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using UserApi.IntegrationTests.Helpers;
using UserApi.IntegrationTests.Setup;
using Xunit.Abstractions;

namespace UserApi.IntegrationTests.Endpoints
{

    [Collection("api")]
    public class POST_Usuarios_Tests : ApiTestBase
    {

        public POST_Usuarios_Tests(MySqlContainerFixture fx, ITestOutputHelper output) : base(fx, output) { }


        /// <summary>
        /// Agrega un usario con Exito y retorna su ID en DB. Si se ejecuta mas de una vez, dara error por duplicidad de email.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Agregar_Usuario_Exito()
        {
            var resp = await _client.PostAsync("/api/usuarios", TestContent.CreateContent("crear_usuario_ok.json"));
            //Imprimo Id generado
            OutputResponse(await resp.Content.ReadAsStringAsync());
            resp.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Creo un usuario con email existente forzando exception de error en base de datos por clave duplicada (email)
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Crear_Usuario_Email_existente()
        {
            var resp = await _client.PostAsync("/api/usuarios", TestContent.CreateContent("crear_usuario_email_existente.json"));
            var body = await resp.Content.ReadFromJsonAsync<ProblemDetails>();
            //valido que body no sea nulo
            body.Should().NotBeNull();

            //valido que sea error de base de datos
            body.Title.Should().NotBeNull().And.Be("Error de base de datos");

            //imprimo response
            OutputResponse(body);

            resp.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        /// <summary>
        /// Intento crear usuario con datos erroneos en request
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Crear_Usuario_Error_Request()
        {
            var content = TestContent.CreateContent("crear_usuario_error_request.json");
            var resp = await _client.PostAsync("/api/usuarios", content);

            var body = await resp.Content.ReadFromJsonAsync<ProblemDetails>();
            //valido que body no sea nulo
            body.Should().NotBeNull();
            OutputResponse(body);

            resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

    }

}