using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.Web
{
    public class HttpDownloaderEventArgs : EventArgs
    {
        public object State { get; set; }
        public string File { get; set; }
    }
}
