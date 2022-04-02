using DataLayer.GameContext;
using GameLogic.Base;

namespace ExApiGame.Controllers.Dtos
{
    public class LobbyDto
    {
        public LobbyDto(GameResult result)
        {
            Id = result.Id;
            State = GameState.GameOver.ToString();
            CurrentGame = new GameResultDto(result);
        }

        public LobbyDto(ILobby lobby)
        {
            Id = lobby.Id;
            State = (lobby.Game?.State ?? GameState.Lobby).ToString();
            CurrentGame = new GameResultDto(lobby);
        }

        public string Id { get; }

        //verbose GameState ToString()
        public string State { get; }

        public GameResultDto? CurrentGame { get; }

    }
}
