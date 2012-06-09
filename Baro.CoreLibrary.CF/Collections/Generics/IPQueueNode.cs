using System;
using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.Collections.Generics
{
    public interface IPQueueNode
    {
        int PQPriority { get; set; }
        int PQPosition { get; set; }
    }
}
