using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.Serializer3
{
    public interface IMessageSerializer
    {
        int MessageSize { get; }

        ArraySegment<byte> ToRawData();
        object FromRawData(ArraySegment<byte> data);
    }
}
