using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Image_Editor
{
    class FileProcessing
    {
        public class OpenEventArgs
        {
            public OpenEventArgs(Bitmap bitmap)
            {
                Input = bitmap;
            }

            public Bitmap Input { get;  }
        }

        public delegate void OpenFileEventHandler(object sender, OpenEventArgs e);

        public event OpenFileEventHandler FileOpened;

        protected virtual void OnFileOpened(Bitmap input)
        {
            FileOpened?.Invoke(this, new OpenEventArgs(input));
        }

        public void OpenFile()
        {
            Bitmap input = null;
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog.Filter = "Image Files(*.jpg; *.bmp; *.png)| *.jpg; *.bmp; *.png";
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    input = new Bitmap(Image.FromFile(openFileDialog.FileName));
                }

                catch (Exception exception)
                {
                    MessageBox.Show("Error: Could not open file. Original error: " + exception.Message);
                }
            }

            OnFileOpened(input);
        }

        public void SaveFileAs(Bitmap image)
        {

            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "JPG Image|*.jpg|BMP Image|*.bmp|PNG Image|*.png";
            saveFileDialog.AddExtension = true;
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    image.Save(saveFileDialog.FileName);
                }

                catch (Exception exception)
                {
                    MessageBox.Show("Error: Could not save file. Original error: " + exception.Message);
                }
            }
        }
    }
}
