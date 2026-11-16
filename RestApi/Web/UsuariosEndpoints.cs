using RestApi.Entidades;
using RestApi.Negocio;

namespace RestApi.Web;

public static class UsuarioEndpoints
{
    public static void MapUsuarioEndpoints(
        this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/usuarios");

        // GET /usuarios → 200 con la lista de UsuarioDto
        g.MapGet("", async (UsuarioServicio s) =>
            Results.Ok(await s.ListarAsync()));

        // POST /usuarios → 201, 400 o 409
        g.MapPost("", (RegistroUsuario r, UsuarioServicio s) =>
            Respuestas.Manejar(async () =>
            {
                var u = await s.RegistrarAsync(r);
                return Results.Created($"/usuarios/{u.Id}", u);
            }));
    }
}
