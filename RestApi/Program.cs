using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestApi.Datos;
using RestApi.Entidades;
using RestApi.Negocio;
using RestApi.Web;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// 1. Evitar error de permisos en Windows EventLog ("Cannot open log for source '.NET Runtime'")
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// 2. Cadena de conexión
var cn = builder.Configuration.GetConnectionString("Restaurante");

// 3. Inyección de dependencias (DIP)
// Productos (en memoria)
builder.Services.AddScoped<IProductoRepositorio, ProductoRepositorio>();
builder.Services.AddScoped<ProductoServicio>();

// Platos
builder.Services.AddScoped<IPlatoRepositorio, PlatoRepositorio>();
builder.Services.AddScoped<PlatoServicio>();

// Categorías
builder.Services.AddScoped<ICategoriaRepositorio, CategoriaRepositorio>();
builder.Services.AddScoped<CategoriaServicio>();

// Mesas
builder.Services.AddScoped<IMesaRepositorio, MesaRepositorio>();
builder.Services.AddScoped<MesaServicio>();

// Usuarios
builder.Services.AddScoped<IUsuarioReposistorio, UsuarioRepositorio>();
builder.Services.AddScoped<UsuarioServicio>();

// JWT y Seguridad
builder.Services.AddScoped<TokenServicio>();
builder.Services.AddScoped<AuthServicio>();

// OpenAPI
builder.Services.AddOpenApi();

// 4. DbContext (MySQL) con detección segura para no tumbar la API al iniciar si la BD aún no responde
ServerVersion serverVersion;
try
{
    serverVersion = ServerVersion.AutoDetect(cn);
}
catch
{
    serverVersion = new MySqlServerVersion(new Version(8, 0, 36));
}

builder.Services.AddDbContext<RestauranteContext>(
    o => o.UseMySql(cn, serverVersion));

// 5. Configuración de JWT Authentication y Authorization
var jwt = builder.Configuration.GetSection("Jwt");
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.MapInboundClaims = false;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwt["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt["Key"]!)),
            NameClaimType = "name",
            RoleClaimType = "role"
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("SoloAdmin", p => p.RequireRole(Roles.Administrador))
    .AddPolicy("soloAdmin", p => p.RequireRole(Roles.Administrador));
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("SoloAdmin",           // ya lo tienes
        p => p.RequireRole(Roles.Administrador))
    // NUEVAS: una política por grupo de roles
    .AddPolicy("Personal",
        p => p.RequireRole(Roles.Administrador,
                           Roles.Mesero, Roles.Cocina))
    .AddPolicy("Mesero",
        p => p.RequireRole(Roles.Mesero))
    .AddPolicy("Cocina",
        p => p.RequireRole(Roles.Cocina));
var app = builder.Build();

// Activar OpenAPI en desarrollo
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// 6. Middlewares de seguridad
app.UseAuthentication();
app.UseAuthorization();

// 8. Endpoints adicionales
app.MapGet("/productos", (ProductoServicio servicio) =>
{
    var productosDisponibles = servicio.ListarDisponibles();
    return Results.Ok(productosDisponibles);
});

// 9. Mapeo de grupos de endpoints por entidad
app.MapPlatoEndpoints();
app.MapCategoriaEndpoints();
app.MapMesaEndpoints();
app.MapUsuarioEndpoints();
app.MapAuthEndpoints();

// 10. Semilla inicial: Si no existe ningún usuario registrado, crear Administrador por defecto
using (var scope = app.Services.CreateScope())
{
    var repo = scope.ServiceProvider.GetService<IUsuarioReposistorio>();
    if (repo != null)
    {
        try
        {
            var usuarios = repo.ListarAsync().GetAwaiter().GetResult();
            if (!usuarios.Any())
            {
                repo.AgregarAsync(new Usuarios
                {
                    Nombre = "Administrador",
                    Correo = "admin@itq.ec",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    Rol = Roles.Administrador,
                    Activo = true
                }).GetAwaiter().GetResult();
            }
        }
        catch
        {
            // Continuar si la BD aún no tiene la tabla o está offline
        }
    }
}

app.Run();
