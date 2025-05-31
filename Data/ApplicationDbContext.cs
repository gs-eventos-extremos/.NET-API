using Microsoft.EntityFrameworkCore;
using WeatherEmergencyAPI.Models;

namespace WeatherEmergencyAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets - Tabelas
        public DbSet<User> Users { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<EmergencyContact> EmergencyContacts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // REMOVA ou COMENTE esta linha:
            // modelBuilder.HasDefaultSchema("WEATHER_API");

            // Configuração da entidade User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();

                // Configurar sequência para ID
                entity.Property(e => e.Id)
                    .UseIdentityColumn();
            });

            // Configuração da entidade Location
            modelBuilder.Entity<Location>(entity =>
            {
                entity.HasOne(l => l.User)
                    .WithMany(u => u.SavedLocations)
                    .HasForeignKey(l => l.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.Id)
                    .UseIdentityColumn();
            });

            // Configuração da entidade EmergencyContact
            modelBuilder.Entity<EmergencyContact>(entity =>
            {
                entity.HasOne(e => e.User)
                    .WithMany(u => u.EmergencyContacts)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.Id)
                    .UseIdentityColumn();
            });
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified && e.Entity is User or Location or EmergencyContact);

            foreach (var entry in entries)
            {
                entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}