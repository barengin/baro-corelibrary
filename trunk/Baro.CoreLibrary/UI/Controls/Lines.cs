using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Baro.CoreLibrary.G3;

namespace Baro.CoreLibrary.UI.Controls
{
    public class Lines: UIElement
    {
        public List<Point[]> LineList { get; private set; }
        public G3Color LineColor { get; set; }

        public Lines()
        {
            LineList = new List<Point[]>();
        }

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
            g.BeginDrawing();
            
            foreach (var item in LineList)
            {
                for (int k = 0; k < item.Length - 1; k++)
                {
                    g.DrawLineSegmentSafe(item[k].X, item[k].Y, item[k + 1].X, item[k + 1].Y, LineColor);
                }
            }

            g.EndDrawing();
        }

        protected override void Dispose(bool disposing)
        {
        }

        #region ICloneable Members

        public Lines Clone()
        {
            return new Lines { LineList = this.LineList.ToList<Point[]>() };
        }

        #endregion
    }
}
