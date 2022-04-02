using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.GameContext;

namespace GameLogic.Base
{
    public interface ILobby
    {
        event EventHandler<string>? GameOver;

        string Id { get; }

        IGame? Game { get; }

        IPlayer PlayerHost { get; }

        IPlayer? PlayerGuest { get; }

        JoinLobbyResponse TryJoinPlayer(IPlayer player);
    }
}
