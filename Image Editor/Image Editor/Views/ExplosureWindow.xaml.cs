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
    /// Interaction logic for Explosure.xaml
    /// </summary>
    public partial class Explosure : Window
    {

        #region Constructor
        public Explosure(Bitmap image, MainWindow Sender)
        {
            InitializeComponent();
            input = image;
            mainWindow = Sender;
        }
        #endregion        

        private Adjustments explosure;
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

        private void sliderExplosure_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if ((sliderExplosure == null) || (sliderGamma == null))
                return;

            Bitmap preview = new Bitmap(input);
            explosure.AdjustExposure(preview, ComputeExposureValue(), ComputeGammaValue());
            mainWindow.image.Source = mainWindow.BitmapToImageSource(preview);


            lblExplosure.Content = ((int)sliderExplosure.Value).ToString();
        }

        private void sliderGamma_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if ((sliderExplosure == null) || (sliderGamma == null))
                return;

            Bitmap preview = new Bitmap(input);
            explosure.AdjustExposure(preview, ComputeExposureValue(), ComputeGammaValue());
            mainWindow.image.Source = mainWindow.BitmapToImageSource(preview);


            lblGamma.Content = ((int)(sliderGamma.Value + 400)).ToString();
        }

        private void ExplosureWindow_Load(object sender, EventArgs e)
        {
            explosure = new Adjustments();
        }

        private double ComputeGammaValue()
        {
            return (sliderGamma.Value + 400) < 400 ? (sliderGamma.Value + 400) / 400.0 : ((sliderGamma.Value + 400) - 400.0) * 7 / 400 + 1;
        }


        private double ComputeExposureValue()
        {
            return sliderExplosure.Value / 100.0;
        }

        private int ComputesliderGammaValue(double gamma)
        {
            return (int)(gamma < 1 ? gamma * 400 : (gamma - 1) * 400 / 7 + 400);
        }
    }
}
