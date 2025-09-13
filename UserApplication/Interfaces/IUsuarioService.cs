using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserApplication.DTOs;

namespace UserApplication.Interfaces
{
    public interface IUsuarioService
    {
        Task AgregarUsuarioAsync(UsuarioDTO usuarioDTO);
        Task<ObtenerUsuarioDTO> ObtenerUsuarioPorIdAsync(int usuarioID);
        Task<IEnumerable<ObtenerUsuarioDTO>> ObtenerUsuariosConFiltrosAsync(string? nombre,string? provincia,string? ciudad);
        Task EliminarUsuarioAsync(int usuarioID);

    }
}
