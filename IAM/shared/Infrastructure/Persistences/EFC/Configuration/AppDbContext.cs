using EntityFrameworkCore.CreatedUpdatedDate.Extensions;
using Frock_backend.shared.Infrastructure.Persistences.EFC.Configuration.Extensions;
using Frock_backend.IAM.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Frock_backend.shared.Infrastructure.Persistences.EFC.Configuration
{
    public class AppDbContext(DbContextOptions options) : DbContext(options)
    {
        // --- 1. ESTO ES LO QUE FALTABA (La Tabla) ---
        public DbSet<User> Users { get; set; }
        // -------------------------------------------

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.AddCreatedUpdatedInterceptor();
            base.OnConfiguring(builder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // IAM Context Configuration
            builder.Entity<User>().ToTable("Users"); // Es buena práctica poner el nombre explícito
            
            builder.Entity<User>().HasKey(u => u.Id);
            builder.Entity<User>().Property(u => u.Id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<User>().Property(u => u.Username).IsRequired();
            builder.Entity<User>().Property(u => u.PasswordHash).IsRequired();
            
            // Si tu Role es un Enum, esto lo convierte a texto. Si es string, no hace daño.
            builder.Entity<User>().Property(u => u.Role).HasConversion<string>().IsRequired();

            // --- IMPORTANTE PARA IAM: ---
            // El Username debe ser único. No pueden haber dos usuarios con el mismo nombre.
            builder.Entity<User>().HasIndex(u => u.Username).IsUnique(); 
            
            builder.UseSnakeCaseNamingConvention();
        }
    }
}