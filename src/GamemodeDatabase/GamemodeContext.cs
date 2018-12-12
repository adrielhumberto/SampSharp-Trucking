using GamemodeDatabase.Models;
using Microsoft.EntityFrameworkCore;

namespace GamemodeDatabase
{
    public class GamemodeContext : DbContext
    {
        public DbSet<PlayerModel> Players { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("Server=localhost;Database=trucking;Uid=root;Pwd=;");

            base.OnConfiguring(optionsBuilder);
        }
    }
}