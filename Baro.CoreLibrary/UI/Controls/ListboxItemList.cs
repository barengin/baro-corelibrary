using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Baro.CoreLibrary.G3;

namespace Baro.CoreLibrary.UI.Controls
{
    public class ListboxItemList: IList<ListboxItem>
    {
        private UICanvas _canvas = new UICanvas();
        internal bool Modified { get; set; }

        public ListboxItemList()
        {
            Modified = true;
        }

        #region IList<ListboxItem> Members

        public int IndexOf(ListboxItem item)
        {
            return _canvas.IndexOf(item);
        }

        public void Insert(int index, ListboxItem item)
        {
            _canvas.Insert(index, item);
            Modified = true;
        }

        public void RemoveAt(int index)
        {
            _canvas.RemoveAt(index);
            Modified = true;
        }

        public ListboxItem this[int index]
        {
            get
            {
                return (ListboxItem)_canvas[index];
            }
            set
            {
                _canvas[index] = value;
                Modified = true;
            }
        }

        #endregion

        #region ICollection<ListboxItem> Members

        public void Add(ListboxItem item)
        {
            _canvas.Add(item);
            Modified = true;
        }

        public void Clear()
        {
            _canvas.Clear();
            Modified = true;
        }

        public bool Contains(ListboxItem item)
        {
            return _canvas.Contains(item);
        }

        public void CopyTo(ListboxItem[] array, int arrayIndex)
        {
            _canvas.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _canvas.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(ListboxItem item)
        {
            Modified = true;
            return _canvas.Remove(item);
        }

        #endregion

        #region IEnumerable<ListboxItem> Members

        public IEnumerator<ListboxItem> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _canvas.GetEnumerator();
        }

        #endregion

        internal void MouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            _canvas.MouseDown(e);
        }

        internal void MouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            _canvas.MouseUp(e);
        }

        internal void MouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            _canvas.MouseMove(e);
        }

        internal void Render(G3Canvas g, Listbox listbox)
        {
            ListboxItem l = listbox.Items[0];
            l.Size = listbox.ItemSize;
            l.Location = listbox.Location;

            l.Render(g);

            //int k = listbox.StartIndex;
            //int h = listbox.ItemSize.Height;

            //while (k < _canvas.Count && k < listbox.RowCount)
            //{

            //    k++;
            //}
        }
    }
}
