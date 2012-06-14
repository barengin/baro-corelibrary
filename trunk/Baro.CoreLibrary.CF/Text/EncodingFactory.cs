using System;
using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.Text
{
    public class EncodingException : Exception
    {
        public EncodingException()
        {
        }
        public EncodingException(string message)
            : base(message)
        {
        }
        public EncodingException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public static class EncodingFactory
    {
        public static Encoding GetEncoding(int codePage)
        {
            switch (codePage)
            {
                case 1254:
                    return new Encoding1254();

                case 12542:
                    return new Encoding12542();

                default:
                    throw new EncodingException("invalid encoding code page.");
            }
        }
    }
}
