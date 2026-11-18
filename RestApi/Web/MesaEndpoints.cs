using QRCoder;
using RestApi.Entidades;
using RestApi.Negocio;

namespace RestApi.Web
{
    public static class MesaEndpoints
    {
        public static void MapMesaEndpoints(
            this IEndpointRouteBuilder app)
        {
            var g = app.MapGroup("/mesas").RequireAuthorization("SoloAdmin");// nuevo

            g.MapGet("", async (MesaServicio s) =>
                Results.Ok(await s.ListarAsync()));

            g.MapGet("/{id:int}", (int id, MesaServicio s) =>
                Respuestas.Manejar(async () =>
                    Results.Ok(await s.ObtenerAsync(id))));

            g.MapPost("", (Mesa m, MesaServicio s) =>
                Respuestas.Manejar(async () =>
                {
                    m.Id = await s.CrearAsync(m);
                    return Results.Created($"/mesas/{m.Id}", m);
                }));
            g.MapPut("/{id:int}", (int id, Mesa m, MesaServicio s) =>
        Respuestas.Manejar(async () =>
        {
            m.Id = id;
            await s.ActualizarAsync(m);
            return Results.Ok(m);
        }));

            g.MapDelete("/{id:int}", (int id, MesaServicio s) =>
                Respuestas.Manejar(async () =>
                {
                    await s.EliminarAsync(id);
                    return Results.NoContent();
                }));

           

            // Endpoint para visualizar la imagen QR directamente en el navegador (o descargar con ?download=true)
            g.MapGet("/{id:int}/qr", (int id, MesaServicio s, IConfiguration cfg, bool? download) =>
                Respuestas.Manejar(async () =>
                {
                    var mesa = await s.ObtenerAsync(id); // 404 si no existe
                    var baseUrl = cfg["App:UrlMenu"] ?? "http://localhost:5205/menu";
                    var url = $"{baseUrl}?mesa={mesa.Numero}";

                    using var gen = new QRCodeGenerator();
                    using var datos = gen.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
                    var png = new PngByteQRCode(datos).GetGraphic(10);

                    if (download == true)
                    {
                        return Results.File(png, "image/png", $"mesa-{mesa.Numero}.png");
                    }

                    // Sin nombre de descarga para que el navegador lo visualice directamente
                    return Results.File(png, "image/png");
                }));

            // Vista HTML para visualizar el código QR y datos de la mesa en el navegador
            g.MapGet("/{id:int}/visor", (int id, MesaServicio s) =>
                Respuestas.Manejar(async () =>
                {
                    var mesa = await s.ObtenerAsync(id);
                    var html = $$"""
                        <!DOCTYPE html>
                        <html lang="es">
                        <head>
                            <meta charset="UTF-8">
                            <meta name="viewport" content="width=device-width, initial-scale=1.0">
                            <title>Mesa #{{mesa.Numero}} - QR</title>
                            <style>
                                body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; display: flex; justify-content: center; align-items: center; min-height: 100vh; margin: 0; background-color: #f1f5f9; }
                                .card { background: white; padding: 2rem; border-radius: 16px; box-shadow: 0 10px 25px rgba(0,0,0,0.08); text-align: center; max-width: 360px; width: 100%; }
                                h1 { margin: 0 0 0.5rem; color: #1e293b; font-size: 1.75rem; }
                                p { margin: 0.25rem 0 1.25rem; color: #64748b; font-size: 1rem; }
                                .qr-img { width: 240px; height: 240px; border-radius: 12px; border: 1px solid #e2e8f0; }
                                .btn { display: inline-block; margin-top: 1.25rem; padding: 0.6rem 1.2rem; background-color: #2563eb; color: white; text-decoration: none; border-radius: 8px; font-weight: 500; transition: background 0.2s; }
                                .btn:hover { background-color: #1d4ed8; }
                            </style>
                        </head>
                        <body>
                            <div class="card">
                                <h1>Mesa #{{mesa.Numero}}</h1>
                                <p>Capacidad: <strong>{{mesa.Capacidad}}</strong> personas</p>
                                <img class="qr-img" src="/mesas/{{mesa.Id}}/qr" alt="QR Mesa {{mesa.Numero}}" />
                                <br />
                                <a class="btn" href="/mesas/{{mesa.Id}}/qr?download=true">Descargar QR</a>
                            </div>
                        </body>
                        </html>
                        """;
                    return Results.Content(html, "text/html");
                }));
        }

    }
}
