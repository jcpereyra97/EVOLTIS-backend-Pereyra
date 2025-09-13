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
            return await _appDbContext.Usuarios
                            .Include(p => p.Domicilios.Where(p => p.Activo))
                            .FirstOrDefaultAsync(p => p.ID == usuarioID);
        }

        public async Task<IEnumerable<Usuario>> ObtenerUsuariosPorFiltrosAsync(Expression<Func<Usuario, bool>> filtros)
        {
            return await _appDbContext.Usuarios
                        .Include(p => p.Domicilios.Where(p => p.Activo))
                        .Where(filtros)
                        .ToListAsync();
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
