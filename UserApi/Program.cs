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
            // EF suele envolver la excepción del proveedor
            DbUpdateException { InnerException: MySqlException mysql } => AsDbProblem(mysql),

            // MySqlException lanzada “cruda”
            MySqlException mysql => AsDbProblem(mysql),

            NotFoundException nf => (StatusCodes.Status404NotFound,
                new ProblemDetails
                {
                    Title = "Recurso no encontrado",
                    Status = StatusCodes.Status404NotFound,
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
