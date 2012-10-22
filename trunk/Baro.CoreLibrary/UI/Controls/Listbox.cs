using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Baro.CoreLibrary.G3;
using System.Windows.Forms;

namespace Baro.CoreLibrary.UI.Controls
{
    public class Listbox : UIElement
    {
        internal int RowCount { get; private set; }

        public ListboxItemList Items { get; private set; }
        public Size ItemSize { get { return new Size(this.Size.Width, this.Size.Height / RowCount); } }
        public Border Border { get; set; }
        
        public G3Color BackgroundColor { get; set; }
        public G3Color SelectedItemColor { get; set; }
        public G3Color ItemColor { get; set; }
        public G3Color ItemBorderColor { get; set; }

        public int StartIndex { get; set; }
        public int SelectedIndex { get; set; }

        public override UIForm Parent
        {
            get
            {
                return base.Parent;
            }
            internal set
            {
                this.Items = new ListboxItemList(value);
                base.Parent = value;
            }
        }

        public Listbox(int rowCountInView)
            : base()
        {
            this.RowCount = rowCountInView;
            this.Border = new Border() { Color = G3Color.GRAY, Enabled = false };
            this.SelectedItemColor = G3Color.GRAY;
            this.ItemColor = G3Color.WHITE;
            this.ItemBorderColor = G3Color.RED;
        }

        internal override void MouseDown(System.Drawing.Point p)
        {
            Items.MouseDown(new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, p.X, p.Y, 0));
        }

        internal override void MouseUp(System.Drawing.Point p)
        {
            Items.MouseUp(new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, p.X, p.Y, 0));
        }

        internal override void MouseMove(System.Drawing.Point p)
        {
            Items.MouseMove(new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, p.X, p.Y, 0));
        }

        public override void Render(G3Canvas g)
        {
            g.BeginDrawing();

            g.FillRectangle(this.Bound, this.BackgroundColor);

            if (Border.Enabled)
            {
                Border.Draw(g, this.Bound);
            }
            
            g.EndDrawing();

            Items.Render(g, this);
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}
