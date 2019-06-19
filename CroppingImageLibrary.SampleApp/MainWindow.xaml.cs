using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Microsoft.WindowsAPICodePack.Dialogs;
using MessageBox = System.Windows.Forms.MessageBox;

namespace CroppingImageLibrary.SampleApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CroppingWindow _croppingWindow;
        private List<string> image_list;
        private string output_path;
        private int pointer = 0;

        public MainWindow()
        {
            InitializeComponent();
            image_list = new List<string>();
            Topmost = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_croppingWindow != null)
                return;            
            OpenFileDialog op = new OpenFileDialog();
            op.Multiselect = true;
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                        "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                        "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                image_list = new List<string>(op.FileNames);
                lstImagelist.ItemsSource = image_list;
                Load_Image();
            }
        }

        private void Load_Image()
        {
            _croppingWindow = new CroppingWindow();
            _croppingWindow.Closed += (a, b) => _croppingWindow = null;
            _croppingWindow.Height = new BitmapImage(new Uri(image_list[0])).Height;
            _croppingWindow.Width = new BitmapImage(new Uri(image_list[0])).Width;

            _croppingWindow.SourceImage.Source = new BitmapImage(new Uri(image_list[0]));
            _croppingWindow.SourceImage.Height = new BitmapImage(new Uri(image_list[0])).Height;
            _croppingWindow.SourceImage.Width = new BitmapImage(new Uri(image_list[0])).Width;
            if (_croppingWindow.SourceImage.Height != 0 && _croppingWindow.SourceImage.Width !=0)
                _croppingWindow.Show();
            else
            {
                image_list.RemoveAt(pointer);
                lstImagelist.ItemsSource = image_list;
                Load_Image();
            }

        }

        private string getCropImageName()
        {
            string curPath = "null";
            if ((pointer + 1) < image_list.Count)
            {
                image_list.RemoveAt(pointer);
                lstImagelist.ItemsSource = image_list;
            }
            string old_path = Path.GetDirectoryName(image_list[0]);
            curPath = image_list[0].Replace(old_path,output_path);
            return curPath;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            BitmapFrame croppedBitmapFrame = _croppingWindow.CroppingAdorner.GetCroppedBitmapFrame();
            //create PNG image
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(croppedBitmapFrame));
            //save image to file
            if(image_list.Count > 0)
            {
                string filename = getCropImageName();
                using (FileStream imageFile = new FileStream(filename, FileMode.Create, FileAccess.Write))
                {
                    encoder.Save(imageFile);
                    imageFile.Flush();
                    imageFile.Close();
                    _croppingWindow.Close();
                    Load_Image();
                }
            }
            else
            {
                this.Close();
            }
           

        }

        private void BtnChoosePath_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\Users";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                btnCropAndSave.IsEnabled = true;
                output_path = dialog.FileName;
            }
        }
    }
}
