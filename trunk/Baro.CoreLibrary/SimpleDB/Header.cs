using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Baro.CoreLibrary.SimpleDB
{
    public sealed class Header: IList<SimpleField>
    {
        private bool m_sealed = false;
        private List<SimpleField> m_list;

        public Header()
        {
            m_list = new List<SimpleField>();
        }

        internal Header(byte[] headerRecord)
        {
            m_list = ParseHeader(headerRecord);
            SealTheHeader(true);
        }

        private List<SimpleField> ParseHeader(byte[] headerRecord)
        {
            return HeaderRecord.Parse(headerRecord);
        }

        internal byte[] CreateHeaderRecord()
        {
            SealTheHeader(true);
            return HeaderRecord.Create(this);
        }

        public void SealTheHeader(bool areYouSure)
        {
            if (!areYouSure) throw new SimpleDBException("Madem emin değilsin neden sınıfı kilitlemeye kalkıyorsun?");

            m_sealed = areYouSure;
        }

        private void CheckSealed()
        {
            if (m_sealed) throw new SimpleDBException("Header sınıfı kilitlenmiş durumda. Üzerinde değişiklik yapamazsınız.");
        }

        public int GetRawRecordSize()
        {
            return HeaderRecord.CalculateRawRecordSize(this);
        }

        public int IndexOf(SimpleField item)
        {
            return m_list.IndexOf(item);
        }

        public void Insert(int index, SimpleField item)
        {
            CheckSealed();

            m_list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            CheckSealed();

            m_list.RemoveAt(index);
        }

        public SimpleField this[int index]
        {
            get
            {
                return m_list[index];
            }
            set
            {
                CheckSealed();

                m_list[index] = value;
            }
        }

        public void Add(SimpleField item)
        {
            CheckSealed();

            m_list.Add(item);
        }

        public void Clear()
        {
            CheckSealed();

            m_list.Clear();
        }

        public bool Contains(SimpleField item)
        {
            return m_list.Contains(item);
        }

        public void CopyTo(SimpleField[] array, int arrayIndex)
        {
            m_list.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return m_list.Count; }
        }

        public bool IsReadOnly
        {
            get { return m_sealed; }
        }

        public bool Remove(SimpleField item)
        {
            CheckSealed();

            return m_list.Remove(item);
        }

        public IEnumerator<SimpleField> GetEnumerator()
        {
            return m_list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return m_list.GetEnumerator();
        }
    }
}
