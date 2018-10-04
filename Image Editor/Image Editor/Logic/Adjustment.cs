using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Image_Editor
{

    enum Argb : byte
    {
        blue, green, red, alfa
    }

    class Adjustments
    {
        #region delegate ApplyToPixel(byte* blue, double factor)
        /* Objects of this delegate type can refer to methods that take as arguments can
         * refer to methods that take as arguments pointer to a single byte of the image (blue)
         * and real number - the coefficient of intensity correction.
         * Delegated methods change the RGBA-values of the pixel by a certain algorithm.
         */
        private unsafe delegate void ApplyToPixel(byte* blue, double factor);
        #endregion

        #region Methods
        #region Adjust
        // Applies to each byte delegated method
        private unsafe void Adjust(Bitmap bitmap, double factor, ApplyToPixel applyToPixel)
        {
            LockedBitmap lockedBitmap = new LockedBitmap(bitmap);

            Parallel.For(0, lockedBitmap.HeightInPixels, iY =>
            {
                byte* pixel = lockedBitmap.FirstByte + (iY * lockedBitmap.Stride);
                for (int iX = 0; iX < lockedBitmap.WidthInBytes; iX += lockedBitmap.BytesPerPixel)
                {
                    applyToPixel(pixel, factor);
                    pixel += lockedBitmap.BytesPerPixel;
                }
            });

            lockedBitmap.Unlock(bitmap);
        }

        struct Adjustment
        {
            public ApplyToPixel adjustment;
            public double factor;

            public Adjustment(ApplyToPixel applyToPixel, double f)
            {
                adjustment = applyToPixel;
                factor = f;
            }
        }

        // Applies to each byte all delegated methods
        private unsafe void Adjust(Bitmap bitmap, List<Adjustment> adjustmentsPack)
        {
            LockedBitmap lockedBitmap = new LockedBitmap(bitmap);

            Parallel.For(0, lockedBitmap.HeightInPixels, iY =>
            {
                byte* pixel = lockedBitmap.FirstByte + (iY * lockedBitmap.Stride);
                for (int iX = 0; iX < lockedBitmap.WidthInBytes; iX += lockedBitmap.BytesPerPixel)
                {
                    foreach (Adjustment adjustment in adjustmentsPack)
                        adjustment.adjustment(pixel, adjustment.factor);

                    pixel += lockedBitmap.BytesPerPixel;
                }
            });

            lockedBitmap.Unlock(bitmap);
        }
        #endregion


        public unsafe void AdjustBrightnessAndContrast(Bitmap source, int brightnessFactor, int contrastFactor)
        {
            List<Adjustment> AdjustmentsPack = new List<Adjustment>();

            if (brightnessFactor != 0)
                AdjustmentsPack.Add(new Adjustment(ApplyBrightnessToPixel, brightnessFactor));

            if (contrastFactor != 0)
                AdjustmentsPack.Add(new Adjustment(ApplyContrastToPixel, contrastFactor));

            if (AdjustmentsPack.Count != 0)
                Adjust(source, AdjustmentsPack);
        }
        public unsafe void AdjustExposure(Bitmap source, double exposure, double gamma)
        {
            List<Adjustment> AdjustmentsPack = new List<Adjustment>();

            if (exposure != 0)
                AdjustmentsPack.Add(new Adjustment(ApplyExposureCompensationToPixel, exposure));

            if (gamma != 0)
                AdjustmentsPack.Add(new Adjustment(ApplyGammaCorrectionToPixel, gamma));

            if (AdjustmentsPack.Count != 0)
                Adjust(source, AdjustmentsPack);
        }
        public unsafe void AdjustColorBalance(Bitmap source, int redFactor, int greenFactor, int blueFactor)
        {

            List<Adjustment> adjustmentsPack = new List<Adjustment>();

            if (redFactor != 0)
                adjustmentsPack.Add(new Adjustment(ApplyBrightnessToPixelRed, redFactor));

            if (greenFactor != 0)
                adjustmentsPack.Add(new Adjustment(ApplyBrightnessToPixelGreen, greenFactor));

            if (blueFactor != 0)
                adjustmentsPack.Add(new Adjustment(ApplyBrightnessToPixelBlue, blueFactor));

            if (adjustmentsPack.Count != 0)
                Adjust(source, adjustmentsPack);
        }


        private unsafe void ApplyContrastToPixel(byte* blue, double factor)
        {
            for (Argb i = Argb.blue; i <= Argb.red; ++i)
            {
                byte componentValue = *(blue + (byte)i);
                *(blue + (byte)i) = RgbComponentOperations.ChangeContrast(componentValue, (int)factor);
            }
        }

        public unsafe void Sepia(Bitmap source, int factor)
        {
            ApplyToPixel adjustment = new ApplyToPixel(ApplySepiaToPixel);
            Adjust(source, factor, adjustment);
        }

        public unsafe void Invert(Bitmap source, int factor)
        {
            ApplyToPixel adjustment = new ApplyToPixel(ApplyInversionToPixel);
            Adjust(source, factor, adjustment);
        }

        public unsafe void BlackAndWhite(Bitmap source, int factor)
        {
            ApplyToPixel adjustment = new ApplyToPixel(ApplyBlackAndWhiteToPixel);
            Adjust(source, factor, adjustment);
        }
        
        public unsafe void Threshold(Bitmap source, int factor)
        {
            ApplyToPixel adjustment = new ApplyToPixel(ApplyThresholdToPixel);
            Adjust(source, factor, adjustment);
        }

        private unsafe void ApplyBrightnessToPixel(byte* blue, double factor)
        {
            for (Argb i = Argb.blue; i <= Argb.red; ++i)
            {
                byte componentValue = *(blue + (byte)i);
                *(blue + (byte)i) = RgbComponentOperations.ChangeBrightness(componentValue, factor);
            }
        }

        private unsafe void ApplyThresholdToPixel(byte* blue, double factor)
        {
            *blue = *(blue + (byte)Argb.green) = *(blue + (byte)Argb.red) =
            RgbComponentOperations.Threshold(*blue, *(blue + (byte)Argb.green), *(blue + (byte)Argb.red), (byte)factor);
        }

        private unsafe void ApplyBlackAndWhiteToPixel(byte* blue, double factor)
        {
            *blue = *(blue + (byte)Argb.green) = *(blue + (byte)Argb.red) =
            RgbComponentOperations.BlackAndWhite(*blue, *(blue + (byte)Argb.green), *(blue + (byte)Argb.red));
        }

        private unsafe void ApplyExposureCompensationToPixel(byte* blue, double factor)
        {
            for (Argb i = Argb.blue; i <= Argb.red; ++i)
            {
                byte componentValue = *(blue + (byte)i);
                *(blue + (byte)i) = RgbComponentOperations.Exposure(componentValue, factor);
            }
        }

        private unsafe void ApplyGammaCorrectionToPixel(byte* blue, double factor)
        {
            for (Argb i = Argb.blue; i <= Argb.red; ++i)
            {
                byte componentValue = *(blue + (byte)i);
                *(blue + (byte)i) = RgbComponentOperations.Gamma(componentValue, factor);
            }
        }

        private unsafe void ApplyBrightnessToPixelRed(byte* blue, double factor)
        {
            byte componentValue = *(blue + (byte)Argb.red);
            *(blue + (byte)Argb.red) = RgbComponentOperations.ChangeBrightness(componentValue, factor);
        }

        private unsafe void ApplyBrightnessToPixelGreen(byte* blue, double factor)
        {
            byte componentValue = *(blue + (byte)Argb.green);
            *(blue + (byte)Argb.green) = RgbComponentOperations.ChangeBrightness(componentValue, factor);
        }

        private unsafe void ApplyBrightnessToPixelBlue(byte* blue, double factor)
        {
            byte componentValue = *blue;
            *blue = RgbComponentOperations.ChangeBrightness(componentValue, factor);
        }

        private unsafe void ApplySepiaToPixel(byte* blue, double factor)
        {
            int tone = RgbComponentOperations.ComputeSepiaTone(*blue, *(blue + (byte)Argb.green), *(blue + (byte)Argb.red));
            *blue = RgbComponentOperations.ComputeSepiaBlue(tone);
            *(blue + (byte)Argb.green) = RgbComponentOperations.ComputeSepiaGreen(tone);
            *(blue + (byte)Argb.red) = RgbComponentOperations.ComputeSepiaRed(tone);
        }

        private unsafe void ApplyInversionToPixel(byte* blue, double factor)
        {
            for (Argb i = Argb.blue; i <= Argb.red; ++i)
            {
                byte componentValue = *(blue + (byte)i);
                *(blue + (byte)i) = RgbComponentOperations.Invert(componentValue, (int)factor);
            }
        }
        #endregion
    }
}
