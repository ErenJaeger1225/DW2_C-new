/*
 *Samuel Sánchez
 *Modulo: PlatoEndpoints.cs
 *Descripcion: Este modulo contiene los endpoints para el CRUD de platos.
 */

using NikoApiRestaurante.Datos;
using NikoApiRestaurante.Entidades;
using NikoApiRestaurante.Servicios;

namespace NikoApiRestaurante.Web
{

    public static class PlatoEndpoints
    {

        public static void MapPlatoEndpoints(this IEndpointRouteBuilder app)
        {

            //endpoints para el crud
            // 1. GET: Listar todos los platos activos
            app.MapGet("/api/platos", async (PlatoServicio servicio) =>
                Results.Ok(await servicio.ObtenerTodosAsync(int id)));

            // 2. GET POR ID: Obtener un plato por su código
            app.MapGet("/api/platos/{id}", async (int id, PlatoServicio servicio) =>
            {
                var plato = await servicio.ObtenerPorIdAsync(id);
                return plato is not null ? Results.Ok(plato) : Results.NotFound(new { error = "Plato no encontrado." });
            });

            // 3. POST: Crear un nuevo plato
            app.MapPost("/api/platos", async (Plato p, PlatoServicio servicio) =>
            {
                try
                {
                    int nuevoId = await servicio.AgregarAsync(p);
                    p.Id = nuevoId;
                    p.Disponible = true;
                    return Results.Created($"/api/platos/{p.Id}", p);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            });

            // 4. PUT: Actualizar un plato
            app.MapPut("/api/platos/{id}", async (int id, Plato p, PlatoServicio servicio) =>
            {
                if (id != p.Id) return Results.BadRequest(new { error = "El ID de la ruta no coincide con el objeto." });
                try
                {
                    await servicio.ActualizarAsync(p);
                    return Results.NoContent();
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            });

            // 5. DELETE: Borrado Lógico (Cambia Disponible a 0)
            app.MapDelete("/api/platos/{id}", async (int id, PlatoServicio servicio) =>
            {
                try
                {
                    await servicio.EliminarLogicoAsync(id);
                    return Results.NoContent();
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            });
        }
    }
}
