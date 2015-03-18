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
        public CompoundFont FontStyle { get; set; }
        public TextAlign TextAlign { get; set; }

        public ImageButton()
            : base()
        {
            this.FontStyle = new CompoundFont(null, G3Color.BLACK, G3Color.WHITE);
            this.TextAlign = TextAlign.Center;
        }

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
            {
                if (Image != null)
                {
                    Graphics gx = g.Surface.WindowsGraphics;
                    gx.DrawImageTransparent(Image, this.Bound);
                }
            }

            if (!string.IsNullOrEmpty(Text) || _pressed)
            {
                g.BeginDrawing();

                g.DrawText(Text, Parent.Encoding, FontStyle.Font, FontStyle.FontColor, FontStyle.HaloColor,
                    this.TextAlign, this.Bound);

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
 