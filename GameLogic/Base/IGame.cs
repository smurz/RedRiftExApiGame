using System.Windows.Input;
using DataLayer.GameContext;

namespace GameLogic.Base
{
    public interface IGame
    {
        GameState State { get; }

        void Start();

        event EventHandler<GameState>? GameStateChanged;
    }
}
