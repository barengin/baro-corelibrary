using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Baro.CoreLibrary.G3;

namespace Baro.CoreLibrary.UI.Controls
{
    public sealed class Gradient
    {
        public bool UseAlpha { get; set; }

        public byte FromAlpha { get; set; }
        public byte ToAlpha { get; set; }

        public G3Color FromColor { get; set; }
        public G3Color ToColor { get; set; }

        public void DrawDarker(G3Canvas canvas, Rectangle bound, byte level)
        {
            if (UseAlpha)
            {
                canvas.Gradient(bound.Left, bound.Top, bound.Width, bound.Height, 
                    FromColor.Darker(level), ToColor.Darker(level), FromAlpha, ToAlpha);
            }
            else
            {
                canvas.Gradient(bound.Left, bound.Top, bound.Width, bound.Height, 
                    FromColor.Darker(level), ToColor.Darker(level));
            }
        }

        public void Draw(G3Canvas canvas, Rectangle bound)
        {
            if (UseAlpha)
            {
                canvas.Gradient(bound.Left, bound.Top, bound.Width, bound.Height, 
                    FromColor, ToColor, FromAlpha, ToAlpha);
            }
            else
            {
                canvas.Gradient(bound.Left, bound.Top, bound.Width, bound.Height, 
                    FromColor, ToColor);
            }
        }
    }
}
