using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.GameContext;

namespace GameLogic.Base
{
    public delegate Task GameOverHandler(ILobby lobby);

    public interface ILobby
    {
        event GameOverHandler? GameOver;

        string Id { get; }

        IGame? Game { get; }

        IPlayer PlayerHost { get; }

        IPlayer? PlayerGuest { get; }

        JoinLobbyResponse TryJoinPlayer(IPlayer player);
    }
}
