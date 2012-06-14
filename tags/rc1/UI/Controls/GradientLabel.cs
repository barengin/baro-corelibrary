using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using Baro.CoreLibrary.G3;

namespace Baro.CoreLibrary.UI.Controls
{
    public class GradientLabel : UIElement
    {
        public G3Font Font { get; set; }
        public string Text { get; set; }

        public G3Color FontColor { get; set; }
        public G3Color HaloColor { get; set; }

        public Border Border { get; set; }
        public Gradient Gradient { get; set; }

        public GradientLabel()
            : base()
        {
            FontColor = G3Color.WHITE;
            HaloColor = G3Color.BLACK;

            Border = new Border() { Color = G3Color.GRAY, Enabled = false };
            Gradient = new Gradient();
        }

        internal override void MouseDown(System.Drawing.Point p)
        {
        }

        internal override void MouseUp(System.Drawing.Point p)
        {
        }

        public override void Render(G3Canvas g)
        {
            g.BeginDrawing();

            Gradient.Draw(g, this.Bound);

            if (Border.Enabled)
                Border.Draw(g, this.Bound);

            g.DrawTextCenter(Text, UICanvas.Encoding, Font,
                Location.X + Size.Width / 2, Location.Y + Size.Height / 2, 0,
                FontColor, HaloColor);

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
