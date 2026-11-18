namespace RestApi.Entidades
{
    public class Pedido
    {
        public int Id { get; set; }
        public int MesaId { get; set; }
        public DateTime Fecha { get; set; }
        public string Estado { get; set; } = "Pendiente";

        public Mesa? Mesa { get; set; }
        public List<DetallePedido> Detalles { get; set; } = [];
    }
}
