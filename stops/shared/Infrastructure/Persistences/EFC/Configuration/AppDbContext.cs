using EntityFrameworkCore.CreatedUpdatedDate.Extensions;
using Frock_backend.shared.Infrastructure.Persistences.EFC.Configuration.Extensions;
using Frock_backend.stops.Domain.Model.Aggregates;
using Frock_backend.stops.Domain.Model.Aggregates.Geographic;
using Frock_backend.stops.Domain.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Frock_backend.shared.Infrastructure.Persistences.EFC.Configuration
{
    public class AppDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Company> Companies { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.AddCreatedUpdatedInterceptor();
            base.OnConfiguring(builder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // REGIÓN, PROVINCE, DISTRICT
            builder.Entity<Region>().HasKey(f => f.Id);
            builder.Entity<Region>().Property(f => f.Id).IsRequired();
            builder.Entity<Region>().Property(f => f.Name).IsRequired();

            builder.Entity<Province>().HasKey(f => f.Id);
            builder.Entity<Province>().Property(f => f.Id).IsRequired();
            builder.Entity<Province>().Property(f => f.Name).IsRequired();
            builder.Entity<Province>().Property(f => f.FkIdRegion).IsRequired();
            builder.Entity<Province>()
                .HasOne<Region>()
                .WithMany()
                .HasForeignKey(p => p.FkIdRegion)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<District>().HasKey(f => f.Id);
            builder.Entity<District>().Property(f => f.Id).IsRequired();
            builder.Entity<District>().Property(f => f.Name).IsRequired();
            builder.Entity<District>().Property(f => f.FkIdProvince).IsRequired();
            builder.Entity<District>()
                .HasOne<Province>()
                .WithMany()
                .HasForeignKey(d => d.FkIdProvince)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.Entity<Stop>(b =>
            {
                b.ToTable("Stops");
                b.HasKey(f => f.Id);
                b.Property(f => f.Id).IsRequired().ValueGeneratedOnAdd();
                b.Property(f => f.Name).IsRequired();
                b.Property(f => f.GoogleMapsUrl).IsRequired(false);
                b.Property(f => f.ImageUrl).IsRequired(false);
                b.Property(f => f.Phone).IsRequired(false);
                b.Property(f => f.Address).IsRequired();
                b.Property(f => f.Reference).IsRequired(false);
                b.Property(f => f.FkIdCompany).IsRequired();
                b.Property(f => f.FkIdDistrict).IsRequired();
            });
            builder.Entity<Company>().ToTable("Companies");
            builder.Entity<Company>().HasKey(c => c.Id);
            builder.Entity<Company>().Property(c => c.Id).ValueGeneratedNever();
            builder.UseSnakeCaseNamingConvention();
        }
    }
}
