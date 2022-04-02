using DataLayer.GameContext;
using GameLogic.Base;

namespace ExApiGame.Controllers.Dtos
{
    public class GameResultDto
    {
        public GameResultDto(GameResult gameResult)
        {
            if (gameResult == null) throw new ArgumentNullException(nameof(gameResult));

            GameId = gameResult.Id;

            PlayerHost = new PlayerDto(gameResult.PlayerHostName, gameResult.PlayerHostHealth);
            PlayerGuest = new PlayerDto(gameResult.PlayerGuestName, gameResult.PlayerGuestHealth);
        }

        public GameResultDto(ILobby gameLobby)
        {
            if (gameLobby == null) throw new ArgumentNullException(nameof(gameLobby));

            GameId = gameLobby.Id;

            PlayerHost = ToPlayerDto(gameLobby.PlayerHost);

            if (gameLobby.PlayerGuest != null)
                PlayerGuest = ToPlayerDto(gameLobby.PlayerGuest);
        }

        public string GameId { get; }

        public PlayerDto? PlayerHost { get; }

        public PlayerDto? PlayerGuest { get; }

        private PlayerDto ToPlayerDto(IPlayer player)
        {
            return new PlayerDto(player);
        }
    }
}
