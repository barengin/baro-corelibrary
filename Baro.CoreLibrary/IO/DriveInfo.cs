using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;

namespace Baro.CoreLibrary.IO
{
    public class DriveInfo
    {
        // Fields
        private long available;
        private string root;
        private long size;
        private long total;

        // Methods
        public DriveInfo(string driveName)
        {
            if (string.IsNullOrEmpty(driveName))
            {
                throw new ArgumentNullException("driveName");
            }
            this.root = driveName;
            NativeMethods.GetDiskFreeSpaceEx(driveName, ref this.available, ref this.size, ref this.total);
        }

        public static DriveInfo[] GetDrives()
        {
            FileAttributes attributes = FileAttributes.Temporary | FileAttributes.Directory;
            List<DriveInfo> list = new List<DriveInfo>();

            list.Add(new DriveInfo(@"\"));
            DirectoryInfo info = new DirectoryInfo(@"\");

            foreach (DirectoryInfo info2 in info.GetDirectories())
            {
                if ((info2.Attributes & attributes) == attributes)
                {
                    list.Add(new DriveInfo(@"\" + info2.Name));
                }
            }

            return list.ToArray();
        }

        public override string ToString()
        {
            return this.root;
        }

        // Properties
        public long AvailableFreeSpace
        {
            get
            {
                return this.available;
            }
        }

        public DirectoryInfo RootDirectory
        {
            get
            {
                return new DirectoryInfo(this.root);
            }
        }

        public long TotalFreeSpace
        {
            get
            {
                return this.total;
            }
        }

        public long TotalSize
        {
            get
            {
                return this.size;
            }
        }
    }
}
