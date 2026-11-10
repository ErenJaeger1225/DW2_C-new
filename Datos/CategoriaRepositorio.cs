/*
 *Samuel Sánchez
 *Modulo: CategoriaRepositorio.cs
 *Descripcion: Este modulo contiene la interfaz que conecta con mysql y érmita obtener y 
 *modificar la informacion de la base de datos
 */

using MySqlConnector;
using NikoApiRestaurante.Entidades;

namespace NikoApiRestaurante.Datos
{
    public class CategoriaRepositorio
    (IConfiguration config)
    {
        private readonly string _cn = config.GetConnectionString("Restaurante")!;

        public async Task<List<Categoria>> ObtenerTodasAsync()
        {
            var lista = new List<Categoria>();
            await using var cn = new MySqlConnection(_cn);
            await cn.OpenAsync();
            await using var cmd = new MySqlCommand(
                "SELECT Id, Nombre FROM Categorias ORDER BY Nombre", cn);
            await using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync())
                lista.Add(new Categoria { Id = dr.GetInt32(0), Nombre = dr.GetString(1) });
            return lista;
        }
        public async Task<Categoria?> ObtenerPorIdAsync(int id)
        {
            await using var cn = new MySqlConnection(_cn);
            await cn.OpenAsync();
            await using var cmd = new MySqlCommand(
                "SELECT Id, Nombre FROM Categorias WHERE Id = @id", cn);
            cmd.Parameters.AddWithValue("@id", id);
            await using var dr = await cmd.ExecuteReaderAsync();
            if (!await dr.ReadAsync()) return null;
            return new Categoria { Id = dr.GetInt32(0), Nombre = dr.GetString(1) };
        }

        public async Task<int> AgregarAsync(Categoria c)
        {
            await using var cn = new MySqlConnection(_cn);
            await cn.OpenAsync();
            await using var cmd = new MySqlCommand(
                "INSERT INTO Categorias (Nombre) VALUES (@nombre)", cn);
            cmd.Parameters.AddWithValue("@nombre", c.Nombre);
            await cmd.ExecuteNonQueryAsync();
            return (int)cmd.LastInsertedId;   // el Id que generó MySQL
        }
        public async Task<int> ActualizarAsync(Categoria c)
        {
            await using var cn = new MySqlConnection(_cn);
            await cn.OpenAsync();
            await using var cmd = new MySqlCommand(
                "UPDATE Categorias SET Nombre = @nombre WHERE Id = @id", cn);
            cmd.Parameters.AddWithValue("@nombre", c.Nombre);
            cmd.Parameters.AddWithValue("@id", c.Id);
            return await cmd.ExecuteNonQueryAsync();   // filas afectadas
        }

        public async Task<int> EliminarAsync(int id)
        {
            await using var cn = new MySqlConnection(_cn);
            await cn.OpenAsync();
            await using var cmd = new MySqlCommand(
                "DELETE FROM Categorias WHERE Id = @id", cn);
            cmd.Parameters.AddWithValue("@id", id);
            return await cmd.ExecuteNonQueryAsync();
        }

        // ¿Cuántos platos usan esta categoría?
        public async Task<int> ContarPlatosAsync(int id)
        {
            await using var cn = new MySqlConnection(_cn);
            await cn.OpenAsync();
            await using var cmd = new MySqlCommand(
                "SELECT COUNT(*) FROM Platos WHERE CategoriaId = @id", cn);
            cmd.Parameters.AddWithValue("@id", id);
            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }
    }
}
