using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Baro.CoreLibrary.G3;

namespace Baro.CoreLibrary.UI.Controls
{
    public class IconMenuItemList: IList<IconMenuItem>
    {
        private UICanvas _canvas = new UICanvas();
        internal bool Modified { get; set; }

        public IconMenuItemList()
        {
            Modified = true;
        }

        #region IList<IconMenuItem> Members

        public int IndexOf(IconMenuItem item)
        {
            return _canvas.IndexOf(item);
        }

        public void Insert(int index, IconMenuItem item)
        {
            _canvas.Insert(index, item);
            Modified = true;
        }

        public void RemoveAt(int index)
        {
            _canvas.RemoveAt(index);
            Modified = true;
        }

        public IconMenuItem this[int index]
        {
            get
            {
                return (IconMenuItem)_canvas[index];
            }
            set
            {
                _canvas[index] = value;
                Modified = true;
            }
        }

        #endregion

        #region ICollection<IconMenuItem> Members

        public void Add(IconMenuItem item)
        {
            _canvas.Add(item);
            Modified = true;
        }

        public void Clear()
        {
            _canvas.Clear();
            Modified = true;
        }

        public bool Contains(IconMenuItem item)
        {
            return _canvas.Contains(item);
        }

        public void CopyTo(IconMenuItem[] array, int arrayIndex)
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

        public bool Remove(IconMenuItem item)
        {
            Modified = true;
            return _canvas.Remove(item);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _canvas.GetEnumerator();
        }

        #endregion

        #region IEnumerable<IconMenuItem> Members

        IEnumerator<IconMenuItem> IEnumerable<IconMenuItem>.GetEnumerator()
        {
            throw new NotImplementedException();
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

        public void Render(G3Canvas g)
        {
            _canvas.Render(g);
        }
    }
}
