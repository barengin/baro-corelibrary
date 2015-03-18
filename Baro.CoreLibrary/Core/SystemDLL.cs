using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Baro.CoreLibrary
{
    internal static class SystemDLL
    {
#if PocketPC || WindowsCE
        public const string NAME = "coredll";
#else
        public const string NAME = "kernel32";
#endif
    }
}
