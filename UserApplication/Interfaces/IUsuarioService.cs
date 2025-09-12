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
        Task<int> AgregarUsuarioAsync(UsuarioDTO usuarioDTO);
        Task<ObtenerUsuarioDTO> ObtenerUsuarioPorIdAsync(int usuarioID);
        Task<ObtenerUsuarioDTO> ObtenerUsuarioConFiltrosAsync(string? nombre,string? provincia,string? ciudad);
        Task EliminarUsuarioAsync(int usuarioID);

    }
}
