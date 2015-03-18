using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Baro.CoreLibrary.G3;

namespace Baro.CoreLibrary.UI.Controls
{
    public sealed class Border
    {
        public bool Enabled { get; set; }
        public G3Color Color { get; set; }

        public void Draw(G3Canvas g, Rectangle bound)
        {
            if (Enabled)
                g.Rectangle(bound, Color);
        }
    }
}
