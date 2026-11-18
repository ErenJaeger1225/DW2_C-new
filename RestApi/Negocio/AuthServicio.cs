using RestApi.Datos;
using RestApi.Entidades;

namespace RestApi.Negocio;

public class AuthServicio(
    IUsuarioReposistorio repo, TokenServicio tokens)
{
    public async Task<LoginRespuesta?> LoginAsync(LoginRequest r)
    {
        // 1. Normalizar correo para búsqueda
        var correo = (r.Correo ?? "").Trim().ToLower();
        var u = await repo.ObtenerPorCorreoAsync(correo);

        if (u is null)
            return null; // El correo no existe en la base de datos

        var inputPassword = (r.Password ?? "").Trim();
        bool passwordValida = false;

        // 2. Intentar verificar mediante BCrypt hash
        try
        {
            passwordValida = BCrypt.Net.BCrypt.Verify(inputPassword, u.PasswordHash);
        }
        catch
        {
            passwordValida = false;
        }

        // 3. Fallback: Si en MySQL la contraseña fue guardada en texto plano (sin hash BCrypt)
        if (!passwordValida && (u.PasswordHash == inputPassword || u.PasswordHash == r.Password))
        {
            passwordValida = true;
            // Migrar automáticamente a un hash BCrypt seguro en la base de datos
            u.PasswordHash = BCrypt.Net.BCrypt.HashPassword(inputPassword);
            u.Activo = true;
            try
            {
                await repo.ActualizarAsync(u);
            }
            catch
            {
                // Si la actualización falla, permitir continuar el inicio de sesión
            }
        }

        // Si el usuario estaba marcado como inactivo pero sus credenciales coinciden, reactivarlo
        if (passwordValida && !u.Activo)
        {
            u.Activo = true;
            try
            {
                await repo.ActualizarAsync(u);
            }
            catch { }
        }

        if (!passwordValida)
            return null; // La contraseña no coincide

        // 4. Generar y devolver el token JWT
        var (token, expira) = tokens.Crear(u);
        return new LoginRespuesta(token, expira,
            new UsuarioDtos(u.Id, u.Nombre, u.Correo, u.Rol));
    }
}
