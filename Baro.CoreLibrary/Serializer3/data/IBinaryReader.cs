using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.Serializer3
{
    interface IBinaryReader
    {
        Byte ReadByte();
        Int32 ReadInt32();
        Int64 ReadInt64();

        Single ReadSingle();
        Double ReadDouble();

        String ReadString();
        Byte[] ReadByteArray();
    }
}
