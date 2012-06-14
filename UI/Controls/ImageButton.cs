using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Baro.CoreLibrary.Extensions;
using Baro.CoreLibrary.G3;

namespace Baro.CoreLibrary.UI.Controls
{
    public class ImageButton: UIElement
    {
        protected bool _pressed = false;

        public Image Image { get; set; }
        public string Text { get; set; }
        public G3Font Font { get; set; }

        public G3Color FontColor { get; set; }
        public G3Color HaloColor { get; set; }

        internal override void MouseDown(System.Drawing.Point p)
        {
            if (HitTest(p))
                _pressed = true;
        }

        internal override void MouseUp(System.Drawing.Point p)
        {
            if (_pressed)
            {
                if (HitTest(p))
                {
                    OnClick(EventArgs.Empty);
                }
            }

            _pressed = false;
        }

        public override void Render(G3Canvas g)
        {
            Graphics gx = g.Surface.WindowsGraphics;

            if (Image != null)
            {
                gx.DrawImageTransparent(Image, this.Bound);
            }

            if (!string.IsNullOrEmpty(Text) || _pressed)
            {
                g.BeginDrawing();

                g.DrawTextCenter(Text, UICanvas.Encoding, Font, Location.X + (Size.Width / 2),
                                                          Location.Y + (Size.Height / 2), 0,
                                                          FontColor, HaloColor);

                if (_pressed)
                {
                    g.DarkBox(Location.X, Location.Y, Size.Width, Size.Height, 9);
                }
                
                g.EndDrawing();
            }
        }

        protected override void Dispose(bool disposing)
        {
        }

        internal override void MouseMove(Point p)
        {
        }
    }
}
 