using System;
using System.Collections.Generic;
using System.IO;

namespace Baro.CoreLibrary
{
    public static class App2
    {
        private static readonly string appPath = string.Concat(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase), "\\");

        public static string AppPath
        {
            get
            {
                return appPath;
            }
        }
    }
}
