using DataLayer;
using ExApiGame.Controllers.Dtos;
using ExApiGame.Controllers.Validators;
using GameLogic;
using GameLogic.Base;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ExApiGame.Controllers
{
    [Route("api/create_lobby")]
    [ApiController]
    public class CreateLobbyController : ControllerBase
    {
        private const string ErrorMessage = "Provided player name is not valid";

        private readonly ILobbiesHost _lobbiesHost;
        private readonly PlayerNameValidator _playerNameValidator;

        public CreateLobbyController(
            ILobbiesHost lobbiesHost,
            PlayerNameValidator playerNameValidator)
        {
            _lobbiesHost = lobbiesHost ?? throw new ArgumentNullException(nameof(lobbiesHost));
            _playerNameValidator = playerNameValidator ?? throw new ArgumentNullException(nameof(playerNameValidator));
        }

        // POST api/create_lobby/
        [HttpPost]
        public ActionResult<LobbyDto> CreateLobbyByPlayerName([FromBody] CreateLobbyParameters createLobbyParameters)
        {
            if (!_playerNameValidator.ValidatePlayerName(createLobbyParameters.PlayerName)) return BadRequest(ErrorMessage);

            var playerHost = new SimplePlayer(createLobbyParameters.PlayerName);
            var lobby = _lobbiesHost.CreateLobby(playerHost);

            var dto = new LobbyDto(lobby);
            return dto;
        }

    }
}
