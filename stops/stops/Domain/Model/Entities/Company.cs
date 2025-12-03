namespace Frock_backend.stops.Domain.Model.Entities
{
    public class Company
    {
        // El ID viene de Transport, no es autoincremental
        public int Id { get; set; } 
        public string Name { get; set; }
    }
}