using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UserDomain.Domain;
using UserInfrastructure.Repositories;

namespace UserInfrastructure.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<int> AgregarUsuarioAsync(Usuario usuario);
        Task ActualizarUsuarioAsync(Usuario usuario);
        Task<Usuario?> ObtenerUsuarioPorIdAsync(int usuarioID);
        Task<PaginationResult<Usuario>> ObtenerUsuariosPorFiltrosAsync(Expression<Func<Usuario, bool>> filtros, int page = 1, int pageSize = 20);
        Task EliminarUsuarioPorId(int usuarioID);

    }
}
