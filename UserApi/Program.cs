using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using UserApi.Mapper;
using UserApplication.DTOs;
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


builder.Services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.



app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
