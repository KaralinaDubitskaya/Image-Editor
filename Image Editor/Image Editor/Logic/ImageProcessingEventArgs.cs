using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Image_Editor
{

    public delegate void ImageProcessingEventHandler(object sender, ImageProcessingEventArgs e);

    public class ImageProcessingEventArgs
    {
        public Bitmap Image { get; set; }
        public ImageProcessingEventArgs(Bitmap bitmap)
        {
            Image = bitmap;
        }

        public ImageProcessingEventArgs() { }
    }

    public delegate Bitmap CustomFilterAdjustment(Bitmap original);

    public class FilterEventArgs
    {
        public FilterEventArgs(Bitmap input, ConvolutionMatrix filter)
        {
            Input = input;
            Filter = filter;
        }

        public Bitmap Input { get; }
        public ConvolutionMatrix Filter { get; }
    }
}
