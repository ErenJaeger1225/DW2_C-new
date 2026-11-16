namespace RestApi.Entidades
{
    public static class Roles
    {
        public const string Administrador = "Administrador";
        public const string Mesero = "Mesero";
        public const string Cocina = "Cocina";

        public static readonly string[] Todos =
            [Administrador, Mesero, Cocina];
    }
}
