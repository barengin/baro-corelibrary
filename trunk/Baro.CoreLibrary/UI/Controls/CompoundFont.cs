using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Baro.CoreLibrary.G3;

namespace Baro.CoreLibrary.UI.Controls
{
    public class CompoundFont
    {
        public G3Font Font { get; set; }
        public G3Color FontColor { get; set; }
        public G3Color HaloColor { get; set; }

        public CompoundFont(G3Font font, G3Color fontColor, G3Color haloColor)
        {
            this.Font = font;
            this.FontColor = fontColor;
            this.HaloColor = haloColor;
        }

        //public void Assign(FontStyle s)
        //{
        //    this.Font = s.Font;
        //    this.FontColor = s.FontColor;
        //    this.HaloColor = s.HaloColor;
        //}
    }
}
