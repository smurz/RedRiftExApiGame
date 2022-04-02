namespace GameLogic.Base
{
    public interface IPlayer
    {
        string Name { get; }

        int PlayerHealth { get; set; }
    }
}
