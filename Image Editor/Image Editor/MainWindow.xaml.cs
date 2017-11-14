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
         

        FileProcessing fileProcessing;

        public MainWindow()
        {
            InitializeComponent();

            fileProcessing = new FileProcessing();

            fileProcessing.FileOpened += ReceiveImage;
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
    }
}
