using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Baro.CoreLibrary.UI.Controls
{
    public class StaticImage: UIElement
    {
        public Image Image { get; set; }
        public bool Stretch { get; set; }

        internal override void MouseDown(System.Drawing.Point p)
        {
        }

        internal override void MouseUp(System.Drawing.Point p)
        {
        }

        internal override void MouseMove(System.Drawing.Point p)
        {
        }

        public override void Render(Baro.CoreLibrary.G3.G3Canvas g)
        {
            if (this.Image == null)
                return;

            using (Graphics gx = g.Surface.WindowsGraphics)
            {
                if (this.Stretch)
                {
                    gx.DrawImage(this.Image, this.Bound, 
                        new Rectangle(0, 0, Size.Width, Size.Height), GraphicsUnit.Pixel);
                }
                else
                {
                    gx.DrawImage(this.Image, Location.X, Location.Y);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}
