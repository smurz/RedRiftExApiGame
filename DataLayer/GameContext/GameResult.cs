using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.GameContext
{
    public class GameResult
    {
        public string Id { get; set; }

        public string PlayerHostName { get; set; }

        public int PlayerHostHealth { get; set; }

        public string PlayerGuestName { get; set; }

        public int PlayerGuestHealth { get; set; }
    }
}
