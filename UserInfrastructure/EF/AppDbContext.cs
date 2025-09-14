using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserDomain.Domain;

namespace UserInfrastructure.EF
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Domicilio> Domicilio => Set<Domicilio>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet("utf8mb4").UseCollation("utf8mb4_0900_ai_ci");

            modelBuilder.Entity<Usuario>(p =>
            {
                p.ToTable("Usuarios");
                p.HasKey(x => x.ID);
                p.Property(x => x.ID).ValueGeneratedOnAdd(); 
                p.Property(x => x.Nombre).IsRequired().HasMaxLength(100);
                p.Property(x => x.Email).IsRequired().HasMaxLength(100);
                p.HasIndex(x => x.Email).IsUnique();
                p.Property(x => x.FechaCreacion).IsRequired();
                p.Property(x => x.Activo).IsRequired();
                p.Property(x => x.FechaUltimaActualizacion);

                p.HasMany(x => x.Domicilios)
                    .WithOne(x => x.Usuario)
                    .HasForeignKey(f => f.UsuarioID)
                    .OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<Domicilio>(p =>
            {
                p.ToTable("Domicilio");
                p.HasKey(x => x.ID);
                p.Property(x => x.ID).ValueGeneratedOnAdd();
                p.Property(x => x.Calle).IsRequired().HasMaxLength(150);
                p.Property(x => x.Numero).IsRequired().HasMaxLength(20);
                p.Property(x => x.Provincia).IsRequired().HasMaxLength(100);
                p.Property(x => x.Ciudad).IsRequired().HasMaxLength(100);
                p.Property(x => x.FechaCreacion).IsRequired();
                p.HasIndex(x => x.UsuarioID);
                p.Property(x => x.Activo).IsRequired();
                p.Property(x => x.FechaUltimaActualizacion);

            });
        }
    }
}
