// Datos/UsuarioRepositorio.cs
using Microsoft.EntityFrameworkCore;
using RestApi.Entidades;

namespace RestApi.Datos;

public class UsuarioRepositorio(RestauranteContext db)
    : IUsuarioReposistorio

{
    public Task<List<Usuarios>> ListarAsync() =>
        db.Usuarios.AsNoTracking()
          .OrderBy(u => u.Nombre).ToListAsync();

    public Task<Usuarios?> ObtenerPorCorreoAsync(string correo) =>
        db.Usuarios.FirstOrDefaultAsync(
            u => u.Correo == correo);

    public Task<bool> ExisteCorreoAsync(string correo) =>
        db.Usuarios.AnyAsync(u => u.Correo == correo);

    public async Task AgregarAsync(Usuarios u)
    {
        db.Usuarios.Add(u);
        await db.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Usuarios u)
    {
        db.Usuarios.Update(u);
        await db.SaveChangesAsync();
    }
}


