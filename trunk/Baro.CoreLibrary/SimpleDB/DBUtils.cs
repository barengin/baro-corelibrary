using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Baro.CoreLibrary.SimpleDB
{
    public static class DBUtils
    {
        public static void CreateEmptyDB(string filename, Header header)
        {
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentException("Dosya adı hatalı");

            if (header == null)
                throw new ArgumentNullException("header");

            if (header.Count == 0)
                throw new InvalidOperationException("Header içinde mutlaka en az bir kolon olmalı");

            using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                BinaryWriter bw = new BinaryWriter(fs);

                byte[] hr = header.CreateHeaderRecord();

                bw.Write((int)hr.Length);

                fs.Write(hr, 0, hr.Length);
            }
        }
    }
}
