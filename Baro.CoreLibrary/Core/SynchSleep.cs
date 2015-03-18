using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Baro.CoreLibrary.Core
{
    public static class SynchSleep
    {
        public static void Go(int milisecond)
        {
            StopWatch s = new StopWatch();
            s.Reset();

            while (s.Peek() < milisecond)
            {
                Thread.Sleep(0);
            }
        }
    }
}
