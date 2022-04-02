using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameLogic;
using GameLogic.Base;
using NUnit.Framework;

namespace Tests.GameLogicTests
{
    [TestFixture]
    public class GameTests
    {
        [Test]
        public void GameCanNotStartWithoutPlayers()
        {
            Assert.Throws(typeof(ArgumentNullException), () => new TakeRandomHitGame(null, null, TestHelperMethods.GetTimeSpan()));
        }

        [Test]
        public void GameCanNotStartWithOnePlayer()
        {
            var player = TestHelperMethods.CreatePlayer();
            Assert.Throws(typeof(ArgumentNullException), () => new TakeRandomHitGame(player, null, TestHelperMethods.GetTimeSpan()));
        }

        [Test]
        public void GameCanNotStartWithAnotherPlayer()
        {
            var player = TestHelperMethods.CreatePlayer();
            Assert.Throws(typeof(ArgumentNullException), () => new TakeRandomHitGame(null, player, TestHelperMethods.GetTimeSpan()));
        }

        [Test]
        public void GameStartsAndChangesStateToActive()
        {
            ManualResetEvent eventRaised = new ManualResetEvent(false);

            var player1 = TestHelperMethods.CreatePlayer();
            var player2 = TestHelperMethods.CreatePlayer();
            var game = new TakeRandomHitGame(player1, player2, TestHelperMethods.GetTimeSpan());
            game.GameStateChanged += (sender, state) => 
            {
                if (state == GameState.Active)
                    eventRaised.Set();
            };
            game.Start();

            Assert.IsTrue(eventRaised.WaitOne(TimeSpan.FromSeconds(3)));
        }

        [Test]
        public void GameStartsAndRunsToCompletion()
        {
            ManualResetEvent eventRaised = new ManualResetEvent(false);

            var player1 = TestHelperMethods.CreatePlayer();
            var player2 = TestHelperMethods.CreatePlayer();
            var game = new TakeRandomHitGame(player1, player2, TestHelperMethods.GetTimeSpan());
            game.GameStateChanged += (sender, state) =>
            {
                if (state != GameState.Active)
                    if(state == GameState.GameOver)
                        eventRaised.Set();
            };
            game.Start();

            Assert.IsTrue(eventRaised.WaitOne(TimeSpan.FromSeconds(5)));
        }

        [TestCase(10)]
        public void GameEndsWhenOnlyOnePlayerHealthIsZeroOrLess(int runs)
        {
            var testFlag = true;

            //hits are random. consider several runs to minimize error case.
            for (int i = 0; i < runs; i++)
            {
                ManualResetEvent eventRaised = new ManualResetEvent(false);

                var player1 = TestHelperMethods.CreatePlayer();
                var player2 = TestHelperMethods.CreatePlayer();
                var game = new TakeRandomHitGame(player1, player2, TestHelperMethods.GetTimeSpan());
                game.GameStateChanged += (sender, state) =>
                {
                    if (state == GameState.GameOver)
                    {
                        if ((player1.PlayerHealth <= 0 || player2.PlayerHealth <= 0) &&
                            !(player1.PlayerHealth <= 0 && player2.PlayerHealth <= 0))
                            eventRaised.Set();
                    }
                };
                game.Start();

                testFlag &= eventRaised.WaitOne(TimeSpan.FromMilliseconds(300));
                if(!testFlag) break;
            }
            Assert.IsTrue(testFlag);
        }
    }
}
