using EntityFrameworkCore.CreatedUpdatedDate.Extensions;
using Frock_backend.shared.Infrastructure.Persistences.EFC.Configuration.Extensions;
using Frock_backend.transport_Company.Domain.Model.Aggregates;
using Frock_backend.transport_Company.Domain.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Frock_backend.shared.Infrastructure.Persistences.EFC.Configuration
{
    public class AppDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.AddCreatedUpdatedInterceptor();
            base.OnConfiguring(builder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // COMPANY
            builder.Entity<Companies>(b =>
            {
                b.HasKey(f => f.Id);
                b.Property(f => f.Id).IsRequired().ValueGeneratedOnAdd();
                b.Property(f => f.Name).IsRequired();
                b.Property(f => f.LogoUrl).IsRequired();
                b.Property(f => f.FkIdUser).IsRequired();
            });
            builder.Entity<User>(b =>
            {
                b.ToTable("Users"); // Nombre de la tabla
                b.HasKey(u => u.Id);
                b.Property(u => u.Id).ValueGeneratedNever(); 
                
                b.Property(u => u.Username).IsRequired();
            });
            builder.UseSnakeCaseNamingConvention();
        }
    }
}