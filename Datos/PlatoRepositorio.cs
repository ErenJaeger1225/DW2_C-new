/*
 *Samuel Sánchez
 *Modulo: Concexion PlatoRepositorio.cs
 *Descripcion: Este modulo contiene las clausulas que nos permiten conectar y guardar
 *la informacion del plato en la base de datos y tambien manipular dicha informacion
 */

namespace NikoApiRestaurante.Datos;

using MySqlConnector;
using Microsoft.Extensions.Configuration;
using NikoApiRestaurante.Entidades;

public class PlatoRepositorio
{
    private readonly string _cn;

    public PlatoRepositorio(IConfiguration config)
    {
        _cn = config.GetConnectionString("Restaurante")!;
    }

    // 1. GET: Obtener todos (Solo los que estén disponibles/activos)
    public async Task<List<Plato>> ObtenerTodosAsync()
    {
        var lista = new List<Plato>();
        await using var cn = new MySqlConnection(_cn);
        await cn.OpenAsync();

        await using var cmd = new MySqlCommand(
            "SELECT Id, Nombre, Descripcion, Precio, Disponible, CategoriaId FROM Platos WHERE Disponible = 1 ORDER BY Nombre", cn);

        await using var dr = await cmd.ExecuteReaderAsync();
        while (await dr.ReadAsync())
        {
            lista.Add(new Plato
            {
                Id = dr.GetInt32(0),
                Nombre = dr.GetString(1),
                Descripcion = dr.IsDBNull(2) ? null : dr.GetString(2),
                Precio = dr.GetDecimal(3),
                Disponible = dr.GetBoolean(4),
                CategoriaId = dr.GetInt32(5)
            });
        }
        return lista;
    }

    // 2. GET POR ID: Obtener un plato específico
    public async Task<Plato?> ObtenerPorIdAsync(int id)
    {
        await using var cn = new MySqlConnection(_cn);
        await cn.OpenAsync();

        await using var cmd = new MySqlCommand(
            "SELECT Id, Nombre, Descripcion, Precio, Disponible, CategoriaId FROM Platos WHERE Id = @id AND Disponible = 1", cn);
        cmd.Parameters.AddWithValue("@id", id);

        await using var dr = await cmd.ExecuteReaderAsync();
        if (await dr.ReadAsync())
        {
            return new Plato
            {
                Id = dr.GetInt32(0),
                Nombre = dr.GetString(1),
                Descripcion = dr.IsDBNull(2) ? null : dr.GetString(2),
                Precio = dr.GetDecimal(3),
                Disponible = dr.GetBoolean(4),
                CategoriaId = dr.GetInt32(5)
            };
        }
        return null;
    }

    // 3. POST: Agregar un plato nuevo
    public async Task<int> AgregarAsync(Plato p)
    {
        await using var cn = new MySqlConnection(_cn);
        await cn.OpenAsync();

        await using var cmd = new MySqlCommand(
            "INSERT INTO Platos (Nombre, Descripcion, Precio, CategoriaId, Disponible) VALUES (@nombre, @desc, @precio, @cat, 1); SELECT LAST_INSERT_ID();", cn);

        cmd.Parameters.AddWithValue("@nombre", p.Nombre);
        cmd.Parameters.AddWithValue("@desc", (object?)p.Descripcion ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@precio", p.Precio);
        cmd.Parameters.AddWithValue("@cat", p.CategoriaId);

        var idGenerado = await cmd.ExecuteScalarAsync();
        return idGenerado != null ? Convert.ToInt32(idGenerado) : 0;
    }

    // 4. PUT: Actualizar un plato
    public async Task<int> ActualizarAsync(Plato p)
    {
        await using var cn = new MySqlConnection(_cn);
        await cn.OpenAsync();

        await using var cmd = new MySqlCommand(
            "UPDATE Platos SET Nombre = @nombre, Descripcion = @desc, Precio = @precio, CategoriaId = @cat WHERE Id = @id AND Disponible = 1", cn);

        cmd.Parameters.AddWithValue("@id", p.Id);
        cmd.Parameters.AddWithValue("@nombre", p.Nombre);
        cmd.Parameters.AddWithValue("@desc", (object?)p.Descripcion ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@precio", p.Precio);
        cmd.Parameters.AddWithValue("@cat", p.CategoriaId);

        return await cmd.ExecuteNonQueryAsync();
    }

    // 5. DELETE: Borrado Lógico (Cambia Disponible a 0 en lugar de borrar la fila)
    public async Task<int> EliminarLogicoAsync(int id)
    {
        await using var cn = new MySqlConnection(_cn);
        await cn.OpenAsync();

        await using var cmd = new MySqlCommand(
            "UPDATE Platos SET Disponible = 0 WHERE Id = @id", cn);

        cmd.Parameters.AddWithValue("@id", id);
        return await cmd.ExecuteNonQueryAsync();
    }
}