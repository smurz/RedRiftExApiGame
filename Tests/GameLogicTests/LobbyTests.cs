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
using NUnit.Framework;

namespace Tests.GameLogicTests
{
    [TestFixture]
    public class LobbyTests
    {
        [Test]
        public void LobbyCanNotBeCreatedWithEmptyPlayer()
        {
            Assert.Throws(typeof(ArgumentNullException), () => new Lobby(null, TestHelperMethods.GetTimeSpan()));
        }

        [Test]
        public void PlayerJoinsCorrectly()
        {
            var playerHost = TestHelperMethods.CreatePlayer();
            var playerGuest = TestHelperMethods.CreatePlayer();

            var lobby = new Lobby(playerHost, TestHelperMethods.GetTimeSpan());
            var joinSecondResponse = lobby.TryJoinPlayer(playerGuest);

            Assert.AreEqual(joinSecondResponse, JoinLobbyResponse.Success);
        }

        [Test]
        public void ThirdPlayerCanNotJoinFullLobby()
        {
            var playerHost = TestHelperMethods.CreatePlayer();
            var playerGuest = TestHelperMethods.CreatePlayer();
            var playerWannaJoin = TestHelperMethods.CreatePlayer();

            var lobby = new Lobby(playerHost, TestHelperMethods.GetTimeSpan());
            var joinSecondResponse = lobby.TryJoinPlayer(playerGuest);
            var wannaJoinResponse = lobby.TryJoinPlayer(playerWannaJoin);

            Assert.AreEqual(wannaJoinResponse, JoinLobbyResponse.LobbyIsFull);
        }

        [Test]
        public void GameStartsAfterSecondPlayerJoinsAutomatically()
        {
            var playerHost = TestHelperMethods.CreatePlayer();
            var playerGuest = TestHelperMethods.CreatePlayer();

            var lobby = new Lobby(playerHost, TestHelperMethods.GetTimeSpan());
            var joinSecondResponse = lobby.TryJoinPlayer(playerGuest);


            Assert.AreEqual(lobby.Game.State, GameState.Active);
        }
    }
}
