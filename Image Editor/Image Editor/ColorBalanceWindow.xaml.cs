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
    /// Interaction logic for ColorBalanceWindow.xaml
    /// </summary>
    public partial class ColorBalanceWindow : Window
    {
        #region Constructor
        public ColorBalanceWindow(Bitmap image, MainWindow Sender)
        {
            InitializeComponent();
            input = image;
            mainWindow = Sender;
        }
        #endregion 

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.currentImage = mainWindow.BitmapImageToBitmap(mainWindow.image.Source as BitmapImage);
            Close();
        }
               

        private Adjustments colorBalance;
        private Bitmap input;
        private MainWindow mainWindow;


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.image.Source = mainWindow.BitmapToImageSource(mainWindow.currentImage);
            Close();
        }

        private void sliderRed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if ((sliderRed == null) || (sliderGreen == null) || (sliderBlue == null))
                return;

            Bitmap preview = new Bitmap(input);
            colorBalance.AdjustColorBalance(preview, (int)sliderRed.Value, (int)sliderGreen.Value, (int)sliderBlue.Value);
            mainWindow.image.Source = mainWindow.BitmapToImageSource(preview);


            lblRed.Content = ((int)sliderRed.Value).ToString();
        }

        private void sliderGreen_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if ((sliderRed == null) || (sliderGreen == null) || (sliderBlue == null))
                return;

            Bitmap preview = new Bitmap(input);
            colorBalance.AdjustColorBalance(preview, (int)sliderRed.Value, (int)sliderGreen.Value, (int)sliderBlue.Value);
            mainWindow.image.Source = mainWindow.BitmapToImageSource(preview);


            lblGreen.Content = ((int)sliderGreen.Value).ToString();
        }

        private void sliderBlue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if ((sliderRed == null) || (sliderGreen == null) || (sliderBlue == null))
                return;

            Bitmap preview = new Bitmap(input);
            colorBalance.AdjustColorBalance(preview, (int)sliderRed.Value, (int)sliderGreen.Value, (int)sliderBlue.Value);
            mainWindow.image.Source = mainWindow.BitmapToImageSource(preview);


            lblBlue.Content = ((int)sliderBlue.Value).ToString();
        }

        private void ColorBalanceWindow_Load(object sender, EventArgs e)
        {
            colorBalance = new Adjustments();

            sliderRed.Value = 0;
            lblRed.Content = "0";

            sliderGreen.Value = 0;
            lblGreen.Content = "0";

            sliderBlue.Value = 0;
            lblBlue.Content = "0";
        }
    }
}
