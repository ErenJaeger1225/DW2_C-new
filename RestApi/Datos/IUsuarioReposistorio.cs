// Datos/IUsuarioRepositorio.cs
using RestApi.Entidades;

namespace RestApi.Datos;

// El contrato: QUÉ sabe hacer el repositorio de usuarios
public interface IUsuarioReposistorio
{
    Task<List<Usuarios>> ListarAsync();
    Task<Usuarios?> ObtenerPorCorreoAsync(string correo);
    Task<bool> ExisteCorreoAsync(string correo);
    Task AgregarAsync(Usuarios u);
}