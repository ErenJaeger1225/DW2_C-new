namespace RestApi.Datos;

using Microsoft.EntityFrameworkCore;
using RestApi.Entidades;

public class RestauranteContext(
    DbContextOptions<RestauranteContext> options)
    : DbContext(options)
{
    public DbSet<Plato> Platos => Set<Plato>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Mesa> Mesas => Set<Mesa>();
    
    public DbSet<Usuarios>Usuarios => Set<Usuarios>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<DetallePedido> Detalles
        => Set<DetallePedido>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Plato>(e =>
        {
            e.Property(p => p.Precio).HasPrecision(10, 2);
            e.Property(p => p.Descripcion).IsRequired(false);
            e.Ignore(p => p.Categoria);   // viene del JOIN
            e.HasQueryFilter(p => p.Disponible == 1);
        });

        // en OnModelCreating
        mb.Entity<DetallePedido>(e =>
        {
            e.ToTable("DetallesPedido");   // nombre real
            e.Property(d => d.PrecioUnitario)
             .HasPrecision(10, 2);
        });
        mb.Entity<Pedido>()
          .HasMany(p => p.Detalles)
          .WithOne()
          .HasForeignKey(d => d.PedidoId);

    }
}
