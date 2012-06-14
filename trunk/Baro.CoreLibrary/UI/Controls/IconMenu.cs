using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Baro.CoreLibrary.G3;

namespace Baro.CoreLibrary.UI.Controls
{
    public class IconMenu : UIElement
    {
        public IconMenuItemList Items { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Gradient Gradient { get; set; }
        public Border Border { get; set; }

        public Size IconSize { get { return new Size(Size.Width / Width, Size.Height / Height); } }

        public IconMenu(int width, int height)
            : base()
        {
            this.Width = width;
            this.Height = height;

            this.Border = new Border()
            {
                Enabled = true,
                Color = G3Color.GRAY
            };

            this.Gradient = new Gradient()
            {
                UseAlpha = false
            };

            this.Items = new IconMenuItemList();
        }

        internal override void MouseDown(System.Drawing.Point p)
        {
            Items.MouseDown(new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, p.X, p.Y, 0));
        }

        internal override void MouseUp(System.Drawing.Point p)
        {
            Items.MouseUp(new System.Windows.Forms.MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, p.X, p.Y, 0));
        }

        public override void Render(G3Canvas g)
        {
            if (Items.Modified)
            {
                CreateMenu();
                Items.Modified = false;
            }

            g.BeginDrawing();

            Gradient.Draw(g, this.Bound);
            Border.Draw(g, this.Bound);

            g.EndDrawing();

            Items.Render(g);
        }

        private void CreateMenu()
        {
            int w = IconSize.Width;
            int h = IconSize.Height;

            for (int k = 0; k < (w * h); k++)
            {
                if (k < Items.Count)
                {
                    Items[k].Size = new System.Drawing.Size(w, h);

                    int x = (w * (k % Width)) + Location.X;
                    int y = (h * (k / Height)) + Location.Y;

                    Items[k].Location = new System.Drawing.Point(x, y);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            Items.Clear();
        }

        internal override void MouseMove(Point p)
        {
        }
    }
}
