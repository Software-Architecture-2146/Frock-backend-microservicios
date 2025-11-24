namespace Frock_backend.routes.Domain.Model.Entities;

public class Company
{
    // Usamos Guid porque así lo definimos en el Contrato (Mensaje)
    public Guid Id { get; set; } 
    public string Name { get; set; }

    // Opcional: Fecha de sincronización para saber cuándo llegó el evento
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}