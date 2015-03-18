using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Baro.CoreLibrary.AAA
{
    public sealed class Token: IEquatable<Token>, ICloneable
    {
        public DateTime ExpireDate { get; private set; }
        public Int64 TokenData { get; private set; }

        internal Token(DateTime expireDate, Int64 tokenData)
        {
            ExpireDate = expireDate;
            TokenData = tokenData;
        }

        public bool isExpired()
        {
            return (DateTime.Now > this.ExpireDate);
        }

        public bool Equals(Token other)
        {
            return this.TokenData == other.TokenData;
        }

        public object Clone()
        {
            return new Token(this.ExpireDate, this.TokenData);
        }
    }
}
