using System.Linq;
using System.Text.Json;
using UserApi.IntegrationTests.Seed;
using UserDomain.Domain;
using UserInfrastructure.EF;

public static class SeedData
{
    public static void Run(AppDbContext db, string? baseDir = null)
    {

        var root = baseDir ?? AppContext.BaseDirectory;
        var path = Path.Combine(root, "Seed", "seed_usuarios.json");
        if (!File.Exists(path)) throw new FileNotFoundException($"Seed JSON no encontrado: {path}");

        var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var usuariosSeedLst = JsonSerializer.Deserialize<List<UsuarioSeed>>(File.ReadAllText(path), opts) ?? new();

        foreach (var usuarioSeed in usuariosSeedLst)
        {
            if (db.Usuarios.Any(x => x.Email == usuarioSeed.Email)) continue;

            var usuario = new Usuario(usuarioSeed.Nombre, usuarioSeed.Email);
            if (usuarioSeed.Domicilio is not null && usuarioSeed.Domicilio.Any())
            {
                foreach (var dom in usuarioSeed.Domicilio)
                {
                    usuario.AgregarDomicilio(
                        dom.Calle, dom.Numero,
                        dom.Provincia, dom.Ciudad);
                }                
            }
            db.Usuarios.Add(usuario);
        }

        db.SaveChanges();
    }
}
