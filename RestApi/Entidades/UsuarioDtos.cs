namespace RestApi.Entidades;

using System.Text.Json.Serialization;

// Lo que ENTRA al registrar
public record RegistroUsuario(
    string Nombre,
    string Correo,
    [property: JsonConverter(typeof(StringOrNumberConverter))] string Password,
    string Rol);

// Lo que SALE de la API: sin PasswordHash
public record UsuarioDtos(
    int Id,
    string Nombre,
    string Correo,
    string Rol);
