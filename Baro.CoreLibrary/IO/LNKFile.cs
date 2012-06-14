using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Baro.CoreLibrary.IO
{
    public static class LNKFile
    {
        public static void Create(string fromExecutable, string lnkFile)
        {
            string f = string.Concat("\"", fromExecutable, "\"");

            StreamWriter sw = new StreamWriter(lnkFile);

            sw.Write(f.Length.ToString());
            sw.Write("#");
            sw.Write(f);

            sw.Close();
        }

        public static void CreateFromThisAssembly(string lnkFile)
        {
            Create(System.Reflection.Assembly.GetCallingAssembly().GetName().CodeBase, lnkFile);
        }
    }
}
