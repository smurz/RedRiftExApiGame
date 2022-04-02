using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameLogic;
using GameLogic.Base;

namespace Tests
{
    public static class TestHelperMethods
    {
        public static TimeSpan GetTimeSpan()
        {
            return TimeSpan.FromMilliseconds(1);
        }

        public static IPlayer CreatePlayer()
        {
            var name = Guid.NewGuid().ToString();
            return new SimplePlayer(name);
        }
    }
}
