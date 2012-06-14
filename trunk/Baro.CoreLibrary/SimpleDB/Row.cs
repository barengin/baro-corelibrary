using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Baro.CoreLibrary.SimpleDB
{
    public enum RowState
    {
        /// <summary>
        /// Bir kayıt var. Bu kayıt bir DB'e bağlı. Yapılan değişiklikler DB'ye yansıyacak.
        /// </summary>
        Ok,

        /// <summary>
        /// Row nesnesi bir DB'ye bağlı değil. Yapılan değişiklikler bellekte saklanıyor.
        /// </summary>
        InMemory,

        /// <summary>
        /// Row nesnesi bir DB'ye bağlı ve o kayıt önceden silinmiş.
        /// </summary>
        Deleted
    } 

    public sealed class Row
    {
        private object[] m_values;

        public RowState State { get; private set; }
        public Header Header { get; private set; }

        public Row(Header header)
        {
            this.Header = header;
            this.State = RowState.InMemory;

            m_values = new object[header.Count];
        }

        public object this[int index]
        {
            get { return m_values[index]; }
            set { m_values[index] = value; }
        }

        internal void CreateRowRecord(byte[] data)
        {
        }
    }
}
