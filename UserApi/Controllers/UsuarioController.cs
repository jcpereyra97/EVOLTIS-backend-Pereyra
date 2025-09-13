using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.VisualBasic;
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

        [HttpPost]
        public async Task<ActionResult> AgregarUsuario(UsuarioDTO usuario)
        {
            await _usuarioService.AgregarUsuarioAsync(usuario);
            return Ok();
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<ObtenerUsuarioDTO>>> ObtenerUsuariosPorFiltros([FromQuery] string? nombre, [FromQuery] string? provincia, [FromQuery] string? ciudad)
        {
            var result = await _usuarioService.ObtenerUsuariosConFiltrosAsync(nombre,provincia,ciudad);

            return Ok(result);
                
        }


    }
}
