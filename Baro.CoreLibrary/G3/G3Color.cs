using System;
using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.G3
{
    public struct G3Color
    {
        public static readonly G3Color RED = new G3Color(255, 0, 0);
        public static readonly G3Color GREEN = new G3Color(0, 255, 0);
        public static readonly G3Color BLUE = new G3Color(0, 0, 255);
        public static readonly G3Color GRAY = new G3Color(128, 128, 128);
        public static readonly G3Color BLACK = new G3Color(0, 0, 0);
        public static readonly G3Color WHITE = new G3Color(255, 255, 255);

        private byte _R, _G, _B;
        private ushort _rgb565;

        public System.Drawing.Color WindowsColor { get { return System.Drawing.Color.FromArgb(_R, _G, _B); } }

        public byte ToYIQ()
        {
            return (byte)((_R * 306 + _G * 601 + _B * 116) / 1024);
        }

        public G3Color Darker(byte level)
        {
            // HSLColor hsl = new HSLColor(this.R, this.G, this.B);
            
            int R = (_R > level) ? _R - level : 0;
            int G = (_G > level) ? _G - level : 0;
            int B = (_B > level) ? _B - level : 0;
            return new G3Color(R, G, B);
        }

        public static G3Color FromRGB(byte r, byte g, byte b)
        {
            return new G3Color(r, g, b);
        }

        public G3Color(System.Drawing.Color windowsColor)
            : this(windowsColor.R, windowsColor.G, windowsColor.B)
        {
        }

        public G3Color(int R, int G, int B)
        {
            _R = (byte)R;
            _G = (byte)G;
            _B = (byte)B;
            _rgb565 = 0;
            Update();
        }

        public G3Color(byte R, byte G, byte B)
        {
            _R = R;
            _G = G;
            _B = B;
            _rgb565 = 0;
            Update();
        }

        public static G3Color From565Color(ushort c)
        {
            byte R = (byte)((c & 0xF800) >> 8);
            byte G = (byte)((c & 0x7E0) >> 3);
            byte B = (byte)((c & 0x1F) << 3);
            return new G3Color(R, G, B);
        }

        public static G3Color FromColorString(string colorStr)
        {
            if (colorStr[0] == '#')
            {
                return G3Color.FromHTMLColor(colorStr);
            }
            else
            {
                ushort c = ushort.Parse(colorStr);
                byte R = (byte)((c & 0xF800) >> 8);
                byte G = (byte)((c & 0x7E0) >> 3);
                byte B = (byte)((c & 0x1F) << 3);
                return new G3Color(R, G, B);
            }
        }

        public string ToHTMLColor()
        {
            return "#" + this._R.ToString("X2") + this._G.ToString("X2") + this._B.ToString("X2");
        }

        public static G3Color FromHTMLColor(string colorStr)
        {
            if (colorStr.Length != 7 || colorStr[0] != '#')
                throw new ArgumentException("verilen renk değeri hex renk kodu değil");

            byte R = Convert.ToByte(colorStr.Substring(1, 2), 16);
            byte G = Convert.ToByte(colorStr.Substring(3, 2), 16);
            byte B = Convert.ToByte(colorStr.Substring(5, 2), 16);

            return new G3Color(R, G, B);
        }

        public ushort RGB565
        {
            get { return _rgb565; }
        }

        private void Update()
        {
            _rgb565 = (ushort)(((this._R >> 3) << 11) | ((this._G >> 2) << 5) | (this._B >> 3));
        }

        public byte B
        {
            get { return _B; }
            set
            {
                _B = value;
                Update();
            }
        }

        public byte G
        {
            get { return _G; }
            set
            {
                _G = value;
                Update();
            }
        }

        public byte R
        {
            get { return _R; }
            set
            {
                _R = value;
                Update();
            }
        }

        private static int Lerp(byte start, byte end, byte amount)
        {
            return start + (((end - start) << 8) / 255 * amount) >> 8;
        }

        public G3Color Lerp(G3Color to, byte amount)
        {
            return new G3Color(Lerp(this.R, to.R, amount),
                               Lerp(this.G, to.G, amount),
                               Lerp(this.B, to.B, amount));
        }

        public static G3Color Lerp(G3Color from, G3Color to, byte amount)
        {
            return new G3Color(Lerp(from.R, to.R, amount),
                               Lerp(from.G, to.G, amount),
                               Lerp(from.B, to.B, amount));
        }
    }
}
