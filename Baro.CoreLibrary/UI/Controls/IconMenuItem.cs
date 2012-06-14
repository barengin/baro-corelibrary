using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Baro.CoreLibrary.Extensions;
using Baro.CoreLibrary.G3;

namespace Baro.CoreLibrary.UI.Controls
{
    public enum IconMenuType
    {
        Click,
        Toggle
    }

    public class IconMenuItem : ImageButton
    {
        public IconMenuType Type { get; set; }
        public Image ToggleImage { get; set; }
        public bool Toggled { get; set; }

        public IconMenuItem()
        {
            this.Type = IconMenuType.Click;
        }

        public override void Render(G3Canvas g)
        {
            Graphics gx = g.Surface.WindowsGraphics;

            if (Image != null)
            {
                gx.DrawImageTransparentUnstreched(Image, this.Center);
            }

            if (!string.IsNullOrEmpty(Text) || _pressed)
            {
                g.BeginDrawing();

                g.DrawTextCenter(Text, UICanvas.Encoding, Font, Location.X + (Size.Width / 2),
                                                          Location.Y + (Size.Height / 2), 0,
                                                          G3Color.GRAY, G3Color.WHITE);

                if (_pressed)
                {
                    g.DarkBox(Location.X, Location.Y, Size.Width, Size.Height, 9);
                }

                g.EndDrawing();
            }
        }

        internal override void MouseUp(Point p)
        {
            if (_pressed)
            {
                if (HitTest(p))
                {
                    if (this.Type == IconMenuType.Toggle)
                        this.Toggled = !this.Toggled;

                    OnClick(EventArgs.Empty);
                }
            }

            _pressed = false;
        }
    }
}
