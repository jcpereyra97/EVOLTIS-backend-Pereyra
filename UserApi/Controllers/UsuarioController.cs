using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.VisualBasic;
using UserApplication.Common.Pagination;
using UserApplication.DTOs;
using UserApplication.Interfaces;

namespace UserApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService) : base()
        {
            _usuarioService = usuarioService;
        }


        [HttpGet("{id}", Name = nameof(ObtenerUsuarioPorId))]
        public async Task<ActionResult<ObtenerUsuarioDTO>> ObtenerUsuarioPorId(int id)
        {
            var user = await _usuarioService.ObtenerUsuarioPorIdAsync(id);
            return user is null ? NotFound(id) : Ok(user);
        }


        [HttpGet(Name = nameof(ObtenerUsuariosPorFiltros))]
        public async Task<ActionResult<PaginationResponse<ObtenerUsuarioDTO>>> ObtenerUsuariosPorFiltros([FromQuery] string? nombre,
                                                        [FromQuery] string? provincia, [FromQuery] string? ciudad, [FromQuery] int page = 1,
                                                        [FromQuery] int pageSize = 20)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 20;

            var result = await _usuarioService.ObtenerUsuariosConFiltrosAsync(nombre, provincia, ciudad, page, pageSize);

            return Ok(result);

        }

        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public async Task<ActionResult<int>> AgregarUsuario(UsuarioDTO usuario)
        {
            var id = await _usuarioService.AgregarUsuarioAsync(usuario);
            return Created($"/usuario/{id}", new {id });
        }

       


        [HttpDelete("{id}")]
        public async Task<ActionResult> EliminarUsuario([FromRoute] int id)
        {
            await _usuarioService.EliminarUsuarioAsync(id);
            return Ok();    
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> ActualizarUsuario([FromRoute] int id,[FromBody] ActualizarUsuarioDTO usuarioDTO)
        {
            await _usuarioService.ActualizarUsuarioAsync(id,usuarioDTO);
            return Ok();
        }
    }
}
