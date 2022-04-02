using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataLayer;
using DataLayer.GameContext;
using GameLogic;
using GameLogic.Base;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Tests.GameLogicTests
{
    [TestFixture]
    public class LobbiesHostTests
    {
        private ILobbiesHost _lobbiesHost;

        [SetUp]
        public void SetUp()
        {
            var contextFactoryMock = new Mock<IDbContextFactory<GameResultContext>>();
            contextFactoryMock.Setup(cf => cf.CreateDbContextAsync(CancellationToken.None))
                .ReturnsAsync(new GameResultContext());

            _lobbiesHost = new LobbiesHost(contextFactoryMock.Object);
        }

        [Test]
        public void LobbyIsCreated()
        {
            var lobby = _lobbiesHost.CreateLobby(TestHelperMethods.CreatePlayer());

            Assert.NotNull(lobby);
        }

        [Test]
        public void DefaultLobbyTimeIs1Second()
        {
            var lobby = _lobbiesHost.CreateLobby(TestHelperMethods.CreatePlayer()) as Lobby;

            Assert.IsTrue(lobby?.GameTick.TotalSeconds == 1d);
        }

        [Test]
        public void PlayerCanJoinLobby()
        {
            var lobby = _lobbiesHost.CreateLobby(TestHelperMethods.CreatePlayer());

            var playerGuest = TestHelperMethods.CreatePlayer();
            var response = lobby.TryJoinPlayer(playerGuest);

            Assert.IsTrue(response == JoinLobbyResponse.Success);
        }

        [Test]
        public void ThirdPlayerCantJoinLobby()
        {
            var lobby = _lobbiesHost.CreateLobby(TestHelperMethods.CreatePlayer());

            var playerGuest = TestHelperMethods.CreatePlayer();
            var playerWannaJoin = TestHelperMethods.CreatePlayer();

            lobby.TryJoinPlayer(playerGuest);
            var wannaJoinResponse = lobby.TryJoinPlayer(playerWannaJoin);

            Assert.IsTrue(wannaJoinResponse == JoinLobbyResponse.LobbyIsFull);
        }

        [Test]
        public void PlayerCanJoinLobbyById()
        {
            var lobby = _lobbiesHost.CreateLobby(TestHelperMethods.CreatePlayer());

            var playerGuest = TestHelperMethods.CreatePlayer();

            var response = _lobbiesHost.JoinLobbyById(playerGuest, lobby.Id);

            Assert.IsTrue(response == JoinLobbyResponse.Success);
        }

        [Test]
        public void ThirdPlayerCantJoinLobbyById()
        {
            var lobby = _lobbiesHost.CreateLobby(TestHelperMethods.CreatePlayer());

            var playerGuest = TestHelperMethods.CreatePlayer();
            var playerWannaJoin = TestHelperMethods.CreatePlayer();

            var response = _lobbiesHost.JoinLobbyById(playerGuest, lobby.Id);
            var wannaJoinResponse = _lobbiesHost.JoinLobbyById(playerWannaJoin, lobby.Id);

            Assert.IsTrue(wannaJoinResponse == JoinLobbyResponse.LobbyIsFull);
        }

        [Test]
        public void PlayerCantJoinLobbyWithWrongId()
        {
            var lobby = _lobbiesHost.CreateLobby(TestHelperMethods.CreatePlayer());

            var playerGuest = TestHelperMethods.CreatePlayer();
            var someId = "wrongIdFormat";
            var response = _lobbiesHost.JoinLobbyById(playerGuest, someId);

            Assert.IsTrue(response == JoinLobbyResponse.Error);
        }

        [Test]
        public void GameLobbyIsAddedToLobbiesWhenCreated()
        {
            LobbiesHost lobbiesHost = _lobbiesHost as LobbiesHost;
            var lobby = lobbiesHost.CreateLobby(TestHelperMethods.CreatePlayer(), TestHelperMethods.GetTimeSpan());
            var findLobbyFlag = lobbiesHost.Lobbies.TryGetValue(lobby.Id, out _);

            Assert.IsTrue(findLobbyFlag);
        }

        [Test]
        public void GameLobbyIsDestroyedAfterGameEnds()
        {
            LobbiesHost lobbiesHost = _lobbiesHost as LobbiesHost;
            var lobby = lobbiesHost.CreateLobby(TestHelperMethods.CreatePlayer(), TestHelperMethods.GetTimeSpan());

            ManualResetEvent eventRaised = new ManualResetEvent(false);

            var playerGuest = TestHelperMethods.CreatePlayer();
            var response = _lobbiesHost.JoinLobbyById(playerGuest, lobby.Id);

            lobby.GameOver += async lobby1 => 
            {
                await Task.Delay(TimeSpan.FromMilliseconds(300));
                eventRaised.Set();
            };

            var gameOverInTime = eventRaised.WaitOne(TimeSpan.FromSeconds(5));
            var findLobbyFlag = _lobbiesHost.Lobbies.TryGetValue(lobby.Id, out _);

            Assert.IsTrue(gameOverInTime && !findLobbyFlag);
        }

        [Test]
        public async Task GameResultIsWrittenToDbAfterGameOver()
        {
            var playerHost = TestHelperMethods.CreatePlayer();
            var playerGuest = TestHelperMethods.CreatePlayer();

            LobbiesHost lobbiesHost = _lobbiesHost as LobbiesHost;
            var lobby = lobbiesHost.CreateLobby(playerHost, TestHelperMethods.GetTimeSpan());

            var joinSecondResponse = lobby.TryJoinPlayer(playerGuest);

            ManualResetEvent eventRaised = new ManualResetEvent(false);
            GameResult? find;

            lobby.Game.GameStateChanged += async (sender, state) =>
            {
                if (state == GameState.GameOver)
                {
                    //delay to ensure data is written
                    await Task.Delay(TimeSpan.FromMilliseconds(100));
                    eventRaised.Set();
                }
            };

            var wait = eventRaised.WaitOne(TimeSpan.FromSeconds(50));

            //instantiate db connection to find result
            await using (var dbContext = new GameResultContext())
            {
                find = await dbContext.GameResults.FindAsync(lobby.Id);
            }

            Assert.IsTrue(
                wait &&
                find != null &&
                find.Id == lobby.Id &&
                find.PlayerHostName == playerHost.Name &&
                find.PlayerGuestName == playerGuest.Name);
        }
    }
}
