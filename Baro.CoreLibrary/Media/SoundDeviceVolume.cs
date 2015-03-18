using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Baro.CoreLibrary.Media
{
    public enum DeviceTypes
    {
        Wave,
        Aux,
        Midi
    }
    
    internal static class NativeSoundMethods
    {
        public const int MIN_VOLUME = 0x0000;
        public const int MAX_VOLUME = 0xFFFF;

        // Error constants
        public const int MMSYSERR_NOERROR = 0;
        public const int MMSYSERR_BADDEVICEID = 2;
        public const int MMSYSERR_INVALHANDLE = 5;
        public const int MMSYSERR_NODRIVER = 6;
        public const int MMSYSERR_NOMEM = 7;
        public const int MMSYSERR_NOTSUPPORTED = 8;
        public const int MMSYSERR_INVALPARAM = 11;

        // See the article Task 1 Declartion section for how to convert the unmanged types to the managed types for each function


        // Wave files

        // http://msdn.microsoft.com/library/default.asp?url=/library/en-us/multimed/htm/_win32_waveoutgetvolume.asp
        [DllImport(SystemDLL.NAME)]
        public static extern int waveOutGetVolume(IntPtr hwo, out uint dwVolume);

        // http://msdn.microsoft.com/library/default.asp?url=/library/en-us/multimed/htm/_win32_waveoutsetvolume.asp
        [DllImport(SystemDLL.NAME)]
        public static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);


        // aux devices

        // http://msdn.microsoft.com/library/default.asp?url=/library/en-us/multimed/htm/_win32_auxgetvolume.asp
        [DllImport(SystemDLL.NAME)]
        public static extern int auxGetVolume(int uDeviceID, out uint dwVolume);

        // http://msdn.microsoft.com/library/default.asp?url=/library/en-us/multimed/htm/_win32_auxsetvolume.asp
        [DllImport(SystemDLL.NAME)]
        public static extern int auxSetVolume(int uDeviceID, uint dwVolume);


        // midi devices

        // http://msdn.microsoft.com/library/default.asp?url=/library/en-us/multimed/htm/_win32_midioutgetvolume.asp
        [DllImport(SystemDLL.NAME)]
        public static extern int midiOutGetVolume(IntPtr hmo, out uint dwVolume);

        // http://msdn.microsoft.com/library/default.asp?url=/library/en-us/multimed/htm/_win32_midioutsetvolume.asp
        [DllImport(SystemDLL.NAME)]
        public static extern int midiOutSetVolume(IntPtr hmo, uint dwVolume);
    }

    public static class SoundDeviceVolume
    {
        // Create a SoundDevice class for a specific device type
        static SoundDeviceVolume()
        {
            _type = DeviceTypes.Wave;
        }

        // set the volume when the slider changes or get the volume for the mute method
        public static ushort Volume
        {
            // Ignore that some devices can set the
            // left and right volumes seperately
            get
            {
                ushort leftVol = 0;
                ushort rightVol = 0;

                GetVolume(ref leftVol, ref rightVol);

                return leftVol;
            }
            set
            {
                SetVolume(value, value);
            }
        }

        public static void Mute()
        {
            _leftVol = 0;
            _rightVol = 0;

            // First store the current volume settings
            int returnValue = GetVolume(ref _leftVol, ref _rightVol);


            if (returnValue == NativeSoundMethods.MMSYSERR_NOERROR)
            {
                // If that was successful then set the volume to zero
                returnValue = SetVolume(0, 0);

                if (returnValue == NativeSoundMethods.MMSYSERR_NOERROR)
                {
                    // all is ok
                }
                else
                {
                    // MessageBox.Show("Could not set the volume to zero", "Sound Sample", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                    throw new Exception("Could not set the volume to zero");
                }
            }
            else
            {
                // MessageBox.Show("Could not get the current volume setting", "Sound Sample", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                throw new Exception("Could not get the current volume setting");
            }
        }
        public static void UnMute()
        {
            // if the stored volume is allready there is nothing to do
            if (_leftVol > 0 || _rightVol > 0)
            {
                // otherwise set the volume back to the values stored
                int returnValue = SetVolume(_leftVol, _rightVol);

                if (returnValue == NativeSoundMethods.MMSYSERR_NOERROR)
                {
                    // all is ok
                }
                else
                {
                    // MessageBox.Show("Could not unmute the sound", "Sound Sample", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                    throw new Exception("Could not unmute the sound");
                }
            }
        }

        private static int GetVolume(ref ushort volLeft, ref ushort volRight)
        {
            uint vol = 0;
            int result = 9999;

            // depending on the sound device type call one of the PInvoke functions
            switch (_type)
            {
                case DeviceTypes.Wave:
                    result = NativeSoundMethods.waveOutGetVolume(_hwo, out vol);
                    break;
                case DeviceTypes.Aux:
                    result = NativeSoundMethods.auxGetVolume(_deviceID, out vol);
                    break;
                case DeviceTypes.Midi:
                    result = NativeSoundMethods.midiOutGetVolume(_hwo, out vol);
                    break;
            }

            if (result != NativeSoundMethods.MMSYSERR_NOERROR)
                return result;

            // extract the two volume settings from the single vol value
            volLeft = (ushort)(vol & 0x0000ffff);
            volRight = (ushort)(vol >> 16);

            return NativeSoundMethods.MMSYSERR_NOERROR;
        }
        private static int SetVolume(ushort volLeft, ushort volRight)
        {
            // Combine the two volume settings into a single value
            uint vol = ((uint)volLeft & 0x0000ffff) | ((uint)volRight << 16);

            // and then call the PInvoke functions with that value
            switch (_type)
            {
                case DeviceTypes.Wave:
                    return NativeSoundMethods.waveOutSetVolume(_hwo, vol);
                case DeviceTypes.Aux:
                    return NativeSoundMethods.auxSetVolume(_deviceID, vol);
                case DeviceTypes.Midi:
                    return NativeSoundMethods.midiOutSetVolume(_hwo, vol);
            }
            return 0;
        }

        private static DeviceTypes _type;
        private static IntPtr _hwo = IntPtr.Zero;
        private static int _deviceID = 0;
        private static ushort _leftVol;
        private static ushort _rightVol;
    }
}
