using Microsoft.EntityFrameworkCore;
using MyLocations.Core.Location;

namespace MyLocations.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Location> Locations { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Setting> Settings { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");

            // Locations
            modelBuilder.Entity<Location>(location =>
            {
                location.ToTable("Location").HasKey(l => l.Id);
                location.Property(l => l.Id).HasColumnName("Id").UseIdentityColumn();

                location.Property(l => l.CreatedAt).HasColumnName("CreatedAt").IsRequired().HasDefaultValueSql("getdate()");

                location.OwnsOne(l => l.Description, d =>
                {
                    d.Property(d => d.Name).HasColumnName("Name").HasMaxLength(100);
                    d.Property(d => d.Region).HasColumnName("Region").HasMaxLength(100);
                    d.Property(d => d.Address).HasColumnName("Address").HasMaxLength(100);
                    d.Property(d => d.Keyword).HasColumnName("Keyword").IsRequired().HasMaxLength(100);
                });

                location.OwnsOne(l => l.GeoCodes, g =>
                {
                    g.Property(g => g.Latitude).HasColumnName("Latitude").IsRequired();
                    g.Property(g => g.Longitude).HasColumnName("Longitude").IsRequired();
                    g.HasIndex(g => new { g.Latitude, g.Longitude }).IsUnique();
                });

                location.HasOne(l => l.Category).WithMany(c => c.Locations);
            });

            // Images
            modelBuilder.Entity<Image>(image =>
            {
                image.ToTable("Image").HasKey(i => i.Id);
                image.Property(i => i.Id).HasColumnName("Id").HasMaxLength(100);

                image.Property(i => i.Description).HasColumnName("Description").HasMaxLength(100);

                image.Property(i => i.Url).HasColumnName("Url").IsRequired().HasMaxLength(100);

                image.HasIndex(i => i.Url).IsUnique();

                image.HasMany(i => i.Locations).WithMany(l => l.Images).UsingEntity(e =>
                {
                    e.ToTable("LocationImage");
                    e.Property("LocationsId").HasColumnName("LocationId");
                    e.Property("ImagesId").HasColumnName("ImageId");
                });
            });

            // Location categories
            modelBuilder.Entity<Category>(locationCategory =>
            {
                locationCategory.ToTable("LocationCategory").HasKey(l => l.Id);
                locationCategory.Property(l => l.Id).HasColumnName("Id");

                locationCategory.Property(l => l.Name).HasColumnName("Name").IsRequired().HasMaxLength(100);
            });


            // Settings
            modelBuilder.Entity<Setting>(setting =>
            {
                setting.ToTable("Setting").HasKey(s => s.Key);
                setting.Property(s => s.Key).HasColumnName("Key").HasMaxLength(100);

                setting.Property(s => s.Value).HasColumnName("Value").IsRequired().HasMaxLength(100);
            });
        }
    }
}

