using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserApplication.Common.Pagination;
using UserApplication.DTOs;

namespace UserApplication.Interfaces
{
    public interface IUsuarioService
    {
        Task<int> AgregarUsuarioAsync(UsuarioDTO usuarioDTO);
        Task<ObtenerUsuarioDTO> ObtenerUsuarioPorIdAsync(int usuarioID);
        Task<PaginationResponse<ObtenerUsuarioDTO>> ObtenerUsuariosConFiltrosAsync(string? nombre, string? provincia, string? ciudad,int page = 1, int pageSize = 20);
        Task EliminarUsuarioAsync(int usuarioID);
        Task<ObtenerUsuarioDTO> ActualizarUsuarioAsync(int usuarioId,ActualizarUsuarioDTO usuarioDTO);
                
    }
}
