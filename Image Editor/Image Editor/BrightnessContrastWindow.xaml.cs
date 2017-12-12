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
using System.Windows.Shapes;
using System.Drawing;

namespace Image_Editor
{
    /// <summary>
    /// Interaction logic for BrightnessContrastWindow.xaml
    /// </summary>
    public partial class BrightnessContrastWindow : Window
    {
        #region Constructor
        public BrightnessContrastWindow(Bitmap image, MainWindow Sender)
        {
            InitializeComponent();
            input = image;
            mainWindow = Sender;
        }
        #endregion        

        private Adjustments brightnessContast;
        private Bitmap input;
        private MainWindow mainWindow;

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.currentImage = mainWindow.BitmapImageToBitmap(mainWindow.image.Source as BitmapImage);
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.image.Source = mainWindow.BitmapToImageSource(mainWindow.currentImage);
            Close();
        }

        private void sliderBrightness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Bitmap preview = new Bitmap(input);
            brightnessContast.AdjustBrightnessAndContrast(preview, (int) sliderBrightness.Value, (int) sliderContrast.Value);
            mainWindow.image.Source = mainWindow.BitmapToImageSource(preview);


            lblBrightness.Content = ((int)sliderBrightness.Value).ToString(); 
        } 

        private void sliderContrast_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if ((sliderBrightness == null) || (sliderContrast == null))
                return;

            Bitmap preview = new Bitmap(input);
            brightnessContast.AdjustBrightnessAndContrast(preview, (int) sliderBrightness.Value, (int) sliderContrast.Value);
            mainWindow.image.Source = mainWindow.BitmapToImageSource(preview);
            lblContrast.Content = ((int)sliderContrast.Value).ToString();
        }

        private void BrightnessContrastWindow_Load(object sender, EventArgs e)
        {
            brightnessContast = new Adjustments();

            sliderBrightness.Value = 0;
            lblBrightness.Content = "0";

            sliderContrast.Value = 0;
            lblContrast.Content = "0";
        }
    }
}
