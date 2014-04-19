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

        [DllImport("PndSystemEx.dll")]
        private static extern int EceSetBacklightValue(int level);

        [DllImport("PndSystemEx.dll")]
        private static extern int EceGetBacklightValue(ref int level);

        [DllImport("PndSystemEx.dll")]
        private static extern int EceSetVolumeLevel(int level);

        /// <summary>
        /// Geri kamera bilgisi
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [DllImport("PndSystemEx.dll")]
        public static extern int EceGetBKWStatus(ref int status);

        [DllImport("PndSystemEx.dll")]
        public static extern int EceHardReset();

        [DllImport("PndSystemEx.dll")]
        public static extern int EceSetBacklightTimeout(int seconds);

        [DllImport("PndSystemEx.dll")]
        public static extern int EceBacklightOn();

        [DllImport("PndSystemEx.dll")]
        public static extern int EceBacklightOff();

        public static bool is3GCardExists()
        {
            Ece3GCard e = Ece3GCard.NotExists;
            EceChk3GComCardExistence(ref e);
            return e == Ece3GCard.Exists;
        }

        /// <summary>
        /// min:0 max: 20
        /// </summary>
        public static bool SetVolumeLevel(int level)
        {
            return EceSetVolumeLevel(level) != 0;
        }

        public static bool GetBacklightValue(ref int level)
        {
            return EceGetBacklightValue(ref level) != 0;
        }

        /// <summary>
        /// Arka plan ışığı. 0-6 arasında değer alır.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static bool SetBacklightValue(int level)
        {
            return EceSetBacklightValue(level) != 0;
        }
    }
}
