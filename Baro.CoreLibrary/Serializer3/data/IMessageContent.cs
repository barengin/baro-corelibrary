using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.Serializer3
{
    interface IMessageContent
    {
        int MessageSize { get; }
        RawData ToRawData();
    }

    interface IMessageContent<T>
    {
        T FromRawData(RawData data);
    }
}
