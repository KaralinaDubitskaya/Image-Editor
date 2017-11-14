using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Windows.Interop;
using System.IO;

namespace Image_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public event ImageProcessingEventHandler AdjustmentCall;

        protected virtual void OnAdjustmentCall()
        {
            AdjustmentCall?.Invoke(this, new ImageProcessingEventArgs(currentImage));
        }

        public delegate void FilterEventHandler(object sender, FilterEventArgs e);

        public event FilterEventHandler FilterCall;

        protected virtual void OnFilterCall(ConvolutionMatrix filterKernel)
        {
            FilterCall?.Invoke(this, new FilterEventArgs(currentImage, filterKernel));
        }


        FileProcessing fileProcessing;
        Filter filter;


        public MainWindow()
        {
            InitializeComponent();

            fileProcessing = new FileProcessing();

            fileProcessing.FileOpened += ReceiveImage;

            filter = new Filter();
            this.FilterCall += filter.ApplyFilter;

            filter.ProcessCompleted += ViewProcessedImage;
            filter.ProcessCompleted += ApproveProcessing;
        }

        private Bitmap originalImage;
        private Bitmap currentImage;
        private Bitmap backupImage;

        private void ReceiveImage(object sender, FileProcessing.OpenEventArgs e)
        {
            if (e.Input == null)
                return;

            originalImage = e.Input;
            currentImage = new Bitmap(originalImage);
            backupImage = new Bitmap(originalImage);
        }

        private void BackUpCurrentImage()
        {
            backupImage = (Bitmap)currentImage.Clone();
        }

        private void RestoreBackupImage()
        {
            currentImage = (Bitmap)backupImage.Clone();
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private void ViewOriginalImage()
        {
            image.Source = BitmapToImageSource(originalImage);
        }

        private void ViewCurrentImage()
        {
            image.Source = BitmapToImageSource(currentImage);
        }

        private void ViewProcessedImage(object sender, ImageProcessingEventArgs e)
        {
            image.Source = BitmapToImageSource(e.Image);
        }

        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        private void ApproveProcessing(object sender, ImageProcessingEventArgs e)
        {
    
            currentImage = BitmapImage2Bitmap(image.Source as BitmapImage);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            image.Source = null;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            fileProcessing.SaveFileAs(currentImage);
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            fileProcessing.OpenFile();
            ViewOriginalImage();
        }

        private void BoxBlur_Click(object sender, RoutedEventArgs e)
        {
            BackUpCurrentImage();
            for (int i = 0; i < 2; i++)
                OnFilterCall(Filter.BoxBlur());
        }

        private void LightSharp_Click(object sender, RoutedEventArgs e)
        {
            BackUpCurrentImage();
            OnFilterCall(Filter.LightSharpenMatrix());
        }

        private void Sharp_Click(object sender, RoutedEventArgs e)
        {
            BackUpCurrentImage();
            OnFilterCall(Filter.SharpenMatrix());
        }

        private void Unsharp_Click(object sender, RoutedEventArgs e)
        {
            BackUpCurrentImage();
            OnFilterCall(Filter.UnsharpMasking());
        }

        private void GaussianBlur_Click(object sender, RoutedEventArgs e)
        {
            BackUpCurrentImage();
            for (int i = 0; i < 2; i++)
                OnFilterCall(Filter.GaussianBlur());
        }

        private void EdgeDetection_Click(object sender, RoutedEventArgs e)
        {
            BackUpCurrentImage();
            OnFilterCall(Filter.EdgeDetection());
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            RestoreBackupImage();
            ViewCurrentImage();
        }
    }
}
