
using Docker.DotNet.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UserApi;
using UserInfrastructure.EF;


public class CustomWebApplicationFactory
  : WebApplicationFactory<Program>
{
    private readonly string _conn;

    public CustomWebApplicationFactory(string conn) => _conn = conn;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Reemplazar DbContext por el de test
            var descriptor = services.Single(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(opt =>
                opt.UseMySql(_conn, ServerVersion.AutoDetect(_conn)));

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();
            SeedData.Run(db); // tus semillas

        });
    }
}
