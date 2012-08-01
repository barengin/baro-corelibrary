using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNETCF.Net
{
    /// <summary>
    /// Represents a strongly typed list of RasEntry objects that can be accessed by index or name.
    /// </summary>
    public class RasEntryCollection : IEnumerable<RasEntry>
    {
        private List<RasEntry> m_entries = new List<RasEntry>();

        internal RasEntryCollection()
        {
        }

        internal void Add(RasEntry entry)
        {
            m_entries.Add(entry);
        }

        /// <summary>
        /// Gets the number of RasEntry items contained in the collection
        /// </summary>
        public int Count
        {
            get { return m_entries.Count; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the RasEntryCollection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<RasEntry> GetEnumerator()
        {
            return m_entries.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return m_entries.GetEnumerator();
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index is less than 0.-or-index is equal to or greater than RasEntryCollection.Count.</exception>
        public RasEntry this[int index]
        {
            get { return m_entries[index]; }
        }

        /// <summary>
        /// Gets or sets the element at the specified by the entry name.
        /// </summary>
        /// <param name="entryName">The name of the RasEntry object.</param>
        /// <returns>The element with the specified entryName null if not found.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index is less than 0.-or-index is equal to or greater than RasEntryCollection.Count.</exception>
      
        public RasEntry this[string entryName]
        {
            get 
            {
                RasEntry entry = m_entries.Find(new Predicate<RasEntry>(delegate(RasEntry re)
                {
                    return (string.Compare(re.Name, entryName, true) == 0);
                }
                ));
                return entry;
                //foreach (RasEntry entry in m_entries)
                //{
                //    if (string.Compare(entry.Name, entryName, true) == 0)
                //    {
                //        return entry;
                //    }
                //}

                //return null;
            }
        }
    }
}
