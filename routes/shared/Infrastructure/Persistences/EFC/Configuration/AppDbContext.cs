using EntityFrameworkCore.CreatedUpdatedDate.Extensions;
using Frock_backend.routes.Domain.Model.Aggregates;
using Frock_backend.routes.Domain.Model.Entities;
using Frock_backend.shared.Infrastructure.Persistences.EFC.Configuration.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Frock_backend.shared.Infrastructure.Persistences.EFC.Configuration
{
    public class AppDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Company> Companies { get; set; }
        public DbSet<Stop> Stops { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.AddCreatedUpdatedInterceptor();
            base.OnConfiguring(builder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Company>().ToTable("Companies");
            builder.Entity<Company>().HasKey(p => p.Id);
            
            // CONFIGURACIÓN STOP
            builder.Entity<Stop>().ToTable("Stops");
            builder.Entity<Stop>().HasKey(p => p.Id);
            builder.Entity<Stop>().Property(p => p.Id).ValueGeneratedNever();
            // ROUTE
            builder.Entity<RouteAggregate>(b =>
            {
                b.ToTable("Routes");
                b.HasKey(r => r.Id);
                b.Property(r => r.Price).IsRequired();
                b.Property(r => r.Duration).IsRequired();
                b.Property(r => r.Frequency).IsRequired();

                // 2) Schedule como entidad hija (1-N)
                b.HasMany(r => r.Schedules)
                 .WithOne() // si no navegas hacia RouteAggregate
                 .HasForeignKey("RouteId") // string por shadow FK
                 .OnDelete(DeleteBehavior.Cascade);

                // RouteStops (join table)
                b.HasMany(r => r.Stops)
                 .WithOne() // relación a RouteAggregate (no objeto, solo FKs)
                 .HasForeignKey("FKRouteId")
                 .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<RoutesStops>(b =>
            {
                b.ToTable("RouteStops");
                b.HasKey(rs => rs.Id);
                b.Property(rs => rs.FkStopId).IsRequired();
                b.Property(rs => rs.FKRouteId).IsRequired();
                // NO uses relaciones directas con Stop, solo IDs. Así el microservicio permanece desacoplado.
            });

            builder.Entity<Schedule>(b =>
            {
                b.ToTable("Schedules");
                b.HasKey(s => s.Id);
                b.Property(s => s.StartTime).IsRequired();
                b.Property(s => s.EndTime).IsRequired();
                b.Property(s => s.DayOfWeek).HasMaxLength(10);
                b.Property(s => s.Enabled).IsRequired();
            });

            builder.UseSnakeCaseNamingConvention();
        }
    }
}
