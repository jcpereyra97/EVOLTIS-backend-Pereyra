using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserDomain.Domain;
using UserInfrastructure.Interfaces;

namespace UserInfrastructure.Repositories
{
    public class UsuarioRepository : IRepository
    {
        public Task AgregarUsuarioAsync(Usuario usuario)
        {
            throw new NotImplementedException();
        }

        public Task ActualizarUsuarioAsync(Usuario usuario)
        {
            throw new NotImplementedException();
        }

        public Task<Usuario> ObtenerUsuarioPorIdAsync(int usuarioID)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Usuario>> ObtenerUsuariosPorFiltrosAsync(string nombre, string ciudad, string provincia)
        {
            throw new NotImplementedException();
        }
    }
}
