namespace RestApi.Entidades
{
    public class Usuarios
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Correo { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string Rol { get; set; } = "";
        public bool Activo { get; set; } = true;
    }
}
