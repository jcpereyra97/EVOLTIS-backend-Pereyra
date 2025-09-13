using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UserDomain.Domain;

namespace UserInfrastructure.Interfaces
{
    public interface IUsuarioRepository
    {
        Task AgregarUsuarioAsync(Usuario usuario);
        Task ActualizarUsuarioAsync(Usuario usuario);
        Task<Usuario?> ObtenerUsuarioPorIdAsync(int usuarioID);
        Task<IEnumerable<Usuario>> ObtenerUsuariosPorFiltrosAsync(Expression<Func<Usuario, bool>> filtros);
        Task EliminarUsuarioPorId(int usuarioID);

    }
}
