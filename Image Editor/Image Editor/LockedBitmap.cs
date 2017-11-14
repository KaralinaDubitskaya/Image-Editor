using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing;

namespace Image_Editor
{

    enum Bits: byte
    {
        bitsInByte = 8
    }

    class LockedBitmap
    {
        public BitmapData Data { get; }

        public int BytesPerPixel { get; }

        public int HeightInPixels { get; }

        public int WidthInBytes { get; }

        public int Stride { get; }

        public unsafe byte* FirstByte { get; }

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
    }
}
