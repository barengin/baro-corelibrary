using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Baro.CoreLibrary.SimpleDB
{
    public enum DBState
    {
        /// <summary>
        /// Database açık ve kullanılabilir.
        /// </summary>
        Open,

        /// <summary>
        /// Database tamamen kapalı. Open komutu ile yeni bir DB açılmalı.
        /// </summary>
        Close,

        /// <summary>
        /// Database düşük bellek yönetimi moduna geçmiş. Bu modda database'i kullanamazsınız.
        /// Kullanabilmek için Restore() komutunu kullanın.
        /// </summary>
        Hibernate
    }

    public sealed class DB : IDisposable, IEnumerable<Row>
    {
        private bool disposed = false;
        private FileStream m_file;
        private string m_filename;
        private int m_offset;

        public Header Header { get; private set; }

        public DBState State
        {
            get
            {
                if (m_filename == null)
                    return DBState.Close;
                else
                    if (m_file == null)
                        return DBState.Hibernate;

                return DBState.Open;
            }
        }

        public void Hibernate()
        {
            // Close the file
            if (m_file != null)
            {
                m_file.Close();
                m_file = null;
            }

            // Clear the header
            this.Header = null;
        }

        public void Restore()
        {
            if (m_filename == null)
                throw new InvalidOperationException("There is nothing to restore");

            Open(m_filename);
        }

        public void Close()
        {
            // Close the file
            if (m_file != null)
            {
                m_file.Close();
                m_file = null;
            }

            // throw it
            m_filename = null;
        }

        public void Open(string filename)
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException("Dosya bulunamadı", filename);
            }

            // FileStream
            m_file = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            
            // Dosya adını Stream'den sonra eşitle.
            m_filename = filename;

            // Header Size
            BinaryReader br = new BinaryReader(m_file);
            int headerSize = br.ReadInt32();

            // Header record
            byte[] header = new byte[headerSize];
            m_file.Read(header, 0, headerSize);

            // File total header size
            // Header'ın 4byte size bilgisi + kendisi
            this.m_offset = headerSize + 4;

            // Header
            this.Header = new Header(header);

            // Row data size as raw
            this.RawRecordSize = Header.GetRawRecordSize();
        }

        internal int RawRecordSize { get; private set; }

        internal void GetRawRecord(int index, byte[] data)
        {
            m_file.Position = (index * RawRecordSize) + m_offset;
            int k = m_file.Read(data, 0, RawRecordSize);

            if (k != RawRecordSize)
                throw new SimpleDBException("Gereken bilgi okunamadı");
        }

        public int RecordCount { get { return (((int)m_file.Length - m_offset) / RawRecordSize); } }

        public Row CreateNewRow()
        {
            return new Row(this.Header);
        }
        
        public void DeleteRow(int index)
        {

        }

        public void AddRow(Row r)
        {
            if (r.Header != this.Header)
                throw new SimpleDBException("Eklenmek istenen Row nesnesinin içindeki Header bilgisi DB ile uyuşmuyor. Yeni kayıt eklemek için DB içindeki CreateNewRow() komutunu kullanın.");
        }
        
        public Row FirstRow()
        {
            return null;
        }

        #region Dispose
        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    Close();
                }

                // shared cleanup logic
                disposed = true;
            }

            // if it's necessary!
            // base.Dispose(disposing);
        }

        ~DB()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);

            // Bu durumda bu kod gerekli değil. Close komutundan sonra tekrar Open çağırılabilir.
            // GC.SuppressFinalize(this);
        }

        #endregion

        public IEnumerator<Row> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
