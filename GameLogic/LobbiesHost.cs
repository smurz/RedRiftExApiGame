using System.Collections.Concurrent;
using DataLayer;
using DataLayer.GameContext;
using GameLogic.Base;

namespace GameLogic
{
    public class LobbiesHost : ILobbiesHost
    {
        private readonly TimeSpan _defaultGameTick = TimeSpan.FromSeconds(1);
        private readonly ConcurrentDictionary<string, ILobby> _lobbies = new();

        public IDictionary<string, ILobby> Lobbies => _lobbies;

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

        //remove active lobby from the list on game over
        private void Lobby_GameOver(object? sender, string e)
        {
            _lobbies.TryRemove(e, out _);
        }
    }
}
