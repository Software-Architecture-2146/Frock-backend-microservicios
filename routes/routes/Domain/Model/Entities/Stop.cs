namespace Frock_backend.routes.Domain.Model.Entities
{
    public class Stop
    {
        public int Id { get; set; } // No es autoincremental, es el ID que viene de 'stops'
        public string Name { get; set; }
        public string Address { get; set; }
    }
}