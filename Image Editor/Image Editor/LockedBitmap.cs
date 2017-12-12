using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing;

namespace Image_Editor
{
    #region enum Bits
    enum Bits : byte
    {
        bitsInByte = 8
    }
    #endregion

    #region class LockedBitmap
    /* Class is used to lock (and unlock) an existing bitmap in system memory 
     * so that it can be changed programmatically. LockBits method offers better
     * performance for large-scale changes than SetPixel method. 
     */
    class LockedBitmap
    {
        #region Properties
        public BitmapData Data { get; }

        public int BytesPerPixel { get; }

        public int HeightInPixels { get; }

        public int WidthInBytes { get; }

        /* The stride is the width of a single row of pixels (a scan line),
         * rounded up to a four-byte boundary. If the stride is positive,
         * the bitmap is top-down. If the stride is negative, the bitmap is bottom-up.
         */
        public int Stride { get; }

        public unsafe byte* FirstByte { get; }
        #endregion

        #region Methods
        public unsafe LockedBitmap(Bitmap source, Rectangle lockedArea)
        {
            Data = source.LockBits(lockedArea, ImageLockMode.ReadWrite, source.PixelFormat);

            BytesPerPixel = Bitmap.GetPixelFormatSize(source.PixelFormat) / (int)Bits.bitsInByte;
            HeightInPixels = Data.Height;
            WidthInBytes = Data.Width * BytesPerPixel;
            FirstByte = (byte*)Data.Scan0;
            Stride = Data.Stride;
        }

        public unsafe LockedBitmap(Bitmap source)
        {
            Data = source.LockBits(new Rectangle(0, 0, source.Width, source.Height),
                                       ImageLockMode.ReadWrite, source.PixelFormat);

            BytesPerPixel = Bitmap.GetPixelFormatSize(source.PixelFormat) / (int)Bits.bitsInByte;
            HeightInPixels = Data.Height;
            WidthInBytes = Data.Width * BytesPerPixel;
            FirstByte = (byte*)Data.Scan0;
            Stride = Data.Stride;
        }

        public void Unlock(Bitmap source)
        {
            source.UnlockBits(Data);
        }
        #endregion
    }
    #endregion
}
