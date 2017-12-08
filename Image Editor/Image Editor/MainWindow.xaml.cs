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
        #region Events
        #region AdjustmentCall
        public event ImageProcessingEventHandler AdjustmentCall;

        protected virtual void OnAdjustmentCall()
        {
            AdjustmentCall?.Invoke(this, new ImageProcessingEventArgs(currentImage));
        }
        #endregion
        #region FilterCall
        public delegate void FilterEventHandler(object sender, FilterEventArgs e);

        public event FilterEventHandler FilterCall;

        protected virtual void OnFilterCall(ConvolutionMatrix filterKernel)
        {
            FilterCall?.Invoke(this, new FilterEventArgs(currentImage, filterKernel));
        }
        #endregion
        #endregion

        #region Fields
        private Bitmap originalImage;
        private Bitmap currentImage;
        private Bitmap backupImage;

        FileProcessing fileProcessing; 
        Filter filter;
        #endregion

        #region Constructor
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
        #endregion

        #region Methods
        #region ReceiveImage. Initialize original, current and backup images.
        private void ReceiveImage(object sender, FileProcessing.OpenEventArgs e)
        {
            if (e.Input == null)
                return;

            originalImage = e.Input;
            currentImage = new Bitmap(originalImage);
            backupImage = new Bitmap(originalImage);
        }
        #endregion
        #region ApproveProcessing. Convert image.Source to Bitmap and set current image.
        private void ApproveProcessing(object sender, ImageProcessingEventArgs e)
        {

            currentImage = BitmapImageToBitmap(image.Source as BitmapImage);
        }
        #endregion

        #region BackUpCurrentImage. Copy current image to backaup image.
        private void BackUpCurrentImage()
        {
            backupImage = (Bitmap)currentImage.Clone();
        }
        #endregion
        #region RestoreBackupImage. Copy backup image to current image.
        private void RestoreBackupImage()
        {
            currentImage = (Bitmap)backupImage.Clone();
        }
        #endregion

        #region BitmapToImageSource. Convert Bitmap to BitmapImage.
        /* Image is a base abstract class representing images in GDI+. 
         * Bitmap is a concrete implementation of this base class.
         * BitmapImage is a way to represent an image in a vector based GUI engine like WPF and Silverlight.
         * Contrary to a Bitmap, it is not based on GDI+. It is based on the Windows Imaging Component.
         * It is the way to load a BitmapImage from a Bitmap.
         */
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
        #endregion
        #region BitmapImageToBitmap. Convert BitmapImage to Bitmap.
        private Bitmap BitmapImageToBitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder bitmapEncoder = new BmpBitmapEncoder();
                bitmapEncoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                bitmapEncoder.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }
        #endregion

        #region ViewOriginalImage. Convert to BitmapImage and show original image.
        private void ViewOriginalImage()
        {
            image.Source = BitmapToImageSource(originalImage);
        }
        #endregion
        #region ViewCurrentImage. Convert to BitmapImage and show current image.
        private void ViewCurrentImage()
        {
            image.Source = BitmapToImageSource(currentImage);
        }
        #endregion
        #region ViewProccesedImage. Convert Bitmap to ImageSource and set image.Source.
        private void ViewProcessedImage(object sender, ImageProcessingEventArgs e)
        {
            image.Source = BitmapToImageSource(e.Image);
        }
        #endregion
        #endregion

        #region Button_Click Events       

        #region Open file...
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            fileProcessing.OpenFile();
            ViewOriginalImage();
        }
        #endregion
        #region Undo
        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            RestoreBackupImage();
            ViewCurrentImage();
        }
        #endregion
        #region Save
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            fileProcessing.SaveFile(currentImage);
        }
        #endregion
        #region Save as...
        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            fileProcessing.SaveFileAs(currentImage);
        }
        #endregion
        #region Close current image
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            image.Source = null;
        }
        #endregion
        #region Exit
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        #region Filters
        #region BoxBlur
        private void BoxBlur_Click(object sender, RoutedEventArgs e)
        {
            BackUpCurrentImage();
            for (int i = 0; i < 2; i++)
                OnFilterCall(Filter.BoxBlur());
        }
        #endregion
        #region LightSharp
        private void LightSharp_Click(object sender, RoutedEventArgs e)
        {
            BackUpCurrentImage();
            OnFilterCall(Filter.LightSharpenMatrix());
        }
        #endregion
        #region Sharp
        private void Sharp_Click(object sender, RoutedEventArgs e)
        {
            BackUpCurrentImage();
            OnFilterCall(Filter.SharpenMatrix());
        }
        #endregion
        #region Unsharp
        private void Unsharp_Click(object sender, RoutedEventArgs e)
        {
            BackUpCurrentImage();
            OnFilterCall(Filter.UnsharpMasking());
        }
        #endregion
        #region GaussianBlur
        private void GaussianBlur_Click(object sender, RoutedEventArgs e)
        {
            BackUpCurrentImage();
            for (int i = 0; i < 2; i++)
                OnFilterCall(Filter.GaussianBlur());
        }
        #endregion
        #region EdgeDetection
        private void EdgeDetection_Click(object sender, RoutedEventArgs e)
        {
            BackUpCurrentImage();
            OnFilterCall(Filter.EdgeDetection());
        }
        #endregion
        #endregion

        #endregion
    }
}
