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

        public void DrawDarker(G3Canvas canvas, int x, int y, int w, int h, byte level)
        {
            if (UseAlpha)
            {
                canvas.Gradient(x, y, w, h,
                    FromColor.Darker(level), ToColor.Darker(level), FromAlpha, ToAlpha);
            }
            else
            {
                canvas.Gradient(x, y, w, h,
                    FromColor.Darker(level), ToColor.Darker(level));
            }
        }

        public void DrawDarker(G3Canvas canvas, Rectangle bound, byte level)
        {
            DrawDarker(canvas, bound.Left, bound.Top, bound.Width, bound.Height, level);
        }

        public void Draw(G3Canvas canvas, int x, int y, int w, int h)
        {
            if (UseAlpha)
            {
                canvas.Gradient(x, y, w, h, FromColor, ToColor, FromAlpha, ToAlpha);
            }
            else
            {
                canvas.Gradient(x, y, w, h, FromColor, ToColor);
            }
        }

        public void Draw(G3Canvas canvas, Rectangle bound)
        {
            Draw(canvas, bound.Left, bound.Top, bound.Width, bound.Height);
        }
    }
}
