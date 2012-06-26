using System;
using Baro.CoreLibrary.Extensions;
using System.Collections.Generic;
using System.Text;
using Baro.CoreLibrary.G3;
using System.Drawing;

namespace Baro.CoreLibrary.UI.Controls
{
    public class ListboxItem: UIElement
    {
        internal override void MouseDown(Point p)
        {
        }

        internal override void MouseUp(Point p)
        {
        }

        internal override void MouseMove(Point p)
        {
        }

        public override void Render(G3Canvas g)
        {
            using (Graphics gx = g.Surface.WindowsGraphics)
            {
                gx.DrawRoundedRectangle(new Pen(Color.Red), Color.Black, this.Bound, this.Size);
            }
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}
