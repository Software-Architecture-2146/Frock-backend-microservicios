namespace Frock_backend.routes.Domain.Model.Entities;

public class Company
{

    public int Id { get; set; } 
    public string Name { get; set; }


    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    public int UserId { get; set; }
}