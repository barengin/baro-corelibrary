using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Baro.CoreLibrary.Extensions;
using Baro.CoreLibrary.G3;

namespace Baro.CoreLibrary.UI.Controls
{
    public class GradientButton : UIElement
    {
        private bool pressed = false;

        public Image MaskImage { get; set; }
        public G3Color MaskImageColor { get; set; }

        public string Text { get; set; }
        public G3Font Font { get; set; }

        public G3Color FontColor { get; set; }
        public G3Color HaloColor { get; set; }

        public Gradient Gradient { get; set; }
        public Border Border { get; set; }

        public GradientButton()
            : base()
        {
            FontColor = G3Color.GRAY;
            HaloColor = G3Color.WHITE;

            MaskImageColor = G3Color.FromRGB(238, 28, 36);

            Border = new Border() { Color = G3Color.GRAY, Enabled = false };
            
            Gradient = new Gradient()
            {
                FromColor = G3Color.FromRGB(247, 243, 247),
                ToColor = G3Color.FromRGB(156, 154, 156),
                UseAlpha = false
            };
        }

        internal override void MouseDown(System.Drawing.Point p)
        {
            if (HitTest(p))
                pressed = true;
        }

        internal override void MouseUp(System.Drawing.Point p)
        {
            if (pressed)
            {
                if (HitTest(p))
                {
                    OnClick(EventArgs.Empty);
                }
            }

            pressed = false;
        }

        public override void Render(G3Canvas g)
        {
            g.BeginDrawing();

            if (pressed)
            {
                Gradient.DrawDarker(g, this.Bound, 30);
            }
            else
            {
                Gradient.Draw(g, this.Bound);
            }

            g.EndDrawing();

            if (MaskImage != null)
            {
                using (Graphics gx = g.Surface.WindowsGraphics)
                {
                    gx.DrawImageTransparent(MaskImage, this.Bound, MaskImageColor.WindowsColor);
                }
            }

            if (!string.IsNullOrEmpty(Text))
            {
                g.BeginDrawing();

                if (Border.Enabled)
                    Border.Draw(g, this.Bound);

                g.DrawTextCenter(Text, UICanvas.Encoding, Font, Location.X + (Size.Width / 2),
                                                          Location.Y + (Size.Height / 2), 0,
                                                          FontColor, HaloColor);

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
