using DataLayer;
using DataLayer.GameContext;
using GameLogic.Base;
using Microsoft.EntityFrameworkCore;

namespace GameLogic
{
    public class Lobby : ILobby
    {
        //game tick time is set explicitly for testing purposes

        public Lobby(
            IPlayer playerHost, 
            TimeSpan tickTime)
        {
            PlayerHost = playerHost ?? throw new ArgumentNullException(nameof(playerHost), "Host player can not be null");
            Id = Guid.NewGuid().ToString();
            GameTick = tickTime;
        }

        public event GameOverHandler? GameOver;
        public string Id { get; }
        public IGame? Game { get; private set; }
        public IPlayer? PlayerHost { get; }
        public IPlayer? PlayerGuest { get; private set; }
        public TimeSpan GameTick { get; }

        public JoinLobbyResponse TryJoinPlayer(IPlayer player)
        {
            if (PlayerGuest != null) return JoinLobbyResponse.LobbyIsFull;

            PlayerGuest = player;

            Game = new TakeRandomHitGame(PlayerHost, PlayerGuest, GameTick);
            Game.GameStateChanged += HandleGameStateChangedAsync;
            Game.Start();

            return JoinLobbyResponse.Success;

        }

        private void HandleGameStateChangedAsync(object? sender, GameState gameState)
        {
            if (gameState == GameState.GameOver)
            {
                OnGameOver();
            }
        }

        protected virtual void OnGameOver()
        {
            GameOver?.Invoke(this);
        }
    }
}
