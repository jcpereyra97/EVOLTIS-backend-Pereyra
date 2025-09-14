using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using UserApi.Mapper;
using UserApplication.Common.Exceptions;
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

var cs = builder.Configuration.GetConnectionString("Default");

var serverVersion = new MySqlServerVersion(new Version(8, 0, 36)); // MySQL 8.x

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseMySql(cs, serverVersion, o =>
    {
        o.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorNumbersToAdd: null);
    }));


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

var runMigrations = builder.Configuration.GetValue<bool>("RunMigrationsAtStartup");

if (runMigrations)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Fallo ejecutando migrations al inicio.");
        // Opcional: volver a lanzar en Prod; en Dev podés dejar continuar para no bloquear Swagger
        if (!app.Environment.IsDevelopment()) throw;
    }
}


app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var feature = context.Features.Get<IExceptionHandlerFeature>();
        var ex = feature?.Error;

        (int, ProblemDetails) AsDbProblem(Exception e)
        {
            var p = new ProblemDetails
            {
                Title = "Error de base de datos",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "No se pudo completar la operación en la base de datos."
            };

            // --- EN produccion esta informacion deberia estar oculta--//
            p.Extensions["exception"] = e.GetType().FullName;
            p.Extensions["message"] = e.Message;
            //----------------------------------------------------------//

            return (StatusCodes.Status500InternalServerError, p);
        }

        (int status, ProblemDetails problem) = ex switch
        {

            DbUpdateException { InnerException: MySqlException mysql } => AsDbProblem(mysql),

            MySqlException mysql => AsDbProblem(mysql),

            NotFoundException nf => (StatusCodes.Status204NoContent,
                new ProblemDetails
                {
                    Title = "Recurso no encontrado",
                    Status = StatusCodes.Status204NoContent,
                    Detail = nf.Message,
                    Extensions =
                    {
                        ["resource"] = nf.Resource,
                        ["key"] = nf.Key ?? ""
                    }
                }),


            FileNotFoundException
            _ => (
                StatusCodes.Status500InternalServerError,
                new ProblemDetails
                {
                    Title = "Error inesperado",
                    Status = 500,
                    Detail = "Ocurrió un error no controlado."
                }
            )
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(problem);
    });
});



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


public partial class Program { }