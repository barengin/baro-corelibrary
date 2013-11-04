﻿using System;
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

        [DllImport("PndSystemEx.dll")]
        public static extern int EceGetBKWStatus(ref int status);

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

        public static bool SetBacklightValue(int level)
        {
            return EceSetBacklightValue(level) != 0;
        }
    }
}
