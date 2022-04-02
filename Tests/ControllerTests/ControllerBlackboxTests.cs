using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataLayer;
using ExApiGame.Controllers;
using ExApiGame.Controllers.Dtos;
using ExApiGame.Controllers.Validators;
using GameLogic;
using GameLogic.Base;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Tests.ControllerTests
{
    [TestFixture]
    public class ControllerBlackboxTests
    {
        private ILobbiesHost _lobbiesHost;
        private PlayerNameValidator _playerNameValidator;
        private GameIdValidator _idValidator;

        [SetUp]
        public void SetUp()
        {
            var contextFactoryMock = new Mock<IDbContextFactory<GameResultContext>>();
            contextFactoryMock.Setup(cf => cf.CreateDbContextAsync(CancellationToken.None))
                .ReturnsAsync(new GameResultContext());

            _lobbiesHost = new LobbiesHost(contextFactoryMock.Object);
            _playerNameValidator = new PlayerNameValidator();
            _idValidator = new GameIdValidator();
        }

        [TestCase("Noggibator")]
        [TestCase("_____ ddsds __D &%$4;;'\\@!$%|||_ _S")]
        [TestCase("John Constatine")]
        public void CreateLobbyByName(string name)
        {
            var createLobbyController = new CreateLobbyController(_lobbiesHost, _playerNameValidator);
            var result = createLobbyController.CreateLobbyByPlayerName(name);

            Assert.AreEqual(result.Value.CurrentGame.PlayerHost.Name, name);
        }

        [TestCase("Noggibator")]
        [TestCase("_____ ddsds __D &%$4;;'\\@!$%|||_ _S")]
        [TestCase("John Constatine")]
        public void CreateLobbyByNameGetsAddedToLobbies(string name)
        {
            var createLobbyController = new CreateLobbyController(_lobbiesHost, _playerNameValidator);
            var result = createLobbyController.CreateLobbyByPlayerName(name);
            var id = result.Value.Id;

            var findFlag = _lobbiesHost.Lobbies.TryGetValue(id, out _);
            Assert.IsTrue(findFlag);
        }

        [TestCase("Noggibator")]
        public async Task GameStateControllerReturnsCorrectLobbyStateWhileWaiting(string name)
        {
            var createLobbyController = new CreateLobbyController(_lobbiesHost, _playerNameValidator);
            var result = createLobbyController.CreateLobbyByPlayerName(name);
            var id = result.Value.Id;

            //db context is injected into the controller with Scoped stated and limited lifetime
            await using (var dbContext = new GameResultContext())
            {
                var gameStateController = new GameStateController(_lobbiesHost, _idValidator, dbContext);
                var stateResponse = await gameStateController.GetGameStateById(id);

                Assert.AreEqual(stateResponse.Value.State, GameState.Lobby.ToString());
            }
        }

        [TestCase("Noggibator")]
        public async Task GameStateControllerReturnsCorrectLobbyStateWhenSecondPlayerJoins(string name)
        {
            var createLobbyController = new CreateLobbyController(_lobbiesHost, _playerNameValidator);
            var result = createLobbyController.CreateLobbyByPlayerName(name);
            var id = result.Value.Id;

            var joinLobbyController = new JoinLobbyController(_lobbiesHost, _playerNameValidator, _idValidator);

            var joinParameters = new JoinLobbyParameters()
            {
                LobbyId = id,
                PlayerName = "some noob -=virgin=-"
            };

            joinLobbyController.JoinLobbyById(joinParameters);

            //db context is injected into the controller with Scoped stated and limited lifetime
            await using (var dbContext = new GameResultContext())
            {
                var gameStateController = new GameStateController(_lobbiesHost, _idValidator, dbContext);
                var stateResponse = await gameStateController.GetGameStateById(id);

                Assert.AreEqual(stateResponse.Value.State, GameState.Active.ToString());
            }
        }

        [TestCase("Noggibator")]
        public async Task ThirdPlayerCannotJoin(string name)
        {
            var createLobbyController = new CreateLobbyController(_lobbiesHost, _playerNameValidator);
            var result = createLobbyController.CreateLobbyByPlayerName(name);
            var id = result.Value.Id;

            var joinLobbyController = new JoinLobbyController(_lobbiesHost, _playerNameValidator, _idValidator);

            var joinParameters = new JoinLobbyParameters()
            {
                LobbyId = id,
                PlayerName = "some noob -=virgin=-"
            };

            var wannaJoinParameters = new JoinLobbyParameters()
            {
                LobbyId = id,
                PlayerName = "big bad wolf"
            };

            joinLobbyController.JoinLobbyById(joinParameters);
            var wannaJoinResult = joinLobbyController.JoinLobbyById(wannaJoinParameters);

            Assert.AreEqual(wannaJoinResult.Value.Status, JoinLobbyResponse.LobbyIsFull.ToString());
        }

        [TestCase("Noggibator")]
        public async Task GameRunsAtLeast3Seconds(string name)
        {
            var createLobbyController = new CreateLobbyController(_lobbiesHost, _playerNameValidator);
            var result = createLobbyController.CreateLobbyByPlayerName(name);
            var id = result.Value.Id;

            var joinLobbyController = new JoinLobbyController(_lobbiesHost, _playerNameValidator, _idValidator);

            var joinParameters = new JoinLobbyParameters()
            {
                LobbyId = id,
                PlayerName = "some noob -=virgin=-"
            };

            joinLobbyController.JoinLobbyById(joinParameters);
            await Task.Delay(TimeSpan.FromSeconds(3));
            //db context is injected into the controller with Scoped stated and limited lifetime
            await using (var dbContext = new GameResultContext())
            {
                var gameStateController = new GameStateController(_lobbiesHost, _idValidator, dbContext);
                var stateResponse = await gameStateController.GetGameStateById(id);

                Assert.AreEqual(stateResponse.Value.State, GameState.Active.ToString());
            }
        }
    }
}
