using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Baro.CoreLibrary.G3;
using System.Drawing;

namespace Baro.CoreLibrary.UI.Controls
{
    public class StaticLabel: UIElement
    {
        public string Text { get; set; }
        public CompoundFont FontStyle { get; set; }

        public StaticLabel()
            : base()
        {
            this.FontStyle = new CompoundFont(null, G3Color.BLACK, G3Color.WHITE);
        }

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
            g.DrawText(Text, UICanvas.Encoding, FontStyle.Font, FontStyle.FontColor, 
                FontStyle.HaloColor, TextAlign.Left, this.Bound);
            g.EndDrawing();
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}
