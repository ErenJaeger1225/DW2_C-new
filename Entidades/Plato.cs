/*
 *Samuel Sánchez
 *Modulo: ENtidad Plato.cs
 *Descripcion: Este modulo contiene la entidad Plato que permite almacenar la informacion de los platos
 *para enviarlos a la base de datos.
 */

namespace NikoApiRestaurante.Entidades;

public class Plato
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public bool Disponible { get; set; } = true;
    public int CategoriaId { get; set; }
}