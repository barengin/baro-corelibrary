using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Baro.CoreLibrary.SimpleDB
{
    public enum SimpleFieldType
    {
        Int32,
        UInt32,
        Int16,
        UInt16,
        Int8,
        UInt8,
        Int64,
        UInt64,
        DateTime,
        Float32,
        Float64,
        
        /// <summary>
        /// If you dont use Size parameter, it will be 16 chars
        /// </summary>
        String,
        
        /// <summary>
        /// If you dont use Size parameter, it will be 16 bytes
        /// </summary>
        ByteArray
    }

    public sealed class SimpleField
    {
        public SimpleFieldType Type { get; private set; }
        public int Size { get; private set; }

        public SimpleField(SimpleFieldType type)
        {
            this.Type = type;
            this.Size = 16;
        }

        public SimpleField(SimpleFieldType type, int size)
        {
            this.Type = type;
            this.Size = size;
        }
    }
}
