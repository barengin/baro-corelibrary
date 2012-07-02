using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Baro.CoreLibrary.Extensions;
using Baro.CoreLibrary.G3;

namespace Baro.CoreLibrary.UI.Controls
{
    public class ListboxItem : UIElement
    {
        internal G3Color ItemColor { get; set; }
        internal G3Color SelectedItemColor { get; set; }
        internal ListboxItemList BelongList { get; set; }
        internal G3Color BorderColor { get; set; }

        public Image Image { get; set; }

        public string Text1 { get; set; }
        public string Text2 { get; set; }

        public CompoundFont Text1Style { get; set; }
        public CompoundFont Text2Style { get; set; }

        public bool Selected { get; set; }

        public ListboxItem()
            : base()
        {
            Text1Style = new CompoundFont(null, G3Color.BLACK, G3Color.WHITE);
            Text2Style = new CompoundFont(null, G3Color.BLACK, G3Color.WHITE);
        }

        internal override void MouseDown(Point p)
        {
        }

        internal override void MouseUp(Point p)
        {
            if (HitTest(p))
            {
                this.BelongList.UnselectAll();
                Selected = true;

                OnClick(EventArgs.Empty);
            }
        }

        internal override void MouseMove(Point p)
        {
        }

        public override void Render(G3Canvas g)
        {
            Graphics gx = g.Surface.WindowsGraphics;
            
            if (Selected)
            {
                gx.DrawRoundedRectangle(new Pen(BorderColor.WindowsColor), this.SelectedItemColor.WindowsColor, this.Bound, new Size(10, 10));
            }
            else
            {
                gx.DrawRoundedRectangle(new Pen(BorderColor.WindowsColor), this.ItemColor.WindowsColor, this.Bound, new Size(10, 10));
            }

            if (this.Image == null)
            {
                g.BeginDrawing();

                Rectangle r1 = this.Bound;
                r1.Height = r1.Height / 2;
                r1.Offset(2, 0);
                
                // DEBUG: g.Rectangle(r1, G3Color.BLUE);

                byte[] chars = UICanvas.Encoding.GetBytes(Text1);                
                chars = CropText(chars, r1.Width, Text1Style);
                
                g._DrawTextC(chars, Text1Style.Font,
                    r1.X + 2 + (Text1Style.Font.TextWidth(chars) / 2),
                    r1.Y + (r1.Height / 2),
                    Text1Style.FontColor, Text1Style.HaloColor);

                //g.DrawText(Text1, UICanvas.Encoding,
                //    Text1Style.Font, Text1Style.FontColor, Text1Style.HaloColor,
                //    TextAlign.Left, r1);

                r1.Offset(0, r1.Height);
                
                // DEBUG: g.Rectangle(r1, G3Color.GREEN);

                chars = UICanvas.Encoding.GetBytes(Text2);
                chars = CropText(chars, r1.Width, Text2Style);

                g._DrawTextC(chars, Text2Style.Font,
                    r1.X + 2 + (Text2Style.Font.TextWidth(chars) / 2),
                    r1.Y + (r1.Height / 2),
                    Text2Style.FontColor, Text2Style.HaloColor);

                //g.DrawText(Text2, UICanvas.Encoding,
                //    Text2Style.Font, Text2Style.FontColor, Text2Style.HaloColor,
                //    TextAlign.Left, r1);

                g.EndDrawing();
            }
            else
            {
                gx.DrawImageTransparent(this.Image, new Rectangle(this.Location.X,
                    this.Location.Y, this.Size.Height, this.Size.Height));

                g.BeginDrawing();

                Rectangle r1 = this.Bound;
                r1.Height = r1.Height / 2;
                r1.Offset(this.Size.Height, 0);
                r1.Width = r1.Width - this.Size.Height;

                byte[] chars = UICanvas.Encoding.GetBytes(Text1);
                chars = CropText(chars, r1.Width, Text1Style);

                g._DrawTextC(chars, Text1Style.Font,
                    r1.X + 2 + (Text1Style.Font.TextWidth(chars) / 2),
                    r1.Y + (r1.Height / 2),
                    Text1Style.FontColor, Text1Style.HaloColor);

                //g.DrawText(Text1, UICanvas.Encoding,
                //    Text1Style.Font, Text1Style.FontColor, Text1Style.HaloColor,
                //    TextAlign.Left, r1);

                r1.Offset(0, r1.Height);

                chars = UICanvas.Encoding.GetBytes(Text2);
                chars = CropText(chars, r1.Width, Text2Style);

                g._DrawTextC(chars, Text2Style.Font,
                    r1.X + 2 + (Text2Style.Font.TextWidth(chars) / 2),
                    r1.Y + (r1.Height / 2),
                    Text2Style.FontColor, Text2Style.HaloColor);

                //g.DrawText(Text2, UICanvas.Encoding,
                //    Text2Style.Font, Text2Style.FontColor, Text2Style.HaloColor,
                //    TextAlign.Left, r1);

                g.EndDrawing();
            }
        }

        private static byte[] CropText(byte[] chars, int w, CompoundFont FontStyle)
        {
            if (chars == null || chars.Length == 1)
                return chars;

            int width = FontStyle.Font.TextWidth(chars);

            while (width >= (w - 3))
            {
                chars = chars.Clone(1, chars.Length - 1);
                width = FontStyle.Font.TextWidth(chars);
            }

            return chars;
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}
