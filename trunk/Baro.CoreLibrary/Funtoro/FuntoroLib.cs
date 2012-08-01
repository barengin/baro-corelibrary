using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Baro.CoreLibrary.Funtoro
{
    public enum Ece3GCard : int
    {
        NotExists = 0,
        Exists
    }

    public static class FuntoroLib
    {
        [DllImport("PndSystemEx.dll")]
        public static extern int EceChk3GComCardExistence(ref Ece3GCard exists);

        [DllImport("PndSystemEx.dll")]
        public static extern int Ece3GComCardOff();

        [DllImport("PndSystemEx.dll")]
        public static extern int Ece3GComCardOn();

        public static bool is3GCardExists()
        {
            Ece3GCard e = Ece3GCard.NotExists;
            EceChk3GComCardExistence(ref e);
            return e == Ece3GCard.Exists;
        }
    }
}
