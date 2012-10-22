using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Baro.CoreLibrary.G3;
using System.Drawing;

namespace Baro.CoreLibrary.UI.Controls
{
    public class ListboxItemList : IList<ListboxItem>
    {
        private UICanvas _canvas = new UICanvas();
        internal bool Modified { get; set; }

        public ListboxItemList(UIForm parent)
        {
            Modified = true;
            _canvas.Parent = parent;
        }

        #region IList<ListboxItem> Members

        public int IndexOf(ListboxItem item)
        {
            return _canvas.IndexOf(item);
        }

        public void Insert(int index, ListboxItem item)
        {
            item.BelongList = this;
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
                value.BelongList = this;
                _canvas[index] = value;
                Modified = true;
            }
        }

        #endregion

        #region ICollection<ListboxItem> Members

        public void Add(ListboxItem item)
        {
            item.BelongList = this;
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
            Calculate(listbox);

            for (int k = listbox.StartIndex; k < (listbox.StartIndex + listbox.RowCount); k++)
            {
                if (k < this.Count)
                {
                    this[k].SelectedItemColor = listbox.SelectedItemColor;
                    this[k].ItemColor = listbox.ItemColor;
                    this[k].BorderColor = listbox.ItemBorderColor;

                    this[k].Render(g);
                }
            }
        }

        private void Calculate(Listbox listbox)
        {
            int h = listbox.Size.Height / listbox.RowCount;
            int t = 0;

            for (int k = listbox.StartIndex; k < (listbox.StartIndex + listbox.RowCount); k++)
            {
                if (k < this.Count)
                {
                    this[k].Location = new Point(listbox.Location.X, (h * t) + listbox.Location.Y);
                    this[k].Size = new Size(listbox.Size.Width, h - 2);
                }

                t++;
            }
        }

        internal void UnselectAll()
        {
            for (int k = 0; k < this.Count; k++)
            {
                this[k].Selected = false;
            }
        }

        public void SetSelected(int index)
        {
            for (int k = 0; k < this.Count; k++)
            {
                this[k].Selected = false;
            }

            this[index].Selected = true;
        }
    }
}
