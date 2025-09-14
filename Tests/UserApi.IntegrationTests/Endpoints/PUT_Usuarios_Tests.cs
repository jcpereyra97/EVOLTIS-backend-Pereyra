using Docker.DotNet.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Json;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Text.Json;
using UserApi.IntegrationTests.Helpers;
using UserApi.IntegrationTests.Setup;
using UserApplication.Common.Pagination;
using UserApplication.DTOs;
using Xunit.Abstractions;

namespace UserApi.IntegrationTests.Endpoints
{
    [Collection("api")]
    public class PUT_Usuarios_Tests : ApiTestBase
    {
        public PUT_Usuarios_Tests(MySqlContainerFixture fx, ITestOutputHelper output) : base(fx, output) { }

        /// <summary>
        /// Actualiza datos de usuario en la base de datos de manera exitosa
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Actualiza_Usuario_Datos_Exito()
        {
            var fileName = "actualizar_usuario_ok.json";
            var usuariosLst = await ObtenerUsuarios();
            var usuario = usuariosLst.LastOrDefault();

            Assert.NotNull(usuario);

            var id = usuario.ID;

            var content = TestContent.CreateContent(fileName);

            var resp = await _client.PutAsync($"/api/usuarios/{id}", content);
            var body = await resp.Content.ReadFromJsonAsync<ObtenerUsuarioDTO>();

            Assert.NotNull(body);

            // seriaizo el Request para comparar 
            var fileText = TestContent.ReadFile(fileName);
            var serializedContent = JsonSerializer.Deserialize<ActualizarUsuarioDTO>(fileText, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(serializedContent);


            //Valido que los datos enviados por el request sean los mismos obtenidos en response
            body.Nombre.Should().Be(serializedContent.Nombre);
            body.Email.Should().Be(serializedContent.Email);

            OutputResponse(serializedContent, "Request");
            OutputLine();
            OutputResponse(body);

            resp.EnsureSuccessStatusCode();

        }

        /// <summary>
        /// Actualizar sin exito datos de usuario que no cumplen con las condiciones de entrada
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Actualiza_Usuario_Datos_Erroneos_No_Exito()
        {
            var fileName = "actualizar_usuario_error_request.json";
            var content = TestContent.CreateContent(fileName);

            //Simulo un numero por que no va a llegar nunca al Service
            var resp = await _client.PutAsync($"/api/usuarios/9999", content);
            var body = await resp.Content.ReadAsStringAsync();
            Assert.NotNull(body);

            OutputResponse(body);

            resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        }

        /// <summary>
        /// Intento de actualizacion de usuario inexistente
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Actualizar_Usuario_Inexistente()
        {
            var fileName = "actualizar_usuario_ok.json";
            var content = TestContent.CreateContent(fileName);

            //Simulo un numero elevado que no encontrara en la base de datos
            var resp = await _client.PutAsync($"/api/usuarios/9999", content);
            var body = await resp.Content.ReadFromJsonAsync<ProblemDetails>();

            Assert.NotNull(body);

            body.Detail.Should().NotBeNull().And.Be("Usuario no encontrado.");
            OutputResponse(body);

            resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }


        /// <summary>
        /// Intento de actualizacion de usuario eliminado sin exito
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Actualizar_Usuario_Inactivo()
        {
            //Elimino un usuario
            //Consulto un usuario para obtener su ID
            var usuariosLst = await ObtenerUsuarios();

            //Obtengo el ultimo id de usuario agregado
            var ultimoUsuario = usuariosLst.LastOrDefault();
            OutputResponse(ultimoUsuario, "Ultimo Usuario");

            var id = ultimoUsuario.ID;
            var resp = await _client.DeleteAsync($"/api/usuarios/{id}");


            //Intento actualizar
            var fileName = "actualizar_usuario_ok.json";
            var content = TestContent.CreateContent(fileName);

            //Simulo un numero elevado que no encontrara en la base de datos
            var putRq = await _client.PutAsync($"/api/usuarios/{id}", content);
            var body = await putRq.Content.ReadFromJsonAsync<ProblemDetails>();


            Assert.NotNull(body);

            body.Detail.Should().NotBeNull().And.Be("Usuario no encontrado.");
            OutputLine();
            OutputResponse(body);

            resp.StatusCode.Should().Be(HttpStatusCode.NoContent);

        }

        /// <summary>
        /// Actualizar Domicilio existente de Usuario existente
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Actualizar_Usuario_Domicilio_Existente()
        {
            //Obtengo usuarios
            var usuariosLst = await ObtenerUsuarios();

            // Me quedo con el ultimo de los usuarios que tenga domicilio
            var ultimoUsuario = usuariosLst.Where(p => p.Domicilios.Any()).LastOrDefault();

            Assert.NotNull(ultimoUsuario);
            ultimoUsuario.Domicilios.Should().NotBeEmpty();
            //Obtengo el usuario ID y el domicilioID para modificarlo en el content
            var id = ultimoUsuario.ID;
            var domicilioId = ultimoUsuario.Domicilios.FirstOrDefault().ID;

            // lee el JSON del archivo
            var json = TestContent.ReadFile("actualizar_usuario_domicilio_existente_ok.json");

            // mapeo el DTO
            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var dto = JsonSerializer.Deserialize<ActualizarUsuarioDTO>(json, opts)!;

            //Actualizo id Ciudad
            var content = AgregarDomicilioIdContent(domicilioId, dto);

            var putRq = await _client.PutAsync($"/api/usuarios/{id}", content);
            var body = await putRq.Content.ReadFromJsonAsync<ObtenerUsuarioDTO>();

            Assert.NotNull(body);

            var validarDomicilio = body.Domicilios.FirstOrDefault(p => p.ID == domicilioId);

            Assert.NotNull(validarDomicilio);
            Assert.NotNull(dto.Domicilio);

            //Valido datos del request y el response
            validarDomicilio.Calle.Should().Be(dto.Domicilio.Calle);
            validarDomicilio.Numero.Should().Be(dto.Domicilio.Numero);
            validarDomicilio.Provincia.Should().Be(dto.Domicilio.Provincia);
            validarDomicilio.Ciudad.Should().Be(dto.Domicilio.Ciudad);

            OutputResponse(dto, "Request");
            OutputLine();
            OutputResponse(body);


            putRq.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Actualizar Usuario completo junto tambien con Domicilio existente.
        /// Si se ejecuta mas de una vez, puede fallar ya que intentara actualizar un email que ya existe.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Actualizar_Usuario_Completo_Exito()
        {
            //Obtengo usuarios
            var usuariosLst = await ObtenerUsuarios();          

            // Me quedo con el ultimo de los usuarios que tenga domicilio
            var ultimoUsuario = usuariosLst.Where(p => p.Domicilios.Any()).LastOrDefault();

            Assert.NotNull(ultimoUsuario);
            ultimoUsuario.Domicilios.Should().NotBeEmpty();
            //Obtengo el usuario ID y el domicilioID para modificarlo en el content
            var id = ultimoUsuario.ID;
            var domicilioId = ultimoUsuario.Domicilios.FirstOrDefault().ID;

            // lee el JSON del archivo
            var json = TestContent.ReadFile("actualizar_usuario_completo_ok.json");

            // mapeo el DTO
            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var dto = JsonSerializer.Deserialize<ActualizarUsuarioDTO>(json, opts)!;


            var content = AgregarDomicilioIdContent(domicilioId, dto);

            var putRq = await _client.PutAsync($"/api/usuarios/{id}", content);
            var body = await putRq.Content.ReadFromJsonAsync<ObtenerUsuarioDTO>();

            Assert.NotNull(body);

            body.Nombre.Should().Be(dto.Nombre);
            body.Email.Should().Be(dto.Email);

            var validarDomicilio = body.Domicilios.FirstOrDefault(p => p.ID == domicilioId);

            Assert.NotNull(validarDomicilio);
            Assert.NotNull(dto.Domicilio);

            validarDomicilio.Calle.Should().Be(dto.Domicilio.Calle);
            validarDomicilio.Numero.Should().Be(dto.Domicilio.Numero);
            validarDomicilio.Provincia.Should().Be(dto.Domicilio.Provincia);
            validarDomicilio.Ciudad.Should().Be(dto.Domicilio.Ciudad);

            OutputResponse(dto,"Request");
            OutputLine();
            OutputResponse(body);


            putRq.EnsureSuccessStatusCode();

        }


        /// <summary>
        /// Intento de actualizacion de domicilio con errores
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Actualizar_Usuario_Domicilio_Existente_ID_Error()
        {
            //Obtengo usuarios
            var usuariosLst = await ObtenerUsuarios();

            // Me quedo con el ultimo de los usuarios que tenga domicilio
            var ultimoUsuario = usuariosLst.Where(p => p.Domicilios.Any()).LastOrDefault();
            Assert.NotNull(ultimoUsuario);
            ultimoUsuario.Domicilios.Should().NotBeEmpty();

            //Obtengo el usuario ID 
            var id = ultimoUsuario.ID;


            var content = TestContent.CreateContent("actualizar_usuario_domicilio_no_existente_id_error.json");
            var putRq = await _client.PutAsync($"/api/usuarios/{id}", content);
            var body = await putRq.Content.ReadFromJsonAsync<ProblemDetails>();

            Assert.NotNull(body);

            OutputResponse(body);


            //Valido el mensaje de Domicilio no encontrado lanzada por ProblemDetail, response que crea la Excepcion
            body.Detail.Should().NotBeNull().And.Be("Domicilio no encontrado.");

            putRq.StatusCode.Should().Be(HttpStatusCode.NoContent);

        }



        /// <summary>
        /// Intento de actualizacion de domicilio con errores
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Actualizar_Usuario_Agregar_Domicilio()
        {
            //Obtengo usuarios
            var usuariosLst = await ObtenerUsuarios();

            // Me quedo con el ultimo de los usuarios que tenga domicilio
            var ultimoUsuario = usuariosLst.LastOrDefault();
            Assert.NotNull(ultimoUsuario);

            //Obtengo el usuario ID 
            var id = ultimoUsuario.ID;


            var content = TestContent.CreateContent("actualizar_usuario_domicilio_no_existente_ok.json");
            var putRq = await _client.PutAsync($"/api/usuarios/{id}", content);
            var body = await putRq.Content.ReadFromJsonAsync<ObtenerUsuarioDTO>();

            Assert.NotNull(body);

            OutputResponse(body);


            putRq.EnsureSuccessStatusCode();

        }

        /// <summary>
        /// Intenta actualizar datos de domicilio con ID existente, pero con datos faltantes
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Actualizar_Usuario_Domicilio_Existente_Error_Datos()
        {
            //Obtengo usuarios
            var usuariosLst = await ObtenerUsuarios();

            // Me quedo con el ultimo de los usuarios que tenga domicilio
            var ultimoUsuario = usuariosLst.Where(p => p.Domicilios.Any()).LastOrDefault();
            Assert.NotNull(ultimoUsuario);
            ultimoUsuario.Domicilios.Should().NotBeEmpty();

            //Obtengo el usuario ID y el domicilioID para modificarlo en el content
            var id = ultimoUsuario.ID;
            var domicilioId = ultimoUsuario.Domicilios.FirstOrDefault().ID;

            // lee el JSON del archivo
            var json = TestContent.ReadFile("actualizar_usuario_domicilio_existente_error_datos.json");

            // mapeo el DTO
            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var dto = JsonSerializer.Deserialize<ActualizarUsuarioDTO>(json, opts)!;

            //Actualizo id Ciudad
            var content = AgregarDomicilioIdContent(domicilioId, dto);

            var putRq = await _client.PutAsync($"/api/usuarios/{id}", content);
            var body = await putRq.Content.ReadFromJsonAsync<ProblemDetails>();

            Assert.NotNull(body);

            OutputResponse(dto, "Request");
            OutputLine();
            OutputResponse(body);


            putRq.StatusCode.Should().Be(HttpStatusCode.BadRequest);        

        }

        /// <summary>
        /// Intenta actualizar datos de domicilio con ID existente, pero con datos faltantes
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Actualizar_Usuario_Domicilio_Existente_Duplicado()
        {
            //Obtengo usuarios
            var usuariosLst = await ObtenerUsuarios();

            // Me quedo con el ultimo de los usuarios que tenga domicilio
            var ultimoUsuario = usuariosLst.Where(p => p.Domicilios.Any()).LastOrDefault();
            Assert.NotNull(ultimoUsuario);
            ultimoUsuario.Domicilios.Should().NotBeEmpty();

            //Obtengo el usuario ID y el domicilioID para modificarlo en el content
            var id = ultimoUsuario.ID;
            var domicilio = ultimoUsuario.Domicilios.FirstOrDefault();
            Assert.NotNull(domicilio);

            //Creo content con datos iguales de otro domicilio existente
            var content = CrearDomicilioContent(domicilio);

            var putRq = await _client.PutAsync($"/api/usuarios/{id}", content);
            var body = await putRq.Content.ReadAsStringAsync();

            Assert.NotNull(body);

            OutputLine();
            OutputResponse(body);


            putRq.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        }

        private StringContent AgregarDomicilioIdContent(int domicilioId,ActualizarUsuarioDTO actualizarUsuarioDTO)
        {
            actualizarUsuarioDTO.Domicilio ??= new ActualizarDomicilioDTO();
            actualizarUsuarioDTO.Domicilio.Id = domicilioId; 

            var content = new StringContent(
                JsonSerializer.Serialize(actualizarUsuarioDTO, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                Encoding.UTF8,
                "application/json");

            return content;
        }

        private StringContent CrearDomicilioContent(ObtenerDomicilioDTO domicilioDTO)
        {
            var domicilio = new ActualizarDomicilioDTO
            {
                Calle = domicilioDTO.Calle,
                Numero = domicilioDTO.Numero,
                Provincia = domicilioDTO.Provincia,
                Ciudad = domicilioDTO.Ciudad
            };
            var actualizarUsuario = new ActualizarUsuarioDTO { Domicilio = domicilio };

            var content = new StringContent(
               JsonSerializer.Serialize(actualizarUsuario, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
               Encoding.UTF8,
               "application/json");

            return content;
        }

    }
}