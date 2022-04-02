using System.Collections.Concurrent;
using DataLayer;
using DataLayer.GameContext;
using GameLogic.Base;
using Microsoft.EntityFrameworkCore;

namespace GameLogic
{
    public class LobbiesHost : ILobbiesHost
    {
        private readonly IDbContextFactory<GameResultContext> _contextFactory;
        private readonly TimeSpan _defaultGameTick = TimeSpan.FromSeconds(1);
        private readonly ConcurrentDictionary<string, ILobby> _lobbies = new();

        public IDictionary<string, ILobby> Lobbies => _lobbies;

        //inject db context
        public LobbiesHost(IDbContextFactory<GameResultContext> contextFactory)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        /// <summary>
        /// Create lobby with single host player.
        /// </summary>
        /// <param name="playerHost"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public ILobby CreateLobby(IPlayer playerHost)
        {
            return CreateLobby(playerHost, _defaultGameTick);
        }

        public ILobby CreateLobby(IPlayer playerHost, TimeSpan gameTick)
        {
            var lobby = new Lobby(playerHost, gameTick);

            lobby.GameOver += Lobby_GameOver;

            _lobbies.TryAdd(lobby.Id, lobby);
            return lobby;
        }

        /// <summary>
        /// Try join active lobby by id. If no lobby with such id this method returns Error status.
        /// If lobby already contains 2 players it returns LobbyIsFull status and does not join new player.
        /// </summary>
        /// <param name="playerGuest"></param>
        /// <param name="lobbyId"></param>
        /// <returns></returns>
        public JoinLobbyResponse JoinLobbyById(IPlayer playerGuest, string lobbyId)
        {
            if (!_lobbies.TryGetValue(lobbyId, out var lobby)) return JoinLobbyResponse.Error;
            return lobby.TryJoinPlayer(playerGuest);

        }
        
        private async Task Lobby_GameOver(ILobby lobby)
        {
            _lobbies.TryRemove(lobby.Id, out _);
            // instantiate short life db connection to write game result
             await using (var dbContext = await _contextFactory.CreateDbContextAsync())
             {
                    var gameResult = new GameResult()
                    {
                        Id = lobby.Id,
                        PlayerHostName = lobby.PlayerHost.Name,
                        PlayerHostHealth = lobby.PlayerHost.PlayerHealth,
                        PlayerGuestName = lobby.PlayerGuest.Name,
                        PlayerGuestHealth = lobby.PlayerGuest.PlayerHealth
                    };

                    dbContext.GameResults.Add(gameResult);
                    await dbContext.SaveChangesAsync().ConfigureAwait(false);
             }
        }
    }
}
