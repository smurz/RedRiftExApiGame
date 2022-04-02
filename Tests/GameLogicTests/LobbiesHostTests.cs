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
    public class LobbiesHostTests
    {
        [Test]
        public void LobbyIsCreated()
        {
            ILobbiesHost lobbiesHost = new LobbiesHost();
            var lobby = lobbiesHost.CreateLobby(TestHelperMethods.CreatePlayer());

            Assert.NotNull(lobby);
        }

        [Test]
        public void DefaultLobbyTimeIs1Second()
        {
            ILobbiesHost lobbiesHost = new LobbiesHost();
            var lobby = lobbiesHost.CreateLobby(TestHelperMethods.CreatePlayer()) as Lobby;

            Assert.IsTrue(lobby?.GameTick.TotalSeconds == 1d);
        }

        [Test]
        public void PlayerCanJoinLobby()
        {
            ILobbiesHost lobbiesHost = new LobbiesHost();
            var lobby = lobbiesHost.CreateLobby(TestHelperMethods.CreatePlayer());

            var playerGuest = TestHelperMethods.CreatePlayer();
            var response = lobby.TryJoinPlayer(playerGuest);

            Assert.IsTrue(response == JoinLobbyResponse.Success);
        }

        [Test]
        public void ThirdPlayerCantJoinLobby()
        {
            ILobbiesHost lobbiesHost = new LobbiesHost();
            var lobby = lobbiesHost.CreateLobby(TestHelperMethods.CreatePlayer());

            var playerGuest = TestHelperMethods.CreatePlayer();
            var playerWannaJoin = TestHelperMethods.CreatePlayer();

            lobby.TryJoinPlayer(playerGuest);
            var wannaJoinResponse = lobby.TryJoinPlayer(playerWannaJoin);

            Assert.IsTrue(wannaJoinResponse == JoinLobbyResponse.LobbyIsFull);
        }

        [Test]
        public void PlayerCanJoinLobbyById()
        {
            ILobbiesHost lobbiesHost = new LobbiesHost();
            var lobby = lobbiesHost.CreateLobby(TestHelperMethods.CreatePlayer());

            var playerGuest = TestHelperMethods.CreatePlayer();

            var response = lobbiesHost.JoinLobbyById(playerGuest, lobby.Id);

            Assert.IsTrue(response == JoinLobbyResponse.Success);
        }

        [Test]
        public void ThirdPlayerCantJoinLobbyById()
        {
            ILobbiesHost lobbiesHost = new LobbiesHost();
            var lobby = lobbiesHost.CreateLobby(TestHelperMethods.CreatePlayer());

            var playerGuest = TestHelperMethods.CreatePlayer();
            var playerWannaJoin = TestHelperMethods.CreatePlayer();

            var response = lobbiesHost.JoinLobbyById(playerGuest, lobby.Id);
            var wannaJoinResponse = lobbiesHost.JoinLobbyById(playerWannaJoin, lobby.Id);

            Assert.IsTrue(wannaJoinResponse == JoinLobbyResponse.LobbyIsFull);
        }

        [Test]
        public void PlayerCantJoinLobbyWithWrongId()
        {
            ILobbiesHost lobbiesHost = new LobbiesHost();
            var lobby = lobbiesHost.CreateLobby(TestHelperMethods.CreatePlayer());

            var playerGuest = TestHelperMethods.CreatePlayer();
            var someId = "wrongIdFormat";
            var response = lobbiesHost.JoinLobbyById(playerGuest, someId);

            Assert.IsTrue(response == JoinLobbyResponse.Error);
        }

        [Test]
        public void GameLobbyIsAddedToLobbiesWhenCreated()
        {
            LobbiesHost lobbiesHost = new LobbiesHost();
            var lobby = lobbiesHost.CreateLobby(TestHelperMethods.CreatePlayer(), TestHelperMethods.GetTimeSpan());
            var findLobbyFlag = lobbiesHost.Lobbies.TryGetValue(lobby.Id, out _);

            Assert.IsTrue(findLobbyFlag);
        }

        [Test]
        public void GameLobbyIsDestroyedAfterGameEnds()
        {
            LobbiesHost lobbiesHost = new LobbiesHost();
            var lobby = lobbiesHost.CreateLobby(TestHelperMethods.CreatePlayer(), TestHelperMethods.GetTimeSpan());

            ManualResetEvent eventRaised = new ManualResetEvent(false);

            var playerGuest = TestHelperMethods.CreatePlayer();
            var response = lobbiesHost.JoinLobbyById(playerGuest, lobby.Id);

            lobby.GameOver += async (sender, s) =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(300));
                eventRaised.Set();
            };

            var gameOverInTime = eventRaised.WaitOne(TimeSpan.FromSeconds(5));
            var findLobbyFlag = lobbiesHost.Lobbies.TryGetValue(lobby.Id, out _);

            Assert.IsTrue(gameOverInTime && !findLobbyFlag);
        }
    }
}
