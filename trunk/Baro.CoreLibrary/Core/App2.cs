using System;
using System.Collections.Generic;
using System.IO;

namespace Baro.CoreLibrary
{
    public static class App2
    {

#if PocketPC || WindowsCE
        private static readonly string appPath = string.Concat(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase), "\\");
#else
        private static readonly string appPath = string.Concat(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "\\");
#endif

        public static string AppPath
        {
            get
            {
                return appPath;
            }
        }
    }
}
