using DataLayer;
using DataLayer.GameContext;
using ExApiGame.Controllers.Dtos;
using ExApiGame.Controllers.Validators;
using GameLogic.Base;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ExApiGame.Controllers
{
    [Route("api/game-state")]
    [ApiController]
    public class GameStateController : ControllerBase
    {
        private readonly ILobbiesHost _lobbiesHost;
        private readonly GameIdValidator _idValidator;
        private readonly GameResultContext _dbContext;

        public GameStateController(
            ILobbiesHost lobbiesHost,
            GameIdValidator idValidator,
            GameResultContext dbContext)
        {
            _lobbiesHost = lobbiesHost ?? throw new ArgumentNullException(nameof(lobbiesHost));
            _idValidator = idValidator ?? throw new ArgumentNullException(nameof(idValidator));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        // GET api/game-state/4796bfbb-caf8-431f-a194-17df54d50e68
        [HttpGet]
        public async Task<ActionResult<LobbyDto>> GetGameStateById(string gameId)
        {
            //invalid game id format
            if (!_idValidator.ValidateGameId(gameId)) return NotFound(gameId);

            //game is active. report current game status
            var result = _lobbiesHost.Lobbies.TryGetValue(gameId, out var lobby);
            if (result && lobby != null)
            {
                return ToDto(lobby);
            }

            //game is over. result is stored to database. report game result.
            var gameResult = await _dbContext.GameResults.FindAsync(gameId);

            //no game found by this id
            if (gameResult == null) return NotFound(gameId);

            return ToDto(gameResult);
        }

        private LobbyDto ToDto(ILobby lobby)
        {
            return new LobbyDto(lobby);
        }

        private LobbyDto ToDto(GameResult result)
        {
            return new LobbyDto(result);
        }
    }
}
