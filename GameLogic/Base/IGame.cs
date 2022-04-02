using System.Windows.Input;
using DataLayer.GameContext;

namespace GameLogic.Base
{
    public delegate Task GameStateChangedHandler(GameState gameState);

    public interface IGame
    {
        GameState State { get; }

        void Start();

        event GameStateChangedHandler GameStateChanged;
    }
}
