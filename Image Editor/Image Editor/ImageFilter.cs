using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Image_Editor
{
    public abstract class ImageFilter
    {
        protected abstract byte ComputeNewRgbComponentValue(byte[] neighborhood);

        protected int size = 0;
        protected int gap;
        protected int bytePerPixel;
        protected int gapInBytes;
        protected int neighborhoodSize;

        protected bool IsValid()
        {
            return size > 1 ? true : false;
        }

        protected void SetUp(int s)
        {
            size = s;
            gap = size / 2;
            neighborhoodSize = size * size;
        }

        private byte[] InitializePixelNeighborhood()
        {
            byte[] neighborhood = new byte[neighborhoodSize];
            return neighborhood;
        }

        private unsafe byte[] GetRgbComponentNeighborhood(LockedBitmap lockedBitmapData, byte* rgbComponent)
        {
            byte[] neighborhood = InitializePixelNeighborhood();
            byte* neighbor = rgbComponent - gap * lockedBitmapData.Stride - gapInBytes;
            int offset = lockedBitmapData.Stride - size * bytePerPixel;

            for (int iY = 0; iY < size; ++iY)
            {
                for (int iX = 0; iX < size; ++iX)
                {
                    neighborhood[iY * size + iX] = *neighbor;
                    neighbor += bytePerPixel;
                }

                neighbor += offset;
            }

            return neighborhood;
        }

        private unsafe void ApplyFilterToBitmap(LockedBitmap intermediate, LockedBitmap bitmap)
        {
            Parallel.For(0, bitmap.HeightInPixels, iY =>
            {
                byte* outputCurrentByte = bitmap.FirstByte + (iY * bitmap.Stride);
                byte* intermediateCurrentByte = intermediate.FirstByte + ((iY + gap) * intermediate.Stride) + gapInBytes;

                for (int i = 0; i < bitmap.WidthInBytes; i += bytePerPixel)
                {
                    for (Argb j = Argb.blue; j <= Argb.red; ++j)
                    {
                        *(outputCurrentByte + (byte)j) =
                        ComputeNewRgbComponentValue(GetRgbComponentNeighborhood(intermediate, intermediateCurrentByte + (byte)j));
                    }

                    *(outputCurrentByte + (byte)Argb.alfa) = *(intermediateCurrentByte + (byte)Argb.alfa);

                    outputCurrentByte += bytePerPixel;
                    intermediateCurrentByte += bytePerPixel;
                }
            });
        }

        protected void Apply(Bitmap input, Bitmap output)
        {
            if (!IsValid())
                return;

            bytePerPixel = Bitmap.GetPixelFormatSize(input.PixelFormat) / (int)Bits.bitsInByte;
            gapInBytes = gap * bytePerPixel;

            IntermediateImage intermediate = new IntermediateImage(input, gap);
            intermediate.FillIn();

            LockedBitmap lockedBitmap = new LockedBitmap(output);
            LockedBitmap lockedIntermediate = new LockedBitmap(intermediate.OutputImage);

            ApplyFilterToBitmap(lockedIntermediate, lockedBitmap);

            lockedBitmap.Unlock(output);
            lockedIntermediate.Unlock(intermediate.OutputImage);
        }
    }
}
