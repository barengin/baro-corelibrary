using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Baro.CoreLibrary.G3;

namespace Baro.CoreLibrary.UI.Controls
{
    public class Checkbox : UIElement
    {
        public Image CheckedImage { get; set; }
        public Image UncheckedImage { get; set; }

        public string Text { get; set; }

        public CompoundFont FontStyle { get; set; }

        public bool Checked { get; set; }

        // First click is inside the checkbox, we can continue.
        private bool inSide = false;

        public Checkbox()
            : base()
        {
            this.FontStyle = new CompoundFont(null, G3Color.WHITE, G3Color.BLACK);
        }

        internal override void MouseDown(System.Drawing.Point p)
        {
            if (HitTest(p))
                inSide = true;
        }

        internal override void MouseUp(System.Drawing.Point p)
        {
            if (inSide)
            {
                if (HitTest(p))
                {
                    this.Checked = !this.Checked;
                    OnClick(EventArgs.Empty);
                }
            }

            inSide = false;
        }

        public override void Render(G3Canvas g)
        {
            {
                Graphics gx = g.Surface.WindowsGraphics;
                
                if (this.Checked)
                {
                    if (CheckedImage != null)
                    {
                        gx.DrawImage(CheckedImage, Location.X, Location.Y + ((Size.Height - CheckedImage.Height) / 2));
                    }
                }
                else
                {
                    if (UncheckedImage != null)
                    {
                        gx.DrawImage(UncheckedImage, Location.X, Location.Y + ((Size.Height - UncheckedImage.Height) / 2));
                    }
                }
            }

            g.BeginDrawing();

            Rectangle r = this.Bound;
            r.Offset(CheckedImage.Width, 0);

            g.DrawText(Text, Parent.Encoding, FontStyle.Font, FontStyle.FontColor, FontStyle.HaloColor,
                 TextAlign.Left, r);

            g.EndDrawing();
        }

        protected override void Dispose(bool disposing)
        {
        }

        internal override void MouseMove(Point p)
        {
        }
    }
}
