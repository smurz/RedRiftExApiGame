using ExApiGame.Controllers.Dtos;
using ExApiGame.Controllers.Validators;
using GameLogic;
using GameLogic.Base;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ExApiGame.Controllers
{
    [Route("api/join-lobby")]
    [ApiController]
    public class JoinLobbyController : ControllerBase
    {
        private const string ErrorMessage = "Provided player name is not valid";

        private readonly ILobbiesHost _lobbiesHost;
        private readonly PlayerNameValidator _playerNameValidator;
        private readonly GameIdValidator _idValidator;

        public JoinLobbyController(
            ILobbiesHost lobbiesHost,
            PlayerNameValidator playerNameValidator,
            GameIdValidator idValidator)
        {
            _lobbiesHost = lobbiesHost ?? throw new ArgumentNullException(nameof(lobbiesHost));
            _playerNameValidator = playerNameValidator ?? throw new ArgumentNullException(nameof(playerNameValidator));
            _idValidator = idValidator ?? throw new ArgumentNullException(nameof(idValidator));
        }

        // POST api/join-lobby/
        [HttpPost]
        public ActionResult<JoinLobbyResponseDto> JoinLobbyById([FromBody] JoinLobbyParameters joinLobbyDto)
        {
            //invalid game id format
            if (!_playerNameValidator.ValidatePlayerName(joinLobbyDto.PlayerName)) return BadRequest(ErrorMessage);
            if (!_idValidator.ValidateGameId(joinLobbyDto.LobbyId)) NotFound(joinLobbyDto.LobbyId);

            var guest = new SimplePlayer(joinLobbyDto.PlayerName);
            var joinLobbyResponse = _lobbiesHost.JoinLobbyById(guest, joinLobbyDto.LobbyId);

            var dto = new JoinLobbyResponseDto(joinLobbyDto.LobbyId, joinLobbyResponse);

            return dto;
        }
    }
}
