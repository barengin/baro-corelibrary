using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.Web
{
    public class FileProgressEventArg : HttpDownloaderEventArgs
    {
        public int Total { get; set; }
        public int Value { get; set; }
    }
}
