using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Baro.CoreLibrary.IO.NativeFile;

namespace Baro.CoreLibrary.Cache
{
    public sealed class CacheStream2
    {
        private CircularQueue<byte[]> m_queue;
        private NativeFileReader m_file;
        private int m_bufferBlockSize;
        private int m_bufferBlockCount;

        public CacheStream2(int bufferBlockSize, int bufferBlockCount, string filename)
        {
            m_bufferBlockSize = bufferBlockSize;
            m_bufferBlockCount = bufferBlockCount;
            m_file = new NativeFileReader(filename);
            m_queue = new CircularQueue<byte[]>(bufferBlockCount);
        }

        public int Size
        {
            get { return m_file.Size; }
        }

        public int Position
        {
            get
            {
                return m_file.Position;
            }
            set
            {
                m_file.Position = value;
            }
        }

        private static void CopyTo(byte[] source, int sourceIndex,
            byte[] dest, ref int destIndex, int count)
        {
            Array.Copy(source, sourceIndex, dest, destIndex, count);
            destIndex += count;
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            int oldPos = m_file.Position;
            int currentIndex = offset;

            int sid = m_file.Position / this.m_bufferBlockSize;
            int fid = (m_file.Position + count - 1) / this.m_bufferBlockSize;

            int readstart = m_file.Position % m_bufferBlockSize;
            int readstartsize = Math.Min(m_bufferBlockSize - readstart, count);

            CacheItem<byte[]> cache = GetCache(sid);
            CopyTo(cache.Cache, readstart, buffer, ref currentIndex, readstartsize);

            if ((fid - sid) > 1)
            {
                for (int c = sid + 1; c < fid; c++)
                {
                    CacheItem<byte[]> cache2 = GetCache(c);
                    CopyTo(cache2.Cache, 0, buffer, ref currentIndex, m_bufferBlockSize);
                }
            }

            if (fid != sid)
            {
                int readfinish = 0;
                int readfinishsize = count - currentIndex; // (m_file.Position + count) % m_bufferBlockSize;

                CacheItem<byte[]> cache3 = GetCache(fid);
                CopyTo(cache3.Cache, readfinish, buffer, ref currentIndex, readfinishsize);
            }

            m_file.Position = oldPos + count;
            return count;
        }

        private CacheItem<byte[]> GetCache(int pid)
        {
            CacheItem<byte[]> c;

            if (m_queue.Get(pid, out c))
            {
                return c;
            }
            else
            {
                // Buffer yarat
                byte[] b = new byte[m_bufferBlockSize];
                
                // Cache yarat
                c = new CacheItem<byte[]>();

                // Cache'e buffer set et
                c.Cache = b;

                // Dosyayı set et
                m_file.Position = pid * m_bufferBlockSize;

                // Dosyadan bir block oku
                m_file.Read(b, m_bufferBlockSize);
                
                // Listemize kaydet
                m_queue.Add(pid, c);
                
                return c;
            }
        }

        public void Close()
        {
            m_queue.ClearAll();
            m_file.Close();
        }
    }
}
