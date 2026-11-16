using RestApi.Datos;
using RestApi.Entidades;

namespace RestApi.Negocio;

public class UsuarioServicio(IUsuarioReposistorio repo)
{
    public async Task<List<UsuarioDtos>> ListarAsync() =>
        (await repo.ListarAsync()).Select(ADto).ToList();

    public async Task<UsuarioDtos> RegistrarAsync(RegistroUsuario r)
    {
        // 1. Validar: cada regla rota es un 400
        if (string.IsNullOrWhiteSpace(r.Nombre))
            throw new ArgumentException("El nombre es obligatorio.");
        if (string.IsNullOrWhiteSpace(r.Correo) || !r.Correo.Contains('@'))
            throw new ArgumentException("El correo no es válido.");
        if (r.Password is null || r.Password.Length < 8)
            throw new ArgumentException("Mínimo 8 caracteres.");
        if (!Roles.Todos.Contains(r.Rol))
            throw new ArgumentException(
                "Rol válido: Administrador, Mesero o Cocina.");
        // 2. Normalizar: ANA@itq.ec y ana@itq.ec son el mismo
        var correo = r.Correo.Trim().ToLower();
        if (await repo.ExisteCorreoAsync(correo))
            throw new InvalidOperationException(
                "Ese correo ya está registrado.");      // 409

        // 3. Cifrar y guardar
        var u = new Usuarios
        {
            Nombre = r.Nombre.Trim(),
            Correo = correo,
            Rol = r.Rol,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(r.Password)
        };
        await repo.AgregarAsync(u);
        return ADto(u);
    }   // fin de RegistrarAsync

    // Convierte la entidad en DTO: el hash se queda aquí
    private static UsuarioDtos ADto(Usuarios u) =>
        new(u.Id, u.Nombre, u.Correo, u.Rol);
}   // fin de UsuarioServicio