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
        #region Events
        #region class OpenEventArgs
        public class OpenEventArgs
        {
            #region Constructor
            public OpenEventArgs(Bitmap bitmap)
            {
                Input = bitmap;
            }
            #endregion

            #region Properties
            public Bitmap Input { get; }
            #endregion
        }
        #endregion
        #region FileOpened
        public delegate void OpenFileEventHandler(object sender, OpenEventArgs e);

        public event OpenFileEventHandler FileOpened;

        protected virtual void OnFileOpened(Bitmap input)
        {
            FileOpened?.Invoke(this, new OpenEventArgs(input));
        }
        #endregion
        #endregion

        #region Properties
        public string FileName { get; set; }
        #endregion      

        #region Methods
        #region OpenFile
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
                    FileName = String.Copy(openFileDialog.FileName);
                }

                catch (Exception exception)
                {
                    MessageBox.Show("Error: Could not open file. Original error: " + exception.Message);
                }
            }

            OnFileOpened(input);
        }
        #endregion
        #region SaveFileAs
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
                    FileName = String.Copy(saveFileDialog.FileName);
                }

                catch (Exception exception)
                {
                    MessageBox.Show("Error: Could not save file. Original error: " + exception.Message);
                }
            }
        }
        #endregion
        #region SaveFile
        public void SaveFile(Bitmap image)
        {
            if (this.FileName != null)
            {
                try
                {
                    image.Save(this.FileName);
                }

                catch (Exception exception)
                {
                    MessageBox.Show("Error: Could not save file. Original error: " + exception.Message);
                }
            }
            else
            {
                MessageBox.Show("Error: Choose file.");
            }
        }
        #endregion
        #endregion
    }
}
