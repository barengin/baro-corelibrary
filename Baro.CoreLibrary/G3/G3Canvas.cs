using System;
using System.Collections.Generic;
using System.Drawing;
using Baro.CoreLibrary.Text;
using Baro.CoreLibrary.Extensions;

namespace Baro.CoreLibrary.G3
{
    public enum TextAlign
    {
        Center, Left, Right, UpperLeft, BottomLeft, TopCenter, BottomCenter, UpperRight, BottomRight
    }

    public unsafe sealed class G3Canvas : IDisposable
    {
        private bool disposed = false;
        private readonly int m_ClipWidth, m_ClipHeight;
        private readonly int SurfaceWidth, SurfaceHeight;

        public G3Surface Surface { get; private set; }

        #region ctor
        public G3Canvas(int width, int height, G3Surface s)
        {
            Surface = s;

            m_ClipWidth = width - 1;
            m_ClipHeight = height - 1;

            SurfaceWidth = width;
            SurfaceHeight = height;
        }

        public G3Canvas(int width, int height)
        {
            Surface = new G3Surface(width, height);

            m_ClipWidth = width - 1;
            m_ClipHeight = height - 1;

            SurfaceWidth = width;
            SurfaceHeight = height;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                Surface.Dispose();
                GC.SuppressFinalize(this);
            }
        }

        ~G3Canvas()
        {
            Dispose();
        }
        #endregion

        #region Draw Text

        private void PutChar0(byte encodedChar, int X1, int Y1, G3Font font,
            G3Color fontColor, G3Color haloColor)
        {
            int charwidth = font.Chars[encodedChar].width;

            if (charwidth == 0)
                return;

            X1 -= charwidth >> 1;
            Y1 -= font.FontHeight >> 1;

            if (X1 < 0 || Y1 < 0 || (X1 + charwidth) >= SurfaceWidth || (Y1 + font.FontHeight) >= SurfaceHeight)
                return;

            ushort pencolor = fontColor.RGB565;
            ushort outlinecolor = haloColor.RGB565;

            ushort* dst = &Surface.DIBImage[Y1 * SurfaceWidth + X1];
            int dstStride = SurfaceWidth - charwidth;

            fixed (byte* b = font.Chars[encodedChar].data)
            {
                byte* d = b;

                for (int y = 0; y < font.FontHeight; y++, dst += dstStride)
                {
                    for (int x = 0; x < charwidth; x++, d++, dst++)
                    {
                        if (*d == G3Font.FONT)
                        {
                            *dst = pencolor;
                            // *dst = 0;
                        }
                        else if (*d == G3Font.HALO)
                        {
                            *dst = outlinecolor;
                            // *dst = 65535;
                        }
                    }
                }
            }
        }

        private void PutChar(byte char1254, int angle, int PosX, int PosY, G3Font font,
            G3Color fontColor, G3Color haloColor)
        {
            if (angle == 0)
            {
                PutChar0(char1254, PosX, PosY, font, fontColor, haloColor);
                return;
            }

            // Font'un iç rengi SİYAH
            byte fontR = fontColor.R;
            byte fontG = fontColor.G;
            byte fontB = fontColor.B;

            // Font'un dış halo rengi BEYAZ
            byte haloR = haloColor.R;
            byte haloG = haloColor.G;
            byte haloB = haloColor.B;

            // get source image size
            int sourcewidth = font.Chars[char1254].width;
            int sourceheight = font.FontHeight;

            int angleCos = SinCos11.cos[angle];
            int angleSin = SinCos11.sin[angle];

            int halfWidth = sourcewidth / 2;
            int halfHeight = sourceheight / 2;

            int halfNewWidth, halfNewHeight;
            int newWidth, newHeight;

            // rotate corners
            int cx1 = (halfWidth * angleCos);
            int cy1 = (halfWidth * angleSin);

            int cx2 = (halfWidth * angleCos - halfHeight * angleSin);
            int cy2 = (halfWidth * angleSin + halfHeight * angleCos);

            int cx3 = (-halfHeight * angleSin);
            int cy3 = (halfHeight * angleCos);

            halfNewWidth = Math.Max(Math.Max(cx1, cx2), Math.Max(cx3, 0)) - Math.Min(Math.Min(cx1, cx2), Math.Min(cx3, 0));
            halfNewHeight = Math.Max(Math.Max(cy1, cy2), Math.Max(cy3, 0)) - Math.Min(Math.Min(cy1, cy2), Math.Min(cy3, 0));

            //halfNewWidth >>= 11;
            //halfNewHeight >>= 11;

            //halfNewHeight++;
            //halfNewWidth++;

            //newHeight = halfNewHeight * 2;
            //newWidth = halfNewWidth * 2;

            if ((halfNewWidth & 0x7ff) > 1024)
            {
                halfNewWidth >>= 11;
                newWidth = (halfNewWidth * 2) + 1;
            }
            else
            {
                halfNewWidth >>= 11;
                newWidth = (halfNewWidth * 2);
            }

            if ((halfNewHeight & 0x7ff) > 1024)
            {
                halfNewHeight >>= 11;
                newHeight = (halfNewHeight * 2) + 1;
            }
            else
            {
                halfNewHeight >>= 11;
                newHeight = (halfNewHeight * 2);
            }

            PosX -= halfNewWidth;
            PosY -= halfNewHeight;

            // Clipping
            if (PosX < 0 || PosY < 0)
                return;

            if (PosX + newWidth >= SurfaceWidth || PosY + newHeight >= SurfaceHeight)
                return;

            // Alt satıra geçiş için
            int dstOffset = SurfaceWidth - newWidth;

            // do the job
            byte[] src = font.Chars[char1254].data;
            ushort* dst = &Surface.DIBImage[(SurfaceWidth * PosY) + PosX];

            // ------------------------------------
            // rotate using  bilinear interpolation
            // ------------------------------------

            int cx, cy;
            int ox, oy;
            int dx1, dy1, dx2, dy2;
            int ox1, oy1, ox2, oy2;
            int ymax = sourceheight - 1;
            int xmax = sourcewidth - 1;

            // Bu satırlar eğer fontlar için renk desteği istersek bu şekilde olacak.

            //byte fontR = (byte)((pencolor & 0xF800) >> 8);
            //byte fontG = (byte)((pencolor & 0x7E0) >> 3);
            //byte fontB = (byte)((pencolor & 0x1F) << 3);

            //byte haloR = (byte)((outlinecolor & 0xF800) >> 8);
            //byte haloG = (byte)((outlinecolor & 0x7E0) >> 3);
            //byte haloB = (byte)((outlinecolor & 0x1F) << 3);

            // RGB
            cy = -halfNewHeight;
            for (int y = 0; y < newHeight; y++)
            {
                cx = -halfNewWidth;
                for (int x = 0; x < newWidth; x++)
                {
                    ox = ((angleCos * cx + angleSin * cy) + (halfWidth << 11));
                    oy = ((-angleSin * cx + angleCos * cy) + (halfHeight << 11));

                    // top-left coordinate
                    ox1 = (ox >> 11);
                    oy1 = (oy >> 11);

                    if ((ox1 < 0) || (oy1 < 0) || (ox1 >= sourcewidth) || (oy1 >= sourceheight))
                    {
                        //dst[2] = fillR;
                        //dst[1] = fillG;
                        //dst[0] = fillB;
                    }
                    else
                    {
                        // bottom-right coordinate
                        ox2 = (ox1 == xmax) ? ox1 : ox1 + 1;
                        oy2 = (oy1 == ymax) ? oy1 : oy1 + 1;

                        if ((dx1 = ox - (ox1 << 11)) < 0)
                            dx1 = 0;

                        dx2 = 2048 - dx1;

                        if ((dy1 = oy - (oy1 << 11)) < 0)
                            dy1 = 0;

                        dy2 = 2048 - dy1;


                        /////////////////////////////
                        byte p1R, p1G, p1B;
                        switch (src[oy1 * sourcewidth + ox1])
                        {
                            case G3Font.TRANSPARENT:
                                p1R = (byte)((*dst & 0xF800) >> 8);
                                p1G = (byte)((*dst & 0x7E0) >> 3);
                                p1B = (byte)((*dst & 0x1F) << 3);
                                break;

                            case G3Font.HALO:
                                p1R = haloR;
                                p1G = haloG;
                                p1B = haloB;
                                break;

                            default:
                                p1R = fontR;
                                p1G = fontG;
                                p1B = fontB;
                                break;
                        }

                        /////////////////////////////
                        byte p2R, p2G, p2B;
                        switch (src[oy1 * sourcewidth + ox2])
                        {
                            case G3Font.TRANSPARENT:
                                p2R = (byte)((*dst & 0xF800) >> 8);
                                p2G = (byte)((*dst & 0x7E0) >> 3);
                                p2B = (byte)((*dst & 0x1F) << 3);
                                break;

                            case G3Font.HALO:
                                p2R = haloR;
                                p2G = haloG;
                                p2B = haloB;
                                break;

                            default:
                                p2R = fontR;
                                p2G = fontG;
                                p2B = fontB;
                                break;
                        }

                        /////////////////////////////
                        byte p3R, p3G, p3B;
                        switch (src[oy2 * sourcewidth + ox1])
                        {
                            case G3Font.TRANSPARENT:
                                p3R = (byte)((*dst & 0xF800) >> 8);
                                p3G = (byte)((*dst & 0x7E0) >> 3);
                                p3B = (byte)((*dst & 0x1F) << 3);
                                break;

                            case G3Font.HALO:
                                p3R = haloR;
                                p3G = haloG;
                                p3B = haloB;
                                break;

                            default:
                                p3R = fontR;
                                p3G = fontG;
                                p3B = fontB;
                                break;
                        }

                        /////////////////////////////
                        byte p4R, p4G, p4B;
                        switch (src[oy2 * sourcewidth + ox2])
                        {
                            case G3Font.TRANSPARENT:
                                p4R = (byte)((*dst & 0xF800) >> 8);
                                p4G = (byte)((*dst & 0x7E0) >> 3);
                                p4B = (byte)((*dst & 0x1F) << 3);
                                break;

                            case G3Font.HALO:
                                p4R = haloR;
                                p4G = haloG;
                                p4B = haloB;
                                break;

                            default:
                                p4R = fontR;
                                p4G = fontG;
                                p4B = fontB;
                                break;
                        }

                        // interpolate using 4 points
                        // red
                        int red = ((dy2 * ((dx2 * p1R + dx1 * p2R) >> 11) + dy1 * ((dx2 * p3R + dx1 * p4R) >> 11)) >> 11);

                        // green
                        int green = ((dy2 * ((dx2 * p1G + dx1 * p2G) >> 11) + dy1 * ((dx2 * p3G + dx1 * p4G) >> 11)) >> 11);

                        // blue
                        int blue = ((dy2 * ((dx2 * p1B + dx1 * p2B) >> 11) + dy1 * ((dx2 * p3B + dx1 * p4B) >> 11)) >> 11);

                        ////////////////////////////////////////////
                        *dst = (ushort)(((red >> 3) << 11) | ((green >> 2) << 5) | (blue >> 3));
                        ////////////////////////////////////////////
                    }
                    cx++;
                    dst++;
                }
                cy++;
                dst += dstOffset;
            }
        }

        public void _DrawTextC(byte[] encodedByteCharArray, G3Font font, int X1, int Y1,
            G3Color fontColor, G3Color haloColor)
        {
            if (encodedByteCharArray == null || font == null)
                return;

            // half width
            float offset = X1 - (font.TextWidth(encodedByteCharArray) / 2f);

            for (int k = 0; k < encodedByteCharArray.Length; k++)
            {
                byte byteChar = encodedByteCharArray[k];

                if (k == 0)
                {
                    offset += (font.Chars[byteChar].width / 2f);
                }
                else
                {
                    offset += (font.Chars[encodedByteCharArray[k - 1]].width / 2f);
                    offset += ((font.Chars[byteChar].width + font.SpaceBetweenChars) / 2f);
                }

                if (byteChar != 32)
                    PutChar0(byteChar, (int)offset, Y1, font, fontColor, haloColor);
            }
        }

        public void DrawText(string text, Encoding encoding, G3Font font, G3Color fontColor, G3Color haloColor,
            TextAlign align, Rectangle r)
        {
            if (text == null || string.IsNullOrEmpty(text) || font == null)
                return;

            byte[] chars = encoding.GetBytes(text);
            IList<byte[]> lines = ClipText(chars, r, font);

            switch (align)
            {
                case TextAlign.Center:
                    DrawTextAlignCenter(lines, r, font, fontColor, haloColor);
                    break;

                case TextAlign.Left:
                    DrawTextAlignLeft(lines, r, font, fontColor, haloColor);
                    break;

                case TextAlign.Right:
                    DrawTextAlignRight(lines, r, font, fontColor, haloColor);
                    break;

                case TextAlign.UpperLeft:
                    throw new NotImplementedException();

                case TextAlign.BottomLeft:
                    throw new NotImplementedException();

                case TextAlign.TopCenter:
                    throw new NotImplementedException();

                case TextAlign.BottomCenter:
                    DrawTextAlignBottomCenter(lines, r, font, fontColor, haloColor);
                    break;

                case TextAlign.UpperRight:
                    throw new NotImplementedException();

                case TextAlign.BottomRight:
                    throw new NotImplementedException();
            }
        }

        private void DrawTextAlignRight(IList<byte[]> lines, Rectangle r,
            G3Font font, G3Color fontColor, G3Color haloColor)
        {
            int x = r.X + r.Width - 1;
            int y = r.Y + (r.Height / 2);

            y = y - ((font.FontHeight * lines.Count) / 2);
            y = y + (font.FontHeight / 2);

            foreach (var l in lines)
            {
                int w = font.TextWidth(l) / 2;

                if (r.Contains(x, y - (font.FontHeight / 2)) && r.Contains(x, y + (font.FontHeight / 2)))
                    _DrawTextC(l, font, x - w - 1, y, fontColor, haloColor);

                y = y + font.FontHeight;
            }
        }

        private void DrawTextAlignLeft(IList<byte[]> lines, Rectangle r,
            G3Font font, G3Color fontColor, G3Color haloColor)
        {
            int x = r.X;
            int y = r.Y + (r.Height / 2);

            y = y - ((font.FontHeight * lines.Count) / 2);
            y = y + (font.FontHeight / 2);

            foreach (var l in lines)
            {
                int w = font.TextWidth(l) / 2;

                if (r.Contains(x, y - (font.FontHeight / 2)) && r.Contains(x, y + (font.FontHeight / 2)))
                    _DrawTextC(l, font, x + w, y, fontColor, haloColor);

                y = y + font.FontHeight;
            }
        }

        private void DrawTextAlignBottomCenter(IList<byte[]> lines, Rectangle r,
            G3Font font, G3Color fontColor, G3Color haloColor)
        {
            int x = r.X + (r.Width / 2); // Orta nokta (X)
            int y = r.Y + r.Height;      //(Y)

            // y = y - ((font.FontHeight * lines.Count) / 2);
            // y = y - (font.FontHeight / 2);

            y = y - (font.FontHeight * lines.Count);

            foreach (var l in lines)
            {
                if (r.Contains(x, y - (font.FontHeight / 2)) && r.Contains(x, y + (font.FontHeight / 2)))
                    _DrawTextC(l, font, x, y, fontColor, haloColor);

                y = y + font.FontHeight;
            }
        }

        private void DrawTextAlignCenter(IList<byte[]> lines, Rectangle r,
            G3Font font, G3Color fontColor, G3Color haloColor)
        {
            int x = r.X + (r.Width / 2);
            int y = r.Y + (r.Height / 2);

            y = y - ((font.FontHeight * lines.Count) / 2);
            y = y + (font.FontHeight / 2);

            foreach (var l in lines)
            {
                if (r.Contains(x, y - (font.FontHeight / 2)) && r.Contains(x, y + (font.FontHeight / 2)))
                    _DrawTextC(l, font, x, y, fontColor, haloColor);

                y = y + font.FontHeight;
            }
        }

        private IList<byte[]> ClipText(byte[] encodedByteCharArray, Rectangle r, G3Font font)
        {
            IList<byte[]> list = new List<byte[]>();
            int x = r.Left, y = r.Top;
            int Start = 0, End = -1, lastSpace = -1;
            int i = 0, totalWidth = 0;

            while (i < encodedByteCharArray.Length)
            {
                // en son rastlanılan SPACE veya ENTER!!!
                if (encodedByteCharArray[i] == 32 || encodedByteCharArray[i] == 10)
                {
                    lastSpace = i;
                }

                totalWidth += font.Chars[encodedByteCharArray[i]].width;

                if ((totalWidth > r.Width) || (encodedByteCharArray[i] == 10))
                {
                    if (lastSpace != -1)
                    {
                        End = lastSpace - 1;

                        list.Add(encodedByteCharArray.Clone(Start, End - Start + 1));

                        // Reset
                        lastSpace = -1;
                        totalWidth = 0;
                        Start = End + 2;
                        i = End + 1;
                    }
                    else
                    {
                        End = i - 1;

                        list.Add(encodedByteCharArray.Clone(Start, End - Start + 1));

                        // Reset
                        lastSpace = -1;
                        totalWidth = 0;
                        Start = End + 1;
                        i = End;
                    }
                }

                i++;
            }

            if (Start < encodedByteCharArray.Length)
            {
                list.Add(encodedByteCharArray.Clone(Start, encodedByteCharArray.Length - Start));
            }

            return list;
        }

        #endregion

        #region Draw Pixel
        /// <summary>
        /// Belirtilen koordinata bir pixel koyar. Clipping var.
        /// </summary>
        /// <param name="X1"></param>
        /// <param name="Y1"></param>
        /// <param name="color"></param>
        public void DrawPixel(int X1, int Y1, G3Color color)
        {
            Surface.DrawPixel(X1, Y1, color);
        }

        #endregion

        #region Thin Line
        private bool ClipLine(ref int X1, ref int Y1, ref int X2, ref int Y2)
        {
            int V;

            int C1 = ((X1 < 0) ? 1 : 0) | ((X1 > m_ClipWidth) ? 2 : 0) | ((Y1 < 0) ? 4 : 0) | ((Y1 > m_ClipHeight) ? 8 : 0);
            int C2 = ((X2 < 0) ? 1 : 0) | ((X2 > m_ClipWidth) ? 2 : 0) | ((Y2 < 0) ? 4 : 0) | ((Y2 > m_ClipHeight) ? 8 : 0);

            if ((C1 & C2) == 0 && (C1 | C2) != 0)
            {
                if ((C1 & 12) != 0)
                {
                    if (C1 < 8)
                    {
                        V = 0;
                    }
                    else
                    {
                        V = m_ClipHeight;
                    }
                    X1 += (V - Y1) * (X2 - X1) / (Y2 - Y1);
                    Y1 = V;
                    C1 = ((X1 < 0) ? 1 : 0) | ((X1 > m_ClipWidth) ? 2 : 0);
                }

                if ((C2 & 12) != 0)
                {
                    if (C2 < 8)
                    {
                        V = 0;
                    }
                    else
                    {
                        V = m_ClipHeight;
                    }
                    X2 += (V - Y2) * (X2 - X1) / (Y2 - Y1);
                    Y2 = V;
                    C2 = ((X2 < 0) ? 1 : 0) | ((X2 > m_ClipWidth) ? 2 : 0);
                }

                if ((C1 & C2) == 0 && (C1 | C2) != 0)
                {
                    if (C1 != 0)
                    {
                        if (C1 == 1)
                        {
                            V = 0;
                        }
                        else
                        {
                            V = m_ClipWidth;
                        }
                        Y1 += (V - X1) * (Y2 - Y1) / (X2 - X1);
                        X1 = V;
                        C1 = 0;
                    }

                    if (C2 != 0)
                    {
                        if (C2 == 1)
                        {
                            V = 0;
                        }
                        else
                        {
                            V = m_ClipWidth;
                        };
                        Y2 += (V - X2) * (Y2 - Y1) / (X2 - X1);
                        X2 = V;
                        C2 = 0;
                    }
                }
            }

            return (C1 | C2) == 0;
        }

        private void DrawLineSegment(int X1, int Y1, int X2, int Y2, G3Color color)
        {
            ushort* DIB = &Surface.DIBImage[Y1 * SurfaceWidth + X1];

            X1 = X2 - X1; // DeltaX
            Y1 = Y2 - Y1; // DeltaY	

            int Sx, Sy;
            if (X1 < 0)  // deltax
            {
                X1 = -X1;
                Sx = -1;
            }
            else
            {
                Sx = 1;
            }

            if (Y1 < 0)  // deltay
            {
                Y1 = -Y1;
                Sy = -SurfaceWidth;
            }
            else
            {
                Sy = SurfaceWidth;
            }

            ushort pencolor = color.RGB565;

            if (X1 > Y1)
            {

                int Delta = X1 >> 1;
                X2 = 0;
                while (X2 < X1)
                {
                    *DIB = pencolor;
                    DIB += Sx;

                    Delta += Y1;
                    if (Delta > X1)
                    {
                        DIB += Sy;
                        Delta -= X1;
                    }
                    X2++;
                }
            }
            else
            {
                int Delta = Y1 >> 1;
                X2 = 0;
                while (X2 < Y1)
                {
                    *DIB = pencolor;
                    DIB += Sy;

                    Delta += X1;
                    if (Delta > Y1)
                    {
                        DIB += Sx;
                        Delta -= Y1;
                    }
                    X2++;
                }
            }
        }

        public void DrawLineSegmentSafe(int x1, int y1, int x2, int y2, G3Color inColor)
        {
            if (ClipLine(ref x1, ref y1, ref x2, ref y2))
                DrawLineSegment(x1, y1, x2, y2, inColor);
        }
        #endregion

        /// <summary>
        /// Bu verilen koordinattaki kutuyu karartır.
        /// </summary>
        /// <param name="x">Başlangıç X</param>
        /// <param name="y">Başlangıç Y</param>
        /// <param name="w">Genişlik</param>
        /// <param name="h">Yükseklik</param>
        /// <param name="decline">Ne kadar karartılacağı? (0-30)</param>
        public void DarkBox(int x, int y, int w, int h, int level)
        {
            if (x < 0 || y < 0 || (x + w) > SurfaceWidth || (y + h) > SurfaceHeight)
                return;

            ushort* DIB = &Surface.DIBImage[y * SurfaceWidth + x];
            int R, G, B, myOffs = SurfaceWidth - w;

            for (int k = 0; k < h; k++, DIB += myOffs)
                for (int l = 0; l < w; l++, DIB++)
                {
                    R = (*DIB & 0xF800) >> 11;
                    G = (*DIB & 0x7E0) >> 5;
                    B = (*DIB & 0x1F);

                    // Azalt
                    R = (R > level) ? R - level : 0;
                    G = (G > (level << 1)) ? G - (level << 1) : 0;
                    B = (B > level) ? B - level : 0;

                    *DIB = (ushort)((R << 11) | (G << 5) | B);
                }
        }

        /// <summary>
        /// Verilen koordinattaki kutuyu aydınlatır
        /// </summary>
        /// <param name="x">Başlangıç X</param>
        /// <param name="y">Başlangıç Y</param>
        /// <param name="w">Genişlik</param>
        /// <param name="h">Yükseklik</param>
        /// <param name="level">Rengin ne kadar açılacağı? (0-30)</param>
        public void LightBox(int x, int y, int w, int h, int level)
        {
            if (x < 0 || y < 0 || (x + w) >= SurfaceWidth || (y + h) >= SurfaceHeight)
                return;

            ushort* DIB = &Surface.DIBImage[y * SurfaceWidth + x];
            int R, G, B, stride = SurfaceWidth - w;

            for (int k = 0; k < h; k++, DIB += stride)
                for (int l = 0; l < w; l++, DIB++)
                {
                    R = (*DIB & 0xF800) >> 11;
                    G = (*DIB & 0x7E0) >> 5;
                    B = (*DIB & 0x1F);

                    // Arttır
                    R = (31 - R > level) ? R + level : 31;
                    G = (63 - G > (level << 1)) ? G + (level << 1) : 63;
                    B = (31 - B > level) ? B + level : 31;

                    *DIB = (ushort)((R << 11) | (G << 5) | B);
                }
        }

        private static int Lerp(byte start, byte end, byte amount)
        {
            return start + ((((end - start) << 8) / 255 * amount) >> 8);
        }

        public void Gradient(Rectangle r, G3Color from, G3Color to, byte TopAlpha, byte BottomAlpha)
        {
            Gradient(r.Left, r.Top, r.Width, r.Height, from, to, TopAlpha, BottomAlpha);
        }

        public void Gradient(int x, int y, int w, int h, G3Color from, G3Color to)
        {
            if (x < 0 || y < 0 || (x + w) > SurfaceWidth || (y + h) > SurfaceHeight)
                return;

            ushort* DIB = &Surface.DIBImage[y * SurfaceWidth + x];
            int stride = SurfaceWidth - w;
            ushort Color = from.RGB565;

            int fR = (Color & 0xF800) >> 11;
            int fG = (Color & 0x7E0) >> 5;
            int fB = (Color & 0x1F);

            int tR = (to.RGB565 & 0xF800) >> 11;
            int tG = (to.RGB565 & 0x7E0) >> 5;
            int tB = (to.RGB565 & 0x1F);

            int cdAlfa = (255 << 8) / h, cAlfa = 0;

            for (int k = 0; k < h; k++, DIB += stride)
            {
                int c = cAlfa >> 8;

                int sR = Lerp((byte)fR, (byte)tR, (byte)c);
                int sG = Lerp((byte)fG, (byte)tG, (byte)c);
                int sB = Lerp((byte)fB, (byte)tB, (byte)c);

                for (int l = 0; l < w; l++, DIB++)
                {
                    //dR = (*DIB & 0xF800) >> 11;
                    //dG = (*DIB & 0x7E0) >> 5;
                    //dB = (*DIB & 0x1F);

                    // int rR = (alpha * (sR - dR)) / 256 + dR;
                    // rR = (a * (sR - dR)) / 256 + dR;

                    //rR = (((256 - a) * dR) + (a * sR)) >> 8;
                    //rG = (((256 - a) * dG) + (a * sG)) >> 8;
                    //rB = (((256 - a) * dB) + (a * sB)) >> 8;

                    *DIB = (ushort)((sR << 11) | (sG << 5) | sB);
                }

                cAlfa += cdAlfa;
            }
        }

        public void Gradient(int x, int y, int w, int h, G3Color from, G3Color to, byte TopAlpha, byte BottomAlpha)
        {
            if ((TopAlpha == 255) && (BottomAlpha == 255))
            {
                Gradient(x, y, w, h, from, to);
                return;
            }

            if (x < 0 || y < 0 || (x + w) > SurfaceWidth || (y + h) > SurfaceHeight)
                return;

            ushort* DIB = &Surface.DIBImage[y * SurfaceWidth + x];
            int rR, rG, rB, dR, dG, dB, stride = SurfaceWidth - w;
            ushort Color = from.RGB565;

            int fR = (Color & 0xF800) >> 11;
            int fG = (Color & 0x7E0) >> 5;
            int fB = (Color & 0x1F);

            int tR = (to.RGB565 & 0xF800) >> 11;
            int tG = (to.RGB565 & 0x7E0) >> 5;
            int tB = (to.RGB565 & 0x1F);

            int dAlfa = ((BottomAlpha - TopAlpha) << 8) / h, alfa = TopAlpha << 8;
            int cdAlfa = (255 << 8) / h, cAlfa = 0;

            for (int k = 0; k < h; k++, DIB += stride)
            {
                int a = alfa >> 8;
                int c = cAlfa >> 8;

                int sR = Lerp((byte)fR, (byte)tR, (byte)c);
                int sG = Lerp((byte)fG, (byte)tG, (byte)c);
                int sB = Lerp((byte)fB, (byte)tB, (byte)c);

                for (int l = 0; l < w; l++, DIB++)
                {
                    dR = (*DIB & 0xF800) >> 11;
                    dG = (*DIB & 0x7E0) >> 5;
                    dB = (*DIB & 0x1F);

                    // int rR = (alpha * (sR - dR)) / 256 + dR;
                    // rR = (a * (sR - dR)) / 256 + dR;

                    rR = (((256 - a) * dR) + (a * sR)) >> 8;
                    rG = (((256 - a) * dG) + (a * sG)) >> 8;
                    rB = (((256 - a) * dB) + (a * sB)) >> 8;

                    *DIB = (ushort)((rR << 11) | (rG << 5) | rB);
                }

                alfa += dAlfa;
                cAlfa += cdAlfa;
            }
        }

        public void FillRectangle(Rectangle r, G3Color color)
        {
            FillRectangle(r.Left, r.Top, r.Width, r.Height, color);
        }

        public void FillRectangle(int x, int y, int w, int h, G3Color color)
        {
            if (x < 0 || y < 0 || (x + w) > SurfaceWidth || (y + h) > SurfaceHeight)
                return;

            ushort* DIB = &Surface.DIBImage[y * SurfaceWidth + x];

            int stride = SurfaceWidth - w;
            ushort pencolor = color.RGB565;

            for (int k = 0; k < h; k++, DIB += stride)
                for (int l = 0; l < w; l++, DIB++)
                {
                    *DIB = pencolor;
                }
        }

        public void Rectangle(Rectangle r, G3Color color)
        {
            Rectangle(r.Left, r.Top, r.Width, r.Height, color);
        }

        public void Rectangle(int x, int y, int w, int h, G3Color color)
        {
            DrawVLine(x, y, h + y, color);
            DrawVLine(x + w, y, h + y, color);
            DrawHLine(y, x, w + x, color);
            DrawHLine(y + h, x, w + x, color);
        }

        public void DrawHLine(int y, int minx, int maxx, G3Color color)
        {
            ushort* DIB = &Surface.DIBImage[y * SurfaceWidth + minx];
            maxx -= minx;

            ushort pencolor = color.RGB565;

            while (maxx-- != 0)
            {
                *DIB++ = pencolor;
            }
        }

        public void DrawVLine(int x, int miny, int maxy, G3Color color)
        {
            ushort* DIB = &Surface.DIBImage[miny * SurfaceWidth + x];
            maxy -= miny;

            int str = Surface.Stride / 2;
            ushort pencolor = color.RGB565;

            while (maxy-- != 0)
            {
                *DIB = pencolor;
                DIB += str;
            }
        }

        public void BeginDrawing()
        {
            Surface.BeginDrawing();
        }

        public void EndDrawing()
        {
            Surface.EndDrawing();
        }
    }
}
