using System.Text.Json;
using UserApi.IntegrationTests.Seed;
using UserDomain.Domain;
using UserInfrastructure.EF;

public static class SeedData
{
    public static void Run(AppDbContext db, string? baseDir = null)
    {
        if (db.Usuarios.Any()) return;

        var root = baseDir ?? AppContext.BaseDirectory; 
        var path = Path.Combine(root, "Seed", "seed_usuarios.json");
        if (!File.Exists(path)) throw new FileNotFoundException($"Seed JSON no encontrado: {path}");

        var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var usuariosSeedLst = JsonSerializer.Deserialize<List<UsuarioSeed>>(File.ReadAllText(path), opts) ?? new();

        foreach (var usuarioSeed in usuariosSeedLst)
        {
            if (db.Usuarios.Any(x => x.Email == usuarioSeed.Email)) continue;

            var usuario = new Usuario(usuarioSeed.Nombre, usuarioSeed.Email);
            if (usuarioSeed.Domicilio is not null)
            {
                usuario.AgregarDomicilio(
                    usuarioSeed.Domicilio.Calle, usuarioSeed.Domicilio.Numero,
                    usuarioSeed.Domicilio.Provincia, usuarioSeed.Domicilio.Ciudad 
                );
            }
            db.Usuarios.Add(usuario);
        }

        db.SaveChanges();
    }
}
