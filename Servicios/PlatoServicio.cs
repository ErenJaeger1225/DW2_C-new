/*
 *Samuel Sánchez
 *Modulo: PlatoServicio.cs
 *Descripcion: Este modulo contiene la logica de negocio para el manejo de los platos, validando los datos antes de enviarlos a la base de datos.
 */

namespace NikoApiRestaurante.Servicios;

using MySqlConnector;
using NikoApiRestaurante.Datos;
using NikoApiRestaurante.Entidades;

public class PlatoServicio(PlatoRepositorio repo)
{
    public Task<List<Plato>> ListarAsync() => repo.ObtenerTodosAsync();
    private readonly PlatoRepositorio _repo = repo;

    public Task<List<Plato>> ObtenerTodosAsync(int id) => _repo.ObtenerTodosAsync();

    public Task<Plato?> ObtenerPorIdAsync(int id) => _repo.ObtenerPorIdAsync(id);

    public async Task<int> AgregarAsync(Plato p)
    {
        if (string.IsNullOrWhiteSpace(p.Nombre))
            throw new ArgumentException("El nombre del plato es obligatorio.");
        if (p.Precio <= 0)
            throw new ArgumentException("El precio debe ser mayor a 0.");
        if (p.CategoriaId <= 0)
            throw new ArgumentException("Debe indicar una categoría válida.");

        p.Nombre = p.Nombre.Trim();

        try
        {
            return await _repo.AgregarAsync(p);
        }
        catch (MySqlException ex) when (ex.Number == 1452)
        {
            throw new ArgumentException($"La categoría con ID {p.CategoriaId} no existe en la base de datos.");
        }
    }

    public async Task ActualizarAsync(Plato p)
    {
        if (p.Id <= 0) throw new ArgumentException("ID de plato inválido.");
        if (string.IsNullOrWhiteSpace(p.Nombre)) throw new ArgumentException("El nombre es obligatorio.");
        if (p.Precio <= 0) throw new ArgumentException("El precio debe ser mayor a 0.");

        var filas = await _repo.ActualizarAsync(p);
        if (filas == 0) throw new ArgumentException($"No se encontró el plato con ID {p.Id} o ya está inactivo.");
    }

    public async Task EliminarLogicoAsync(int id)
    {
        if (id <= 0) throw new ArgumentException("ID de plato inválido.");
        var filas = await _repo.EliminarLogicoAsync(id);
        if (filas == 0) throw new ArgumentException($"No se encontró ningún plato activo con el ID {id}.");
    }

   
}