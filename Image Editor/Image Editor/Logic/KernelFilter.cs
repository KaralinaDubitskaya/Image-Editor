using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Image_Editor
{
    class KernelFilter : ImageFilter
    {
        private double factor;

        private int[] kernel;
        private int kernelSize;

        private void SetUpKernel(ConvolutionMatrix filterKernel)
        {
            kernel = new int[kernelSize];

            for (int iY = 0; iY < size; ++iY)
                for (int iX = 0; iX < size; ++iX)
                    kernel[iY * size + iX] = filterKernel.matrix[iY][iX];
        }

        public KernelFilter(ConvolutionMatrix filterKernel)
        {
            SetUp(filterKernel.size);
            kernelSize = size * size;
            factor = filterKernel.convolutionFactor;
            SetUpKernel(filterKernel);
        }

        protected override byte ComputeNewRgbComponentValue(byte[] neighborhood)
        {
            double sum = 0;

            for (int i = 0; i < neighborhood.Length; ++i)
                sum += neighborhood[i] * kernel[i];

            sum *= factor;

            return RgbComponentOperations.ControlOverflow(sum);
        }

        public Bitmap ApplyKernelFilter(Bitmap source)
        {
            Bitmap output = new Bitmap(source.Width, source.Height);
            Apply(source, output);
            return output;
        }
    }
}
