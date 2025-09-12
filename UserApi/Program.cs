using Microsoft.EntityFrameworkCore;
using UserApplication.Interfaces;
using UserApplication.Services;
using UserInfrastructure.EF;
using UserInfrastructure.Interfaces;
using UserInfrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var conn = builder.Configuration.GetConnectionString("MySql");

builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseMySql(conn, ServerVersion.AutoDetect(conn))
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));




builder.Services.AddScoped<IRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

var app = builder.Build();

// Configure the HTTP request pipeline.



app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
