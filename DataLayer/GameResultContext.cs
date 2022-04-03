using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.GameContext;
using Microsoft.EntityFrameworkCore;

namespace DataLayer
{
    public class GameResultContext : DbContext
    {

        public GameResultContext(DbContextOptions<GameResultContext> options)
            : base(options)
        { }

        public DbSet<GameResult> GameResults { get; set; } = null!;

#if DEBUG
        //for tests
        public GameResultContext()
        { }

        //for tests
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "Host=localhost;Database=ExApiGame;Username=postgres;Password=cvehp1988";
            optionsBuilder.UseNpgsql(connectionString);
        }
#endif
    }
}
