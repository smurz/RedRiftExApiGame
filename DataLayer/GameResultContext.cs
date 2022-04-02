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
        public const string ConnectionString = "GameResultsDb";

        public GameResultContext()
        {}

        public GameResultContext(DbContextOptions<GameResultContext> options)
            : base(options)
        { }

        public DbSet<GameResult> GameResults { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(ConnectionString);
        }
    }
}
