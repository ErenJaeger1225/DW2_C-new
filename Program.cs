/*
 *Samuel Sánchez
 *Modulo: Program.cs
 *Descripcion: Este modulo contiene la configuracion de la aplicacion, asi como la conexion a la base de datos.
 */

//Importamos las entidades, repositorios y servicios
using NikoApiRestaurante.Datos;
using NikoApiRestaurante.Entidades;
using NikoApiRestaurante.Servicios;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//inyeccion de dependencias 
// solocitamos a .NET que instancie el repositorio y el servicio cuando se necesite
builder.Services.AddScoped<PlatoRepositorio>();
builder.Services.AddScoped<PlatoServicio>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();





app.Run();
