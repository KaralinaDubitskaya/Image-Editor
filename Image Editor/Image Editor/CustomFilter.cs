using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Editor
{
    public class CustomFilter : ImageFilter
    {
        public delegate byte ComputeRgbComponentValue(byte[] neighborhood);

        public new void SetUp(int size)
        {
            base.SetUp(size);
        }

        ComputeRgbComponentValue computeRgbComponentValue;

        protected override byte ComputeNewRgbComponentValue(byte[] neighborhood)
        {
            return computeRgbComponentValue(neighborhood);
        }
    }
}
