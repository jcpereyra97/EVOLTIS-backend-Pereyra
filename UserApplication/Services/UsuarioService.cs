using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserApplication.DTOs;
using UserApplication.Interfaces;

namespace UserApplication.Services
{
    public class UsuarioService : IUsuarioService
    {
        public Task<int> AgregarUsuarioAsync(UsuarioDTO usuarioDTO)
        {
            throw new NotImplementedException();
        }

        public Task<ObtenerUsuarioDTO> ObtenerUsuarioPorIdAsync(int usuarioID)
        {
            throw new NotImplementedException();
        }

        public Task<ObtenerUsuarioDTO> ObtenerUsuarioConFiltrosAsync(string? nombre, string? provincia, string? ciudad)
        {
            throw new NotImplementedException();
        }

        public Task EliminarUsuarioAsync(int usuarioID)
        {
            throw new NotImplementedException();
        }
    }
}
