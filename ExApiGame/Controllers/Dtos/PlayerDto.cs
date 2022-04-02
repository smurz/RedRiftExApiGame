using DataLayer.GameContext;
using GameLogic.Base;

namespace ExApiGame.Controllers.Dtos
{
    public class PlayerDto
    {
        public PlayerDto(IPlayer player)
        {
            Name = player?.Name;
            PlayerHealth = player?.PlayerHealth;
        }

        public PlayerDto(string name, int health)
        {
            Name = name;
            PlayerHealth = health;
        }

        public string? Name { get; }

        public int? PlayerHealth { get; }
    }
}
