using Microsoft.EntityFrameworkCore;
using GolfTracker.Models;

namespace GolfTracker.Data
{
    public class GolfContext : DbContext
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<Round> Rounds { get; set; }
        public DbSet<HoleScore> HoleScores { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=golftracker.db");
        }
    }
}
