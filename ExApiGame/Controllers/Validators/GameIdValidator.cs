namespace ExApiGame.Controllers.Validators
{
    public class GameIdValidator
    {
        public bool ValidateGameId(string gameId)
        {
            return Guid.TryParse(gameId, out _);
        }
    }
}
