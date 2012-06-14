using Baro.CoreLibrary.Power;

namespace Baro.CoreLibrary.Device
{
#if PocketPC || WindowsCE
    public static class UnattendedMode
    {
        public static int On()
        {
            return CoreDLL.PowerPolicyNotify(PPNMessage.PPN_UNATTENDEDMODE, 1);
        }

        public static int Off()
        {
            return CoreDLL.PowerPolicyNotify(PPNMessage.PPN_UNATTENDEDMODE, 0);
        }
    }
#endif
}
