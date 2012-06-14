using System;

using System.Collections.Generic;
using System.IO;

namespace Baro.CoreLibrary.IO
{
    public static class SDCards
    {
        private const System.IO.FileAttributes attrStorageCard = System.IO.FileAttributes.Directory | System.IO.FileAttributes.Temporary;

        public static List<string> GetStorageCardNames()
        {
            List<string> scards = new List<string>();

            DirectoryInfo rootDir = new DirectoryInfo(@"\");

            foreach (DirectoryInfo di in rootDir.GetDirectories())
            {
                //if directory and temporary
                if ((di.Attributes & attrStorageCard) == attrStorageCard)
                {
                    //add to collection of storage cards
                    scards.Add("\\" + di.Name);
                }
            }

            return scards;
        }
    }
}
