using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Baro.CoreLibrary.Extensions;
using Baro.CoreLibrary.G3;

namespace Baro.CoreLibrary.UI.Controls
{
    public class Textbox : UIElement
    {
        public bool Focused { get; set; }
        public string Text { get; set; }

        public G3Font Font { get; set; }
        public G3Color FontColor { get; set; }
        public G3Color HaloColor { get; set; }

        public Textbox()
            : base()
        {
            FontColor = G3Color.BLACK;
            HaloColor = G3Color.WHITE;

            Text = string.Empty;
        }

        public void AddChar(char c, KeyboardControl t)
        {
            switch (t)
            {
                case KeyboardControl.Char:
                    Text = Text + c;
                    break;

                case KeyboardControl.SpaceBar:
                    Text = Text + '\u0020';
                    break;

                case KeyboardControl.NumericKeypad:
                    break;

                case KeyboardControl.Back:
                    break;

                case KeyboardControl.Backspace:
                    if (Text.Length > 1)
                    {
                        Text = Text.Substring(0, Text.Length - 1);
                    }
                    else if (Text.Length == 1)
                    {
                        Text = string.Empty;
                    }
                    break;

                case KeyboardControl.Enter:
                    break;

                case KeyboardControl.AlphaKeypad:
                    break;
            }
        }

        internal override void MouseDown(System.Drawing.Point p)
        {
            if (this.HitTest(p))
            {
                MakeOthersUnFocused();
                Focused = true;
            }
        }

        private void MakeOthersUnFocused()
        {
            foreach (var e in UICanvas)
            {
                if (e is Textbox)
                {
                    ((Textbox)e).Focused = false;
                }
            }
        }

        internal override void MouseUp(System.Drawing.Point p)
        {
        }

        public override void Render(G3Canvas g)
        {
            if (Text == null)
                Text = string.Empty;

            g.BeginDrawing();

            Rectangle r = this.Bound;
            g.FillRectangle(r, G3Color.WHITE);
            g.Rectangle(r, G3Color.GRAY);

            byte[] chars;

            if (Focused)
            {
                chars = UICanvas.Encoding.GetBytes(Text + "\u25cf");
            }
            else
            {
                chars = UICanvas.Encoding.GetBytes(Text);
            }

            chars = CropText(chars, Size.Width);

            g.DrawTextUL(chars, Font,
                Location.X + 2,
                Location.Y + ((Size.Height - Font.FontHeight) / 2),
                FontColor, HaloColor);

            g.EndDrawing();
        }

        private byte[] CropText(byte[] chars, int w)
        {
            if (chars == null || chars.Length == 1)
                return chars;

            int width = Font.TextWidth(chars);

            while (width >= (w - 3))
            {
                chars = chars.Clone(1, chars.Length - 1);
                width = Font.TextWidth(chars);
            }

            return chars;
        }

        protected override void Dispose(bool disposing)
        {
        }

        internal override void MouseMove(Point p)
        {
        }
    }
}
