using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UserDomain.Domain;
using UserInfrastructure.EF;
using UserInfrastructure.Interfaces;

namespace UserInfrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _appDbContext;
        public UsuarioRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        /// <summary>
        /// Agrega Usuario
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        public async Task<int> AgregarUsuarioAsync(Usuario usuario)
        {

            await _appDbContext.Set<Usuario>().AddAsync(usuario);
            await _appDbContext.SaveChangesAsync();
            return usuario.ID;

        }
        /// <summary>
        /// Actualiza Usuario
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        public async Task ActualizarUsuarioAsync(Usuario usuario)
        {
            _appDbContext.Update(usuario);
            await _appDbContext.SaveChangesAsync();
        }
        /// <summary>
        /// Obtiene Usuario por ID incluyendo domicilios activos
        /// </summary>
        /// <param name="usuarioID"></param>
        /// <returns></returns>
        public async Task<Usuario?> ObtenerUsuarioPorIdAsync(int usuarioID)
        {
            return await _appDbContext.Usuarios
                            .Include(p => p.Domicilios.Where(p => p.Activo))
                            .FirstOrDefaultAsync(p => p.Activo && p.ID == usuarioID );
        }
        /// <summary>
        /// Obtiene Usuarios por filtros y paginacion
        /// </summary>
        /// <param name="filtros"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<PaginationResult<Usuario>> ObtenerUsuariosPorFiltrosAsync(Expression<Func<Usuario, bool>> filtros, int page = 1, int pageSize = 20)
        {
            // Construir la consulta base con los filtros
            var query = _appDbContext.Usuarios.Where(filtros);

            var total = await query.CountAsync();
            // Aplicar paginación
            var items = await query
                       .OrderBy(u => u.ID)
                       .Skip((page - 1) * pageSize)
                       .Take(pageSize)
                       .Include(u => u.Domicilios.Where(d => d.Activo))
                       .AsNoTracking()
                       .ToListAsync();

            return new PaginationResult<Usuario>(items, page, pageSize, total);
        }

    }
}
