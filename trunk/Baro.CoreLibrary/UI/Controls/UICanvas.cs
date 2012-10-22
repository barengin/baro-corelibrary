using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Baro.CoreLibrary.Text;
using Baro.CoreLibrary.G3;

namespace Baro.CoreLibrary.UI.Controls
{
    public class UICanvas : List<UIElement>
    {
        internal UIForm Parent { get; set; }

        public void Render(G3Canvas g)
        {
            foreach (var e in this)
            {
                if (e.Visible) e.Render(g);
            }
        }

        public void DisableAll()
        {
            foreach (var item in this)
            {
                item.Enable = false;
            }
        }

        public void EnableAll()
        {
            foreach (var item in this)
            {
                item.Enable = true;
            }
        }

        public new void Clear()
        {
            foreach (var e in this)
            {
                e.Dispose();
            }

            base.Clear();
        }

        public new void Add(UIElement e)
        {
            e.Parent = this.Parent;
            base.Add(e);
        }

        public T Add<T>(Func<T> build) where T : UIElement
        {
            T element = build();
            Add(element as UIElement);
            return element;
        }

        internal void MouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            Point p = new Point(e.X, e.Y);

            for (int i = this.Count - 1; i >= 0; i--)
            {
                if (i < this.Count)
                {
                    UIElement m = this[i];

                    if (m.Visible && m.Enable)
                    {
                        m.MouseDown(p);

                        if (this.Parent != null && this.Parent.NewActivityLoaded)
                            break;
                    }
                }
            }
        }

        internal void MouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            Point p = new Point(e.X, e.Y);

            for (int i = this.Count - 1; i >= 0; i--)
            {
                if (i < this.Count)
                {
                    UIElement m = this[i];

                    if (m.Visible && m.Enable)
                    {
                        m.MouseUp(p);

                        if (this.Parent != null && this.Parent.NewActivityLoaded)
                            break;
                    }
                }
            }
        }

        internal void MouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            Point p = new Point(e.X, e.Y);

            for (int i = this.Count - 1; i >= 0; i--)
            {
                if (i < this.Count)
                {
                    UIElement m = this[i];

                    if (m.Visible && m.Enable)
                    {
                        m.MouseMove(p);

                        if (this.Parent != null && this.Parent.NewActivityLoaded)
                            break;
                    }
                }
            }
        }
    }
}
