using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using DataLayer.GameContext;
using NUnit.Framework;

namespace Tests.DataLayerTests
{
    [TestFixture]
    public class ContextUnitTest
    {
        [Test]
        public async Task IsGameResultStoredCorrectly()
        {
            var id = Guid.NewGuid().ToString();
            var p1Name = "nagibator";
            var p2Name = "some Wierd _name! 1231 !23 4455  ((( &^%$ [] jasdhskjlaskdgv ;;dljsjd ll;sdf'p adupq87y h??? <>";
            var health1 = 10;
            var health2 = int.MinValue;

            // instantiate short life db connection to write game result
            await using (var dbContext = new GameResultContext())
            {
                var gameResult = new GameResult()
                {
                    Id = id,
                    PlayerHostName = p1Name,
                    PlayerHostHealth = health1,
                    PlayerGuestName = p2Name,
                    PlayerGuestHealth = health2
                };

                dbContext.GameResults.Add(gameResult);
                await dbContext.SaveChangesAsync().ConfigureAwait(false);
            }

            GameResult? find;
            //instantiate another connection
            await using (var dbContext = new GameResultContext())
            {
                find = await dbContext.GameResults.FindAsync(id);
            }

            Assert.IsTrue(
                find != null &&
                find.Id == id &&
                find.PlayerHostName == p1Name &&
                find.PlayerGuestName == p2Name &&
                find.PlayerHostHealth == health1 &&
                find.PlayerGuestHealth == health2);
        }
    }
}
