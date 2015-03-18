using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;

namespace Baro.CoreLibrary.Extensions
{
    public struct BlendFunction
    {
        public byte BlendOp;
        public byte BlendFlags;
        public byte SourceConstantAlpha;
        public byte AlphaFormat;
    }

    public enum BlendOperation : byte
    {
        AC_SRC_OVER = 0x00
    }

    public enum BlendFlags : byte
    {
        Zero = 0x00
    }

    public enum SourceConstantAlpha : byte
    {
        Transparent = 0x00,
        Opaque = 0xFF
    }

    public enum AlphaFormat : byte
    {
        AC_SRC_ALPHA = 0x01
    }

    public enum TernaryRasterOperations : uint
    {
        /// <summary>dest = source</summary>
        SRCCOPY = 0x00CC0020,
        /// <summary>dest = source OR dest</summary>
        SRCPAINT = 0x00EE0086,
        /// <summary>dest = source AND dest</summary>
        SRCAND = 0x008800C6,
        /// <summary>dest = source XOR dest</summary>
        SRCINVERT = 0x00660046,
        /// <summary>dest = source AND (NOT dest)</summary>
        SRCERASE = 0x00440328,
        /// <summary>dest = (NOT source)</summary>
        NOTSRCCOPY = 0x00330008,
        /// <summary>dest = (NOT src) AND (NOT dest)</summary>
        NOTSRCERASE = 0x001100A6,
        /// <summary>dest = (source AND pattern)</summary>
        MERGECOPY = 0x00C000CA,
        /// <summary>dest = (NOT source) OR dest</summary>
        MERGEPAINT = 0x00BB0226,
        /// <summary>dest = pattern</summary>
        PATCOPY = 0x00F00021,
        /// <summary>dest = DPSnoo</summary>
        PATPAINT = 0x00FB0A09,
        /// <summary>dest = pattern XOR dest</summary>
        PATINVERT = 0x005A0049,
        /// <summary>dest = (NOT dest)</summary>
        DSTINVERT = 0x00550009,
        /// <summary>dest = BLACK</summary>
        BLACKNESS = 0x00000042,
        /// <summary>dest = WHITE</summary>
        WHITENESS = 0x00FF0062
    }

    public struct TRIVERTEX
    {
        public int x;
        public int y;
        public ushort Red;
        public ushort Green;
        public ushort Blue;
        public ushort Alpha;

        public TRIVERTEX(int x, int y, Color color)
            : this(x, y, color.R, color.G, color.B, color.A)
        {
        }

        public TRIVERTEX(
            int x, int y,
            ushort red, ushort green, ushort blue,
            ushort alpha)
        {
            this.x = x;
            this.y = y;
            this.Red = (ushort)(red << 8);
            this.Green = (ushort)(green << 8);
            this.Blue = (ushort)(blue << 8);
            this.Alpha = (ushort)(alpha << 8);
        }
    }

    public struct GRADIENT_RECT
    {
        public uint UpperLeft;
        public uint LowerRight;

        public GRADIENT_RECT(uint ul, uint lr)
        {
            this.UpperLeft = ul;
            this.LowerRight = lr;
        }
    }

    public sealed class Win32Helper
    {
        [DllImport("coredll.dll")]
        extern public static Int32 AlphaBlend(IntPtr hdcDest,
            Int32 xDest, Int32 yDest, Int32 cxDest, Int32 cyDest, IntPtr hdcSrc,
            Int32 xSrc, Int32 ySrc, Int32 cxSrc, Int32 cySrc, BlendFunction blendFunction);

        [DllImport("coredll.dll")]
        public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth,
           int nHeight, IntPtr hObjSource, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

        [DllImport("coredll.dll", SetLastError = true, EntryPoint = "GradientFill")]
        public extern static bool GradientFill(
            IntPtr hdc,
            TRIVERTEX[] pVertex,
            uint dwNumVertex,
            GRADIENT_RECT[] pMesh,
            uint dwNumMesh,
            uint dwMode);

        public const int GRADIENT_FILL_RECT_H = 0x00000000;
        public const int GRADIENT_FILL_RECT_V = 0x00000001;

    }

    public static class ImagingFactory
    {
        private static IImagingFactory factory;

        public static IImagingFactory GetImaging()
        {
            if (factory == null)
            {
                factory = (IImagingFactory)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("327ABDA8-072B-11D3-9D7B-0000F81EF32E")));
            }

            return factory;
        }
    }

    // Pulled from gdipluspixelformats.h in the Windows Mobile 5.0 Pocket PC SDK
    public enum PixelFormatID : int
    {
        PixelFormatIndexed = 0x00010000, // Indexes into a palette
        PixelFormatGDI = 0x00020000, // Is a GDI-supported format
        PixelFormatAlpha = 0x00040000, // Has an alpha component
        PixelFormatPAlpha = 0x00080000, // Pre-multiplied alpha
        PixelFormatExtended = 0x00100000, // Extended color 16 bits/channel
        PixelFormatCanonical = 0x00200000,

        PixelFormatUndefined = 0,
        PixelFormatDontCare = 0,

        PixelFormat1bppIndexed = (1 | (1 << 8) | PixelFormatIndexed | PixelFormatGDI),
        PixelFormat4bppIndexed = (2 | (4 << 8) | PixelFormatIndexed | PixelFormatGDI),
        PixelFormat8bppIndexed = (3 | (8 << 8) | PixelFormatIndexed | PixelFormatGDI),
        PixelFormat16bppRGB555 = (5 | (16 << 8) | PixelFormatGDI),
        PixelFormat16bppRGB565 = (6 | (16 << 8) | PixelFormatGDI),
        PixelFormat16bppARGB1555 = (7 | (16 << 8) | PixelFormatAlpha | PixelFormatGDI),
        PixelFormat24bppRGB = (8 | (24 << 8) | PixelFormatGDI),
        PixelFormat32bppRGB = (9 | (32 << 8) | PixelFormatGDI),
        PixelFormat32bppARGB = (10 | (32 << 8) | PixelFormatAlpha | PixelFormatGDI | PixelFormatCanonical),
        PixelFormat32bppPARGB = (11 | (32 << 8) | PixelFormatAlpha | PixelFormatPAlpha | PixelFormatGDI),
        PixelFormat48bppRGB = (12 | (48 << 8) | PixelFormatExtended),
        PixelFormat64bppARGB = (13 | (64 << 8) | PixelFormatAlpha | PixelFormatCanonical | PixelFormatExtended),
        PixelFormat64bppPARGB = (14 | (64 << 8) | PixelFormatAlpha | PixelFormatPAlpha | PixelFormatExtended),
        PixelFormatMax = 15
    }

    // Pulled from imaging.h in the Windows Mobile 5.0 Pocket PC SDK
    public enum BufferDisposalFlag : int
    {
        BufferDisposalFlagNone,
        BufferDisposalFlagGlobalFree,
        BufferDisposalFlagCoTaskMemFree,
        BufferDisposalFlagUnmapView
    }

    // Pulled from imaging.h in the Windows Mobile 5.0 Pocket PC SDK
    public enum InterpolationHint : int
    {
        InterpolationHintDefault,
        InterpolationHintNearestNeighbor,
        InterpolationHintBilinear,
        InterpolationHintAveraging,
        InterpolationHintBicubic
    }

    // Pulled from gdiplusimaging.h in the Windows Mobile 5.0 Pocket PC SDK
    public struct BitmapData
    {
        public uint Width;
        public uint Height;
        public int Stride;
        public PixelFormatID PixelFormat;
        public IntPtr Scan0;
        public IntPtr Reserved;
    }

    // Pulled from imaging.h in the Windows Mobile 5.0 Pocket PC SDK
    public struct ImageInfo
    {
        public uint GuidPart1;  // I am being lazy here, I don't care at this point about the RawDataFormat GUID
        public uint GuidPart2;  // I am being lazy here, I don't care at this point about the RawDataFormat GUID
        public uint GuidPart3;  // I am being lazy here, I don't care at this point about the RawDataFormat GUID
        public uint GuidPart4;  // I am being lazy here, I don't care at this point about the RawDataFormat GUID
        public PixelFormatID pixelFormat;
        public uint Width;
        public uint Height;
        public uint TileWidth;
        public uint TileHeight;
        public double Xdpi;
        public double Ydpi;
        public uint Flags;
    }

    // Pulled from imaging.h in the Windows Mobile 5.0 Pocket PC SDK
    [ComImport, Guid("327ABDA7-072B-11D3-9D7B-0000F81EF32E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComVisible(true)]
    public interface IImagingFactory
    {
        uint CreateImageFromStream();       // This is a place holder, note the lack of arguments
        uint CreateImageFromFile(string filename, out IImage image);
        // We need the MarshalAs attribute here to keep COM interop from sending the buffer down as a Safe Array.
        uint CreateImageFromBuffer([MarshalAs(UnmanagedType.LPArray)] byte[] buffer, uint size, BufferDisposalFlag disposalFlag, out IImage image);
        uint CreateNewBitmap(uint width, uint height, PixelFormatID pixelFormat, out IBitmapImage bitmap);
        uint CreateBitmapFromImage(IImage image, uint width, uint height, PixelFormatID pixelFormat, InterpolationHint hints, out IBitmapImage bitmap);
        uint CreateBitmapFromBuffer();      // This is a place holder, note the lack of arguments
        uint CreateImageDecoder();          // This is a place holder, note the lack of arguments
        uint CreateImageEncoderToStream();  // This is a place holder, note the lack of arguments
        uint CreateImageEncoderToFile();    // This is a place holder, note the lack of arguments
        uint GetInstalledDecoders();        // This is a place holder, note the lack of arguments
        uint GetInstalledEncoders();        // This is a place holder, note the lack of arguments
        uint InstallImageCodec();           // This is a place holder, note the lack of arguments
        uint UninstallImageCodec();         // This is a place holder, note the lack of arguments
    }

    // Pulled from imaging.h in the Windows Mobile 5.0 Pocket PC SDK
    [ComImport, Guid("327ABDA9-072B-11D3-9D7B-0000F81EF32E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComVisible(true)]
    public interface IImage
    {
        uint GetPhysicalDimension(out Size size);
        uint GetImageInfo(out ImageInfo info);
        uint SetImageFlags(uint flags);
        uint Draw(IntPtr hdc, ref Rectangle dstRect, IntPtr NULL); // "Correct" declaration: uint Draw(IntPtr hdc, ref Rectangle dstRect, ref Rectangle srcRect);
        uint PushIntoSink();    // This is a place holder, note the lack of arguments
        uint GetThumbnail(uint thumbWidth, uint thumbHeight, out IImage thumbImage);
    }

    // Pulled from imaging.h in the Windows Mobile 5.0 Pocket PC SDK
    [ComImport, Guid("327ABDAA-072B-11D3-9D7B-0000F81EF32E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComVisible(true)]
    public interface IBitmapImage
    {
        uint GetSize(out Size size);
        uint GetPixelFormatID(out PixelFormatID pixelFormat);
        uint LockBits(ref Rectangle rect, uint flags, PixelFormatID pixelFormat, out BitmapData lockedBitmapData);
        uint UnlockBits(ref BitmapData lockedBitmapData);
        uint GetPalette();  // This is a place holder, note the lack of arguments
        uint SetPalette();  // This is a place holder, note the lack of arguments
    }

    // The direction to the GradientFill will follow
    public enum FillDirection
    {
        //
        // The fill goes horizontally
        //
        LeftToRight = Win32Helper.GRADIENT_FILL_RECT_H,
        //
        // The fill goes vertically
        //
        TopToBottom = Win32Helper.GRADIENT_FILL_RECT_V
    }

    public static class GraphicsExtensions
    {
        public static bool GradientFill(
            this Graphics gr,
            Rectangle rc,
            Color startColor, Color endColor,
            FillDirection fillDir)
        {

            // Initialize the data to be used in the call to GradientFill.
            TRIVERTEX[] tva = new TRIVERTEX[2];
            tva[0] = new TRIVERTEX(rc.X, rc.Y, startColor);
            tva[1] = new TRIVERTEX(rc.Right, rc.Bottom, endColor);
            GRADIENT_RECT[] gra = new GRADIENT_RECT[] { new GRADIENT_RECT(0, 1) };

            // Get the hDC from the Graphics object.
            IntPtr hdc = gr.GetHdc();

            // PInvoke to GradientFill.
            bool b;

            b = Win32Helper.GradientFill(
                    hdc,
                    tva,
                    (uint)tva.Length,
                    gra,
                    (uint)gra.Length,
                    (uint)fillDir);

            System.Diagnostics.Debug.Assert(b, string.Format(
                "GradientFill failed: {0}",
                System.Runtime.InteropServices.Marshal.GetLastWin32Error()));

            // Release the hDC from the Graphics object.
            gr.ReleaseHdc(hdc);

            return b;
        }

        /// <summary>
        /// Draws an image with transparency. Source must be 32bpp (or 24bpp)
        /// </summary>
        /// <param name="gx">Graphics to drawn on.</param>
        /// <param name="image">Image to draw.</param>
        /// <param name="transparency">Transparency constant</param>
        /// <param name="x">X location</param>
        /// <param name="y">Y location</param>
        public static void DrawAlpha(this Graphics gx, Bitmap image, byte transparency, int x, int y)
        {
            using (Graphics gxSrc = Graphics.FromImage(image))
            {
                IntPtr hdcDst = gx.GetHdc();
                IntPtr hdcSrc = gxSrc.GetHdc();

                BlendFunction blendFunction = new BlendFunction();
                blendFunction.BlendOp = (byte)BlendOperation.AC_SRC_OVER;   // Only supported blend operation
                blendFunction.BlendFlags = (byte)BlendFlags.Zero;           // Documentation says put 0 here
                blendFunction.SourceConstantAlpha = transparency;           // Constant alpha factor
                blendFunction.AlphaFormat = (byte)0;                        // Don't look for per pixel alpha

                Win32Helper.AlphaBlend(hdcDst, x, y, image.Width, image.Height,
                               hdcSrc, 0, 0, image.Width, image.Height, blendFunction);

                gx.ReleaseHdc(hdcDst);                                      // Required cleanup to GetHdc()
                gxSrc.ReleaseHdc(hdcSrc);                                   // Required cleanup to GetHdc()
            }
        }

        public static void DrawImageAlphaChannel(this Graphics gx, IImage image, int x, int y)
        {
            ImageInfo imageInfo = new ImageInfo();
            image.GetImageInfo(out imageInfo);
            Rectangle rc = new Rectangle(x, y, (int)imageInfo.Width + x, (int)imageInfo.Height + y);
            IntPtr hdc = gx.GetHdc();
            image.Draw(hdc, ref rc, IntPtr.Zero);
            gx.ReleaseHdc(hdc);
        }

        public static void DrawImageAlphaChannel(this Graphics gx, IImage image, Rectangle dest)
        {
            Rectangle rc = new Rectangle(dest.X, dest.Y, dest.Width + dest.X, dest.Height + dest.Y);
            IntPtr hdc = gx.GetHdc();
            image.Draw(hdc, ref rc, IntPtr.Zero);
            gx.ReleaseHdc(hdc);
        }

        public static void CopyGraphics(Graphics gxSrc, Graphics gxDest, int width, int height)
        {
            IntPtr destDc = gxDest.GetHdc();
            IntPtr srcDc = gxSrc.GetHdc();
            Win32Helper.BitBlt(destDc, 0, 0, width, height, srcDc, 0, 0, TernaryRasterOperations.SRCCOPY);
            gxSrc.ReleaseHdc(srcDc);
            gxDest.ReleaseHdc(destDc);
        }

        private static Color GetTransparentColor(Image image)
        {
            return ((Bitmap)image).GetPixel(image.Width - 1, image.Height - 1);
        }

        /// <summary>
        /// Draws the image with transparency
        /// </summary>
        /// <param name="gx">Destination graphics</param>
        /// <param name="image">The image to draw</param>
        /// <param name="destRect">Desctination rectangle</param>
        public static void DrawImageTransparent(this Graphics gx, Image image, Rectangle destRect)
        {
            Color transpColor = GetTransparentColor(image);
            DrawImageTransparent(gx, image, destRect, transpColor);
        }

        public static void DrawImageTransparentUnstreched(this Graphics gx, Image image, Point pos)
        {
            Color transpColor = GetTransparentColor(image);

            ImageAttributes imageAttr = new ImageAttributes();
            imageAttr.SetColorKey(transpColor, transpColor);

            gx.DrawImage(image, new Rectangle(pos.X - (image.Width / 2), pos.Y - (image.Height / 2), image.Width, image.Height),
                0, 0, image.Width, image.Height,
                GraphicsUnit.Pixel, imageAttr);

            imageAttr.Dispose();
        }

        /// <summary>
        /// Draws the image with transparency
        /// </summary>
        /// <param name="gx">Destination graphics</param>
        /// <param name="image">The image to draw</param>
        /// <param name="destRect">Desctination rectangle</param>
        public static void DrawImageTransparent(this Graphics gx, Image image, Rectangle destRect, Color transpColor)
        {
            ImageAttributes imageAttr = new ImageAttributes();
            imageAttr.SetColorKey(transpColor, transpColor);
            gx.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttr);
            imageAttr.Dispose();
        }

        public static void DrawRoundedRectangle(this Graphics g, Pen p, Color backColor, Rectangle rc, Size size)
        {
            Point[] points = new Point[8];

            //prepare points for poligon
            points[0].X = rc.Left + size.Width / 2;
            points[0].Y = rc.Top + 1;
            points[1].X = rc.Right - size.Width / 2;
            points[1].Y = rc.Top + 1;

            points[2].X = rc.Right;
            points[2].Y = rc.Top + size.Height / 2;
            points[3].X = rc.Right;
            points[3].Y = rc.Bottom - size.Height / 2;

            points[4].X = rc.Right - size.Width / 2;
            points[4].Y = rc.Bottom;
            points[5].X = rc.Left + size.Width / 2;
            points[5].Y = rc.Bottom;

            points[6].X = rc.Left + 1;
            points[6].Y = rc.Bottom - size.Height / 2;
            points[7].X = rc.Left + 1;
            points[7].Y = rc.Top + size.Height / 2;

            //prepare brush for background
            Brush fillBrush = new SolidBrush(backColor);

            //draw side lines and circles in the corners
            g.DrawLine(p, rc.Left + size.Width / 2, rc.Top,
             rc.Right - size.Width / 2, rc.Top);

            g.FillEllipse(fillBrush, rc.Right - size.Width, rc.Top,
             size.Width, size.Height);

            g.DrawEllipse(p, rc.Right - size.Width, rc.Top,
            size.Width, size.Height);


            g.DrawLine(p, rc.Right, rc.Top + size.Height / 2,
             rc.Right, rc.Bottom - size.Height / 2);

            g.FillEllipse(fillBrush, rc.Right - size.Width, rc.Bottom - size.Height,
            size.Width, size.Height);

            g.DrawEllipse(p, rc.Right - size.Width, rc.Bottom - size.Height,
             size.Width, size.Height);



            g.DrawLine(p, rc.Right - size.Width / 2, rc.Bottom,
             rc.Left + size.Width / 2, rc.Bottom);

            g.FillEllipse(fillBrush, rc.Left, rc.Bottom - size.Height,
            size.Width, size.Height);

            g.DrawEllipse(p, rc.Left, rc.Bottom - size.Height,
             size.Width, size.Height);



            g.DrawLine(p, rc.Left, rc.Bottom - size.Height / 2,
             rc.Left, rc.Top + size.Height / 2);
            g.FillEllipse(fillBrush, rc.Left, rc.Top,
             size.Width, size.Height);

            g.DrawEllipse(p, rc.Left, rc.Top,
            size.Width, size.Height);

            //fill the background and remove the internal arcs  
            g.FillPolygon(fillBrush, points);
            //dispose the brush
            fillBrush.Dispose();
        }

    }
}
