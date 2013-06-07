using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.Serializer3
{
    public interface IMessageContent
    {
        int MessageSize { get; }
        ArraySegment<byte> ToRawData();
    }

    public interface IMessageContent<T>
    {
        T FromRawData(ArraySegment<byte> data);
    }
}
