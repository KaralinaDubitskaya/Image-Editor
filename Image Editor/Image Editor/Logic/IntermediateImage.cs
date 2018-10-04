using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Image_Editor
{
    class IntermediateImage
    {
        private Bitmap intermediateImage;
        private LockedBitmap intermediateData;

        private Bitmap sourceImage;
        private LockedBitmap sourceData;

        private int gap;

        public Bitmap OutputImage
        {
            get { return intermediateImage; }
        }

        public IntermediateImage(Bitmap source, int offset)
        {
            sourceImage = source;
            gap = offset;
            intermediateImage = new Bitmap(sourceImage.Width + 2 * gap, sourceImage.Height + 2 * gap);
        }

        private void LockArea(Rectangle sourceLockedArea, Rectangle intermediateLockedArea)
        {
            sourceData = new LockedBitmap(sourceImage, sourceLockedArea);
            intermediateData = new LockedBitmap(intermediateImage, intermediateLockedArea);
        }


        public void UnlockArea()
        {
            sourceData.Unlock(sourceImage);
            intermediateData.Unlock(intermediateImage);
        }


        unsafe public void Copy()
        {
            Parallel.For(0, sourceData.HeightInPixels, iY =>
            {
                byte* sourceCurrentLine = sourceData.FirstByte + (iY * sourceData.Stride);
                byte* intermediateCurrentLine = intermediateData.FirstByte + (iY * intermediateData.Stride);

                for (int iX = 0; iX < sourceData.WidthInBytes; ++iX)
                    intermediateCurrentLine[iX] = sourceCurrentLine[iX];
            });
        }


        public void CopyArea(Rectangle sourceLockedArea, Rectangle intermediateLockedArea)
        {
            LockArea(sourceLockedArea, intermediateLockedArea);
            Copy();
            UnlockArea();
        }


        public void FillInCenter()
        {
            CopyArea(new Rectangle(0, 0, sourceImage.Width, sourceImage.Height),
                     new Rectangle(gap, gap, intermediateImage.Width - gap * 2, intermediateImage.Height - gap * 2));
        }


        private void FillInLeftUpperCorner()
        {
            CopyArea(new Rectangle(0, 0, gap, gap),
                     new Rectangle(0, 0, gap, gap));
        }


        private void FillInRightUpperCorner()
        {
            CopyArea(new Rectangle(sourceImage.Width - gap, 0, gap, gap),
                     new Rectangle(intermediateImage.Width - gap, 0, gap, gap));
        }


        private void FillInRightLowerCorner()
        {
            CopyArea(new Rectangle(sourceImage.Width - gap, sourceImage.Height - gap, gap, gap),
                     new Rectangle(intermediateImage.Width - gap, intermediateImage.Height - gap, gap, gap));
        }


        private void FillInLeftLowerCorner()
        {
            CopyArea(new Rectangle(0, sourceImage.Height - gap, gap, gap),
                     new Rectangle(0, intermediateImage.Height - gap, gap, gap));
        }


        private void FillInUpperSide()
        {
            CopyArea(new Rectangle(0, 0, sourceImage.Width, gap),
                     new Rectangle(gap, 0, intermediateImage.Width - 2 * gap, gap));
        }


        private void FillInLowerSide()
        {
            CopyArea(new Rectangle(0, sourceImage.Height - gap, sourceImage.Width, gap),
                     new Rectangle(gap, intermediateImage.Height - gap, intermediateImage.Width - gap, gap));
        }


        private void FillInLeftSide()
        {
            CopyArea(new Rectangle(0, 0, gap, sourceImage.Height),
                     new Rectangle(0, gap, gap, intermediateImage.Height - gap));
        }


        private void FillInRightSide()
        {
            CopyArea(new Rectangle(sourceImage.Width - gap, 0, gap, sourceImage.Height),
                     new Rectangle(intermediateImage.Width - gap, gap, gap, intermediateImage.Height - gap));
        }


        public void FillIn()
        {
            FillInLeftUpperCorner();
            FillInUpperSide();
            FillInRightUpperCorner();
            FillInRightLowerCorner();
            FillInLeftLowerCorner();
            FillInLeftSide();
            FillInRightSide();
            FillInLowerSide();
            FillInCenter();
        }
    }
}
