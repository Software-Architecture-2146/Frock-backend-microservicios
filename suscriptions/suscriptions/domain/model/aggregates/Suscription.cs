using System;

namespace Frock_backend.suscriptions.domain.model.aggregates
{
    public class Suscription
    {
        public int Id { get; set; }
        
        // Esta es la clave: El ID de la empresa que viene de RabbitMQ
        public int CompanyId { get; set; } 
        
        public string PlanName { get; set; } // Ej: "Free", "Premium"
        public double Price { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsActive { get; set; }

        // Constructor vacío para EF Core
        public Suscription() { }

        // Constructor para crearla fácil
        public Suscription(int companyId, string planName, double price)
        {
            CompanyId = companyId;
            PlanName = planName;
            Price = price;
            StartDate = DateTime.UtcNow;
            IsActive = true;
        }
    }
}