using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Baro.CoreLibrary.Serializer2
{
    public sealed class MessageKey
    {
        private byte[] m_pwd;

        internal byte[] Pwd
        {
            get { return m_pwd; }
        }

        public MessageKey(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new InvalidOperationException("Şifre null,empty ve boşluk olamaz");

            m_pwd = System.Text.Encoding.UTF8.GetBytes(password);
        }
    }
}
