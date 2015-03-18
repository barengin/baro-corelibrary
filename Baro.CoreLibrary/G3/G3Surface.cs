using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Baro.CoreLibrary.G3
{
    public unsafe class G3Surface : IDisposable
    {
        private bool disposed;
        private Bitmap m_bitmap;
        private int m_bitmapWidth, m_bitmapHeight;
        private BitmapData m_bitmapData;
        private Rectangle m_bitmapRect;
        private ushort* m_DIBImage;
        private int m_bmpStride;
        private Graphics m_graphics = null;

        public Graphics WindowsGraphics
        {
            get
            {
                if (m_graphics == null)
                    m_graphics = Graphics.FromImage(m_bitmap);

                return m_graphics;
            }
        }

        /// <summary>
        /// Width
        /// </summary>
        public int Width { get { return m_bitmapWidth; } }

        /// <summary>
        /// Height
        /// </summary>
        public int Height { get { return m_bitmapHeight; } }

        /// <summary>
        /// Bitmap stride değeri
        /// </summary>
        public int Stride { get { return m_bmpStride; } }

        /// <summary>
        /// Bitmap'in pointer'i
        /// </summary>
        public ushort* DIBImage
        {
            get
            {
                if (m_bitmapData == null)
                    throw new Exception("Surface init edilmemiş. BeginDrawing()'in çağırıldığından emin olun.");

                return m_DIBImage;
            }
        }

        /// <summary>
        /// Bitmap safe pointer
        /// </summary>
        public G3SafePointer DIBImageSafe
        {
            get { return new G3SafePointer(this.DIBImage, this.Stride * this.Height); }
        }

        /// <summary>
        /// DIB yaratır. Ulaşım için bir pointer verir.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public G3Surface(int width, int height)
        {
            m_bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format16bppRgb565);
            m_bitmapRect = new Rectangle(0, 0, width, height);
            m_bitmapWidth = width;
            m_bitmapHeight = height;
        }

        /// <summary>
        /// Canvas üzerinde çizim yapılmaya başlamadan önce mutlaka çağırılmalıdır. Çizim bitip
        /// ekrana yapıştırmak istediğimiz zaman da <b>EndDrawing()</b> çağırılmalıdır.
        /// </summary>
        /// <seealso cref="EndDrawing()"/>
        public void BeginDrawing()
        {
            m_bitmapData = m_bitmap.LockBits(m_bitmapRect, ImageLockMode.ReadWrite, PixelFormat.Format16bppRgb565);
            m_DIBImage = (ushort*)m_bitmapData.Scan0;
            m_bmpStride = m_bitmapData.Stride;
        }

        /// <summary>
        /// Canvas üzerinde çizim yapılıp bittikten sonra mutlaka çağırılmalı. Çizime başlanmadan önce
        /// <b>BeginDrawing()</b> çağırılmalıdır.
        /// </summary>
        /// <seealso cref="BeginDrawing()"/>
        public void EndDrawing()
        {
            if (m_bitmapData != null)
            {
                m_bitmap.UnlockBits(m_bitmapData);
                m_bitmapData = null;
            }
        }

        /// <summary>
        /// Canvas yüzeyini belirtilen renk ile boyar.
        /// </summary>
        /// <param name="color">Color</param>
        public void Clear(G3Color color)
        {
            ushort col565 = color.RGB565;

            int i = col565 | (col565 << 16);
            int s = (m_bitmapWidth * m_bitmapHeight) >> 1;
            int* DIB = (int*)this.DIBImage;

            while (s != 0)
            {
                *DIB++ = i;
                s--;
            }
        }

        /// <summary>
        /// Canvas içindeki resimi belirtilen Graphics objesi üzerine, belirtilen koordinatlara kopyalar.
        /// Birimler pixel cinsindendir.
        /// </summary>
        public void DrawCanvas(Graphics g, int x, int y)
        {
            g.DrawImage(m_bitmap, x, y);
        }

        public void DrawPixel(int x, int y, G3Color color)
        {
            // Clipping
            if (x < 0 || x >= m_bitmapWidth || y < 0 || y >= m_bitmapHeight)
                return;

            ushort* DIB = &this.DIBImage[y * m_bitmapWidth + x];
            *DIB = color.RGB565;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                m_bitmap.Dispose();
                GC.SuppressFinalize(this);
            }
        }

        ~G3Surface()
        {
            Dispose();
        }

        #endregion

        public void DrawCanvas(Graphics g, Rectangle rect)
        {
            g.DrawImage(m_bitmap, rect, rect, GraphicsUnit.Pixel);
        }

        public void SmoothFilter(int weight)
        {
            BitmapFilter.Smooth(m_bitmap, weight);
        }
    }
}
