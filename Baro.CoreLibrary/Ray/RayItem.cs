using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Baro.CoreLibrary.Ray
{
    public abstract class RayItem<T> : RayHandler, IRayItem<T>
    {
        #region Flyweight Lock
        private volatile ReaderWriterLockSlim _flylock = null;

        protected ReaderWriterLockSlim _lock
        {
            get { return _flylock ?? (_flylock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion)); }
        }

        #endregion

        #region Locking Tools
        protected TResult WriterLock<TResult>(Func<TResult> writeOp)
        {
            _lock.EnterWriteLock();

            try
            {
                return writeOp();
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        protected void WriterLock(Action writeOp)
        {
            _lock.EnterWriteLock();

            try
            {
                if (writeOp != null) writeOp();
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        protected TResult ReaderLock<TResult>(Func<TResult> readOp)
        {
            _lock.EnterReadLock();

            try
            {
                return readOp();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        protected void ReaderLock(Action readOp)
        {
            _lock.EnterReadLock();

            try
            {
                if (readOp != null) readOp();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        #endregion

        public abstract T Clone();

        public abstract XmlNode CreateXmlNode(XmlDocument xmlDoc);

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}
