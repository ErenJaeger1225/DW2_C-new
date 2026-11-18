using System.Security.Claims;
using RestApi.Entidades;
using RestApi.Negocio;

namespace RestApi.Web;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(
        this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/auth");

        // LOGIN: público → 200 con el token, 400 si falta el body o 401 si credenciales no coinciden
        g.MapPost("/login", async (LoginRequest? r, AuthServicio s) =>
        {
            if (r is null || string.IsNullOrWhiteSpace(r.Correo) || string.IsNullOrWhiteSpace(r.Password))
            {
                return Results.BadRequest(new { error = "Debe enviar un JSON con 'correo' y 'password'." });
            }

            var res = await s.LoginAsync(r);
            return res is not null
                ? Results.Ok(res)
                : Results.Json(new { error = "Credenciales incorrectas o usuario no encontrado." }, statusCode: 401);
        });

        // REGISTRO: solo el administrador crea cuentas
        g.MapPost("/registro", (RegistroUsuario? r, UsuarioServicio s) =>
            Respuestas.Manejar(async () =>
            {
                if (r is null)
                {
                    return Results.BadRequest(new { error = "El cuerpo de la petición no puede estar vacío." });
                }
                var u = await s.RegistrarAsync(r);
                return Results.Created($"/usuarios/{u.Id}", u);
            }))
         .RequireAuthorization("SoloAdmin");

        // YO: ¿quién soy según mi token?
        g.MapGet("/yo", (ClaimsPrincipal user) => Results.Ok(new
        {
            id = user.FindFirstValue("sub"),
            nombre = user.Identity?.Name,
            rol = user.FindFirstValue("role")
        }))
         .RequireAuthorization();
    }   // fin de MapAuthEndpoints
}   // fin de AuthEndpoints
