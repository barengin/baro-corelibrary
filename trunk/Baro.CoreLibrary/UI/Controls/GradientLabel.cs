using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using Baro.CoreLibrary.G3;

namespace Baro.CoreLibrary.UI.Controls
{
    public class GradientLabel : UIElement
    {
        public string Text { get; set; }
        public CompundFont FontStyle { get; set; }

        public Border Border { get; set; }
        public Gradient Gradient { get; set; }

        public GradientLabel()
            : base()
        {
            this.FontStyle = new CompundFont(null, G3Color.WHITE, G3Color.BLACK);
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

            g.DrawText(Text, UICanvas.Encoding, FontStyle.Font,
                 FontStyle.FontColor, FontStyle.HaloColor, TextAlign.Center, this.Bound);

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
