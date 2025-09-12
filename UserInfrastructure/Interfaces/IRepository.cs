using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserDomain.Domain;

namespace UserInfrastructure.Interfaces
{
    public interface IRepository
    {
        Task AgregarUsuarioAsync(Usuario usuario);
        Task ActualizarUsuarioAsync(Usuario usuario);
        Task<Usuario> ObtenerUsuarioPorIdAsync(int usuarioID);
        Task<IEnumerable<Usuario>> ObtenerUsuariosPorFiltrosAsync(string nombre, string ciudad, string provincia);

    }
}
