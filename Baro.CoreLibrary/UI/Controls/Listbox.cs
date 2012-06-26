using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Baro.CoreLibrary.UI.Controls
{
    public class Listbox : UIElement
    {
        internal int RowCount { get; private set; }

        public ListboxItemList Items { get; private set; }
        public Size ItemSize { get { return new Size(this.Size.Width, this.Size.Height / RowCount); } }

        public override Size Size
        {
            get
            {
                return base.Size;
            }
            set
            {
                // base.Size = value;
            }
        }

        public int StartIndex { get; set; }
        public int SelectedIndex { get; set; }

        public Listbox(int rowCountInView)
            : base()
        {
            this.RowCount = rowCountInView;
            this.Items = new ListboxItemList();
        }

        internal override void MouseDown(System.Drawing.Point p)
        {
            Items.MouseDown(new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, p.X, p.Y, 0));
        }

        internal override void MouseUp(System.Drawing.Point p)
        {
            Items.MouseUp(new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, p.X, p.Y, 0));
        }

        internal override void MouseMove(System.Drawing.Point p)
        {
            Items.MouseMove(new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, p.X, p.Y, 0));            
        }

        public override void Render(Baro.CoreLibrary.G3.G3Canvas g)
        {
            Items.Render(g, this);
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}
