using GameLogic.Base;

namespace ExApiGame.Controllers.Dtos
{
    public class JoinLobbyResponseDto
    {
        public JoinLobbyResponseDto(string gameId, JoinLobbyResponse response)
        {
            GameId = gameId;
            Status = response.ToString();
        }

        //verbose JoinLobbyResponse ToString()
        public string Status { get; set; }

        public string GameId { get; set; }
    }
}
