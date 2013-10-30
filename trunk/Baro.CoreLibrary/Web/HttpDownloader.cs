using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.IO;

namespace Baro.CoreLibrary.Web
{
    public partial class HttpDownloader
    {
        public event EventHandler<FileProgressEventArg> OnProgress;
        public event EventHandler<FileCompletedEventArg> OnCompleted;
        public event EventHandler<FileErrorEventArg> OnError;

        protected void FireOnError(FileErrorEventArg e)
        {
            if (OnError != null)
            {
                OnError(this, e);
            }
        }

        protected void FireOnProgress(FileProgressEventArg e)
        {
            if (OnProgress != null)
            {
                OnProgress(this, e);
            }
        }

        protected void FireOnCompleted(FileCompletedEventArg e)
        {
            if (OnCompleted != null)
            {
                OnCompleted(this, e);
            }
        }

        public HttpDownloader()
        {
        }

        private volatile bool cancel = false;
        private volatile bool isDownloading = false;
        private Uri _downloadUri;
        private string _path;
        private object _state;

        public void Cancel()
        {
            cancel = true;
        }

        public void DownloadFile(Uri uri, string path, object state)
        {
            if (isDownloading)
                return;

            _downloadUri = uri;
            _path = path;
            _state = state;

            Thread t = new Thread(new ThreadStart(download));
            t.IsBackground = true;
            t.Start();
        }

        private void download()
        {
            isDownloading = true;
            FileStream fs = null;
            HttpWebRequest r = null;
            HttpWebResponse res = null;

            try
            {
                r = (HttpWebRequest)HttpWebRequest.Create(_downloadUri);
                res = (HttpWebResponse)r.GetResponse();

                int sizeOfFile = (int)res.ContentLength;
                Stream s = res.GetResponseStream();

                int bufsize = 65536;
                byte[] buf = new byte[bufsize];

                fs = new FileStream(_path, FileMode.Create);

                int total = 0;
                int k;
                while ((k = s.Read(buf, 0, bufsize)) > 0)
                {
                    fs.Write(buf, 0, k);
                    total += k;

                    if (cancel)
                    {
                        fs.Close();
                        s.Close();
                        res.Close();
                        r = null;

                        if (File.Exists(_path))
                            File.Delete(_path);

                        return;
                    }

                    FireOnProgress(new FileProgressEventArg() { Value = total, File = _path, Total = sizeOfFile, State = _state });
                }

                fs.Close();
                s.Close();
                res.Close();
                r = null;
            }
            catch (Exception ex)
            {
                if (fs != null) fs.Close();
                if (res != null) res.Close();
                r = null;

                FireOnError(new FileErrorEventArg() { Error = ex, File = _path, State = _state });
            }
            finally
            {
                cancel = false;
                isDownloading = false;
                FireOnCompleted(new FileCompletedEventArg() { File = _path, State = _state });
            }
        }
    }
}
