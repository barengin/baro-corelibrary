using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Baro.CoreLibrary.Media
{
    public class SoundPlayer
    {
        // Fields
        private byte[] mBuffer;
        private string mSoundLocation = string.Empty;
        private Stream mStream;

        public SoundPlayer(Stream stream)
        {
            this.Stream = stream;
        }

        public SoundPlayer(string soundLocation)
        {
            this.SoundLocation = soundLocation;
        }

        public void Play()
        {
            this.PlaySound(NativeMethods.SoundFlags.Async);
        }

        public void PlayLooping()
        {
            this.PlaySound(NativeMethods.SoundFlags.Loop | NativeMethods.SoundFlags.Async);
        }

        private void PlaySound(NativeMethods.SoundFlags flags)
        {
            if (this.mBuffer != null)
            {
                GCHandle handle = GCHandle.Alloc(this.mBuffer, GCHandleType.Pinned);
                NativeMethods.PlaySound((IntPtr)handle.AddrOfPinnedObject().ToInt32(), IntPtr.Zero, NativeMethods.SoundFlags.Memory | flags);
                handle.Free();
            }
            else
            {
                NativeMethods.PlaySound(this.mSoundLocation, IntPtr.Zero, NativeMethods.SoundFlags.FileName | flags);
            }
        }

        public void PlaySync()
        {
            this.PlaySound(NativeMethods.SoundFlags.Sync);
        }

        public void Stop()
        {
            NativeMethods.PlaySound((string)null, IntPtr.Zero, NativeMethods.SoundFlags.NoDefault);
        }

        // Properties
        public string SoundLocation
        {
            get
            {
                return this.mSoundLocation;
            }
            set
            {
                if (File.Exists(value))
                {
                    this.mSoundLocation = value;
                    this.Stream = null;
                }
            }
        }

        public Stream Stream
        {
            get
            {
                return this.mStream;
            }
            set
            {
                this.mStream = value;

                if (value == null)
                {
                    this.mBuffer = null;
                }
                else
                {
                    this.mBuffer = new byte[value.Length];
                    value.Read(this.mBuffer, 0, this.mBuffer.Length);
                    value.Close();
                    this.mSoundLocation = string.Empty;
                }
            }
        }

        public object Tag { get; set; }
    }
}
