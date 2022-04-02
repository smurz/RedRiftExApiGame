using DataLayer.GameContext;

namespace GameLogic.Base
{
    public interface ILobbiesHost
    {
        IDictionary<string, ILobby> Lobbies { get; }

        ILobby CreateLobby(IPlayer playerHost);

        JoinLobbyResponse JoinLobbyById(IPlayer playerGuest, string lobbyId);
    }
}
