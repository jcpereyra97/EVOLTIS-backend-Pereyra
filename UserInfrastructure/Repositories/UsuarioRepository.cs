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

        public async Task<int> AgregarUsuarioAsync(Usuario usuario)
        {

            await _appDbContext.Set<Usuario>().AddAsync(usuario);
            await _appDbContext.SaveChangesAsync();
            return usuario.ID;

        }

        public async Task ActualizarUsuarioAsync(Usuario usuario)
        {
            _appDbContext.Update(usuario);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<Usuario?> ObtenerUsuarioPorIdAsync(int usuarioID)
        {
            return await _appDbContext.Usuarios
                            .Include(p => p.Domicilios.Where(p => p.Activo))
                            .FirstOrDefaultAsync(p => p.ID == usuarioID);
        }

        public async Task<PaginationResult<Usuario>> ObtenerUsuariosPorFiltrosAsync(Expression<Func<Usuario, bool>> filtros, int page = 1, int pageSize = 20)
        {

            var query = _appDbContext.Usuarios.Where(filtros);

            var total = await query.CountAsync();

            var items = await query
                       .OrderBy(u => u.ID)
                       .Skip((page - 1) * pageSize)
                       .Take(pageSize)
                       .Include(u => u.Domicilios.Where(d => d.Activo))
                       .AsNoTracking()
                       .ToListAsync();

            return new PaginationResult<Usuario>(items, page, pageSize, total);
        }

        public async Task EliminarUsuarioPorId(int usuarioID)
        {
            var usuario = await _appDbContext.FindAsync<Usuario>(usuarioID);
            if (usuario == null)
                throw new Exception();

            _appDbContext.Remove<Usuario>(usuario);
            await _appDbContext.SaveChangesAsync();


        }
    }
}
