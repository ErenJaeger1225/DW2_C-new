namespace RestApi.Web;

using RestApi.Entidades;
using RestApi.Negocio;

/// <summary>
/// Mapeo de rutas y endpoints para la entidad Plato usando ASP.NET Core Minimal APIs.
/// Aplica MapGroup("/platos") y delega toda la lógica de negocio a PlatoServicio.
/// </summary>
public static class PlatoEndpoints
{
    /// <summary>
    /// Registra los endpoints HTTP relacionados con la gestión de platos.
    /// </summary>
    public static void MapPlatoEndpoints(this WebApplication app)
    {
        var grupo = app.MapGroup("/platos");

        // GET /platos - Listado de todos los platos disponibles
        grupo.MapGet("/", async (PlatoServicio s) =>
            Results.Ok(await s.ObtenerTodosAsync()));

        // GET /platos/{id} - Obtener un plato por su Id
        grupo.MapGet("/{id:int}", (int id, PlatoServicio s) =>
            Respuestas.Manejar(async () => Results.Ok(await s.ObtenerPorIdAsync(id))));

        // POST /platos - Crear un nuevo plato (HTTP 201)
        grupo.MapPost("/", (Plato p, PlatoServicio s) =>
            Respuestas.Manejar(async () =>
            {
                p.Id = await s.AgregarAsync(p);
                return Results.Created($"/platos/{p.Id}", p); // 201 Created
            })).RequireAuthorization("SoloAdmin");//nuevo

        // PUT /platos/{id} - Actualizar los datos de un plato existente (HTTP 200)
        grupo.MapPut("/{id:int}", (int id, Plato p, PlatoServicio s) =>
            Respuestas.Manejar(async () =>
            {
                p.Id = id;
                await s.ActualizarAsync(p);
                return Results.Ok(p);
            })).RequireAuthorization("SoloAdmin");//nuevo

        // DELETE /platos/{id} - Borrado lógico de un plato (HTTP 204)
        grupo.MapDelete("/{id:int}", (int id, PlatoServicio s) =>
            Respuestas.Manejar(async () =>
            {
                await s.EliminarAsync(id);
                return Results.NoContent(); // 204 No Content
            })).RequireAuthorization("SoloAdmin");//nuevo
    }
}
