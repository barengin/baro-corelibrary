using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.Web
{
    public class FileErrorEventArg : HttpDownloaderEventArgs
    {
        public Exception Error { get; set; }
    }
}
