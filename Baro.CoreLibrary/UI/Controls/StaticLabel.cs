using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Baro.CoreLibrary.G3;

namespace Baro.CoreLibrary.UI.Controls
{
    public class StaticLabel: UIElement
    {
        public string Text { get; set; }
        public G3Font Font { get; set; }
        public G3Color FontColor { get; set; }
        public G3Color HaloColor { get; set; }

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
            g.BeginDrawing();
            g.DrawTextUL(Text, UICanvas.Encoding, Font, Location.X, Location.Y, FontColor, HaloColor);
            g.EndDrawing();
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}
