

using RestApi.Datos;
using RestApi.Negocio;
using RestApi.Web;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var cn = builder.Configuration
    .GetConnectionString("Restaurante");

// ==========================================
// Registro de dependencias en el contenedor DI
// Siguiendo el principio de inversión de dependencias (DIP):
// Los servicios de negocio dependen de interfaces de repositorio.
// ==========================================

// 1. Productos
builder.Services.AddScoped<IProductoRepositorio, ProductoRepositorio>();
builder.Services.AddScoped<ProductoServicio>();

builder.Services.AddDbContext<RestauranteContext>(
    o => 
    o.UseMySql(cn, ServerVersion.AutoDetect(cn)));

// 2. Categorías
builder.Services.AddScoped<ICategoriaRepositorio, CategoriaRepositorio>();
builder.Services.AddScoped<CategoriaServicio>();
builder.Services.AddScoped<IMesaRepositorio, MesaRepositorio>();
builder.Services.AddScoped<MesaServicio>();
builder.Services.AddScoped<IUsuarioReposistorio, UsuarioRepositorio>();
builder.Services.AddScoped<UsuarioServicio>();


// 3. Platos
builder.Services.AddScoped<IPlatoRepositorio, PlatoRepositorio>();
builder.Services.AddScoped<PlatoServicio>();

var app = builder.Build();

// ==========================================
// Mapeo de Endpoints (Minimal API)
// ==========================================

// Endpoint raíz con enlaces para fácil navegación en navegador
app.MapGet("/", () => Results.Content("""
    <!DOCTYPE html>
    <html lang="es">
    <head>
        <meta charset="UTF-8">
        <title>RestApi - Restaurante ITQ</title>
        <style>
            body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; max-width: 650px; margin: 40px auto; padding: 20px; line-height: 1.6; background-color: #f8fafc; color: #1e293b; }
            .card { background: white; padding: 24px; border-radius: 12px; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.1); }
            h1 { color: #0f172a; margin-top: 0; }
            ul { list-style: none; padding-left: 0; }
            li { margin-bottom: 12px; padding: 10px; background: #f1f5f9; border-radius: 8px; }
            a { color: #2563eb; font-weight: 600; text-decoration: none; }
            a:hover { text-decoration: underline; }
            .badge { display: inline-block; font-size: 0.75rem; background: #e2e8f0; padding: 2px 8px; border-radius: 9999px; margin-left: 6px; }
        </style>
    </head>
    <body>
        <div class="card">
            <h1>🍽️ Restaurante ITQ - API</h1>
            <p>Selecciona un endpoint para visualizar en el navegador:</p>
            <ul>
                <li><a href="/mesas/1/visor">/mesas/1/visor</a> <span class="badge">Recomendado</span> - Visor interactivo del QR</li>
                <li><a href="/mesas/1/qr">/mesas/1/qr</a> - Imagen PNG directa del QR</li>
                <li><a href="/mesas">/mesas</a> - Listado de mesas en JSON</li>
                <li><a href="/platos">/platos</a> - Catálogo de platos en JSON</li>
                <li><a href="/categorias">/categorias</a> - Categorías en JSON</li>
                <li><a href="/productos">/productos</a> - Productos en memoria</li>
            </ul>
        </div>
    </body>
    </html>
""", "text/html"));

// Endpoint en memoria para productos disponibles
app.MapGet("/productos", (ProductoServicio servicio) =>
{
    var productosDisponibles = servicio.ListarDisponibles();
    return Results.Ok(productosDisponibles);
});

// Grupos de endpoints organizados por entidad
app.MapCategoriaEndpoints();
app.MapPlatoEndpoints();
app.MapMesaEndpoints();
app.MapUsuarioEndpoints();
app.Run();
