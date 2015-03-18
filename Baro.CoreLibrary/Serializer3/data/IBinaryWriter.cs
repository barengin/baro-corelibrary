using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.Serializer3
{
    interface IBinaryWriter
    {
        void WriteByte(Byte b);
        void WriteInt32(Int32 i);
        void WriteInt64(Int64 i);

        void WriteSingle(Single s);
        void WriteDouble(Double d);

        void WriteString(String s);
        void WriteByteArray(Byte[] b);
    }
}
