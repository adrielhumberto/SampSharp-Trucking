using GamemodeDatabase.Models;
using Microsoft.EntityFrameworkCore;

namespace GamemodeDatabase
{
    public class GamemodeContext : DbContext
    {
        public DbSet<PlayerModel> Players { get; set; }
        public DbSet<PlayerBanModel> Bans { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("Server=localhost;Database=trucking;Uid=root;Pwd=;");

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerModel>(entity =>
            {
                entity.Property(e => e.PositionX)
                    .HasDefaultValue(1470.9402);

                entity.Property(e => e.PositionY)
                    .HasDefaultValue(974.7820);

                entity.Property(e => e.PositionZ)
                    .HasDefaultValue(10.8203);
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}