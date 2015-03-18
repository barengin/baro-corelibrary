using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Baro.CoreLibrary.G3;

namespace Baro.CoreLibrary.UI.Controls
{
    public abstract class UIElement : IDisposable
    {
        public bool Enable { get; set; }
        public bool Visible { get; set; }
        public virtual Size Size { get; set; }
        public virtual Point Location { get; set; }
        public virtual UIForm Parent { get; internal set; }
        public Rectangle Bound { get { return new Rectangle(Location.X, Location.Y, Size.Width, Size.Height); } }
        public Point Center { get { return new Point(Location.X + (Size.Width / 2), Location.Y + (Size.Height / 2)); } }
        public object Tag { get; set; }

        internal abstract void MouseDown(Point p);
        internal abstract void MouseUp(Point p);
        internal abstract void MouseMove(Point p);

        public abstract void Render(G3Canvas g);

        #region Events
        public event EventHandler OnClickEvent;

        protected virtual void OnClick(EventArgs e)
        {
            if (OnClickEvent != null)
            {
                OnClickEvent(this, e);
            }
        }

        #endregion

        public bool HitTest(Point point)
        {
            return this.Bound.Contains(point);
        }

        public UIElement()
        {
            this.Enable = true;
            this.Visible = true;
            this.Size = new Size(80, 40);
            this.Location = new Point(0, 0);
        }

        #region IDisposable Members

        protected abstract void Dispose(bool disposing);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
