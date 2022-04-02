using DataLayer;
using DataLayer.GameContext;
using GameLogic.Base;
using Microsoft.EntityFrameworkCore;

namespace GameLogic
{
    public class Lobby : ILobby
    {
        //game tick time is set explicitly for testing purposes

        public Lobby(IPlayer playerHost, TimeSpan tickTime)
        {
            PlayerHost = playerHost ?? throw new ArgumentNullException(nameof(playerHost), "Host player can not be null");
            Id = Guid.NewGuid().ToString();
            GameTick = tickTime;
        }

        public event EventHandler<string>? GameOver;
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

        //should use reactive pipeline, commands and observable error handling instead of events.
        private async Task HandleGameStateChangedAsync(GameState e)
        {
            if (e == GameState.GameOver)
            {
                try
                {
                    // instantiate short life db connection to write game result
                    await using (var dbContext = new GameResultContext())
                    {
                        var gameResult = new GameResult()
                        {
                            Id = Id,
                            PlayerHostName = PlayerHost.Name,
                            PlayerHostHealth = PlayerHost.PlayerHealth,
                            PlayerGuestName = PlayerGuest.Name,
                            PlayerGuestHealth = PlayerGuest.PlayerHealth
                        };

                        dbContext.GameResults.Add(gameResult);
                        await dbContext.SaveChangesAsync().ConfigureAwait(false);
                    }
                }
                finally
                {
                    OnGameOver(Id);
                }
            }
        }

        protected virtual void OnGameOver(string e)
        {
            GameOver?.Invoke(this, e);
        }
    }
}
