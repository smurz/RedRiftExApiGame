using GameLogic.Base;

namespace GameLogic
{
    public class SimplePlayer : IPlayer
    {
        private const int DefaultHealth = 10;

        public SimplePlayer(string name)
        {
            Name = name;
            PlayerHealth = DefaultHealth;
        }

        public string Name { get; }
        public int PlayerHealth { get; set; }
    }
}
