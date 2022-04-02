namespace ExApiGame.Controllers.Validators
{
    public class PlayerNameValidator
    {
        public bool ValidatePlayerName(string playerName)
        {
            return !string.IsNullOrEmpty(playerName);
        }
    }
}
