using Microsoft.EntityFrameworkCore;
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

        public async Task AgregarUsuarioAsync(Usuario usuario)
        {
            await _appDbContext.AddAsync(usuario);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task ActualizarUsuarioAsync(Usuario usuario)
        {
            _appDbContext.Update(usuario);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<Usuario?> ObtenerUsuarioPorIdAsync(int usuarioID)
        {
            return await _appDbContext.FindAsync<Usuario>(usuarioID);
        }

        public async Task<IEnumerable<Usuario>> ObtenerUsuariosPorFiltrosAsync(Expression<Func<Usuario, bool>> predicate)
        {
            return await _appDbContext.Usuarios
                        .Include(p => p.Domicilios)
                        .Where(predicate)
                        .ToListAsync();
        }
    }
}
