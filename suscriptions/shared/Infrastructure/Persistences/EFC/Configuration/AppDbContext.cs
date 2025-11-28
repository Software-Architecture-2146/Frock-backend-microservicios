using Microsoft.EntityFrameworkCore;
// Asegúrate de que este using coincida con donde creaste tu entidad Suscription
using Frock_backend.suscriptions.domain.model.aggregates; 

namespace suscriptions.shared.Infrastructure.Persistence.EFC.Configuration
{
    // 1. Heredamos de DbContext (la clase base de Entity Framework)
    public class AppDbContext : DbContext
    {
        // 2. Constructor: Necesario para recibir la cadena de conexión desde Program.cs
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // 3. Definimos la Tabla: Aquí le decimos "Quiero una tabla de Suscripciones"
        public DbSet<Suscription> Suscriptions { get; set; }

        // 4. Configuración detallada de la tabla
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Nombre de la tabla en MySQL
            builder.Entity<Suscription>().ToTable("Suscriptions");
            
            // Llave primaria
            builder.Entity<Suscription>().HasKey(s => s.Id);
            
            // Campos obligatorios
            builder.Entity<Suscription>().Property(s => s.CompanyId).IsRequired();
            builder.Entity<Suscription>().Property(s => s.PlanName).IsRequired();
            builder.Entity<Suscription>().Property(s => s.Price).IsRequired();
        }
    }
}