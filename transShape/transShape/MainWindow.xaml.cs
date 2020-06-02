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
using System.IO;
using System.Windows.Forms;
using OpenCvSharp;
using Point = System.Windows.Point;
using Window = System.Windows.Window;

namespace transShape
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void getPic_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "图像文件|*.jpg;*.png;*.jpeg;*.bmp;*.gif|所有文件|*.*"
            };
            if (ofd.ShowDialog() == true)
            {
                BitmapImage bmp = new BitmapImage();
                using (FileStream fs = new FileStream(ofd.FileName, FileMode.Open))
                {
                    bmp.BeginInit();
                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    bmp.StreamSource = fs;
                    bmp.EndInit();
                }
                bmp.Freeze();
                inPutImage.Height = bmp.Height;
                
                baseWindow.Height = (bmp.Height + 200);

                inPutImage.Width = bmp.Width; 
                baseWindow.Width = (bmp.Width + 200);
                inPutImage.Source = bmp;
            
            }
        }

        private void setSavePath(object sender, RoutedEventArgs e)
        {
            String path = GetFolder();
            if(path != null)
                savePath.Text = path;
        }
        /// <summary>
        /// 控制点击图像
        /// </summary>
        private double[,] clickPosition = new double[4,2];
        private int clickTime = 0;
        private bool canSetClickPosition = true;
        private void img_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!canSetClickPosition)
                return;
            Point point = e.GetPosition(inPutImage);
            double a = point.X;
            clickPosition[clickTime,0] = point.X;
            clickPosition[clickTime,1] = point.Y;
            clickTime++;
            if (clickTime > 3)
            {
                clickTime = 0;
                canSetClickPosition = false;
                transImage.IsEnabled = true;
            }
        }

        private void transPic(object sender, RoutedEventArgs e)
        {
            
            Bitmap bmp = ShapeService.ImageSourceToBitmap(inPutImage.Source);
            Mat mat = ShapeService.Bitmap2Mat(bmp);
            Mat mat2 = new Mat();
            ShapeService.transImage(mat,mat2,clickPosition);
            Cv2.ImShow("result",mat2);
            String path = savePath.Text;
            path += "\\"+DateTime.Now.ToString().Replace("/", "").Replace(" ", "").Replace(":", "") + ".jpg";
            try
            {
                Cv2.ImWrite(path, mat2);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            canSetClickPosition = true;
            transImage.IsEnabled = false;
        }

        private  string GetFolder()
        {
            FolderBrowserDialog m_Dialog = new FolderBrowserDialog();
            DialogResult result = m_Dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return null;
            }
            return m_Dialog.SelectedPath.Trim();

        }

        private void showReadMe(object sender, RoutedEventArgs e)
        {
            ReadMe rm = new ReadMe();
            rm.ShowDialog();
        }

    }
}
