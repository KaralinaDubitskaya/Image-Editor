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
    /// Interaction logic for ThresholdWindow.xaml
    /// </summary>
    public partial class ThresholdWindow : Window
    {
        #region Constructor
        public ThresholdWindow(Bitmap image, MainWindow Sender)
        {
            InitializeComponent();
            input = image;
            mainWindow = Sender;
        }
        #endregion        

        private Adjustments threshold;
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

        private void sliderThreshold_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sliderThreshold == null) 
                return;

            Bitmap preview = new Bitmap(input);
            threshold.Threshold(preview, (int)sliderThreshold.Value);
            mainWindow.image.Source = mainWindow.BitmapToImageSource(preview);


            lblThreshold.Content = ((int)sliderThreshold.Value).ToString();
        }

        private void ThresholdWindow_Load(object sender, EventArgs e)
        {
            threshold = new Adjustments();

            sliderThreshold.Value = 0;
            lblThreshold.Content = "0";
        }
    }
}
