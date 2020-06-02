using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenCvSharp;
using Size = OpenCvSharp.Size;

namespace transShape
{
    static class ShapeService
    {
        public static void transImage(Mat src, Mat dst,double[,] positon)
        {
            //变换前的四点
            Point2f[] srcPoints = new Point2f[]
            {
                new Point2f((float) positon[0,0], (float) positon[0,1]),
                new Point2f((float) positon[1,0], (float) positon[1,1]),
                new Point2f((float) positon[2,0], (float) positon[2,1]),
                new Point2f((float) positon[3,0], (float) positon[3,1]),
               
           
             
         
 
            };
            float[] size = setSize(positon);
            //变换后的四点
            Point2f[] dstPoints = new Point2f[]
            {
                new Point2f(0, 0),
                new Point2f(0, size[1]),
                new Point2f( size[0],  size[1]),
                new Point2f( size[0], 0),
   
        
            };
       
            //根据变换前后四个点坐标,获取变换矩阵
            Mat mm = Cv2.GetPerspectiveTransform(srcPoints, dstPoints);
            Size size1 = new Size(size[0],size[1]);
            
            //进行透视变换
            Cv2.WarpPerspective(src, dst, mm, size1);
        }

        private static float[] setSize(double[,] positon)
        {
            float[] size = new float[] {0, 0};
            size[0] = (float) (positon[3, 0] - positon[0, 0]);
            size[1] = (float)(positon[1, 1] - positon[0, 1]);
            return size;

        }
        // ImageSource --> Bitmap
        public static System.Drawing.Bitmap ImageSourceToBitmap(ImageSource imageSource)
        {
            BitmapSource m = (BitmapSource) imageSource;

            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(m.PixelWidth, m.PixelHeight,
                System.Drawing.Imaging.PixelFormat.Format32bppPArgb); // 坑点：选Format32bppRgb将不带透明度

            System.Drawing.Imaging.BitmapData data = bmp.LockBits(
                new System.Drawing.Rectangle(System.Drawing.Point.Empty, bmp.Size),
                System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            m.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height*data.Stride, data.Stride);
            bmp.UnlockBits(data);

            return bmp;
        }

        // Bitmap --> BitmapImage
        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png); // 坑点：格式选Bmp时，不带透明度

                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                // Force the bitmap to load right now so we can dispose the stream.
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }
        
        // bitmap --> mat
        public static Mat Bitmap2Mat(Bitmap bitmap)
        {
            MemoryStream s2_ms = null;
            Mat source = null;
            try
            {
                using (s2_ms = new MemoryStream())
                {
                    bitmap.Save(s2_ms, ImageFormat.Bmp);
                    source = Mat.FromStream(s2_ms, ImreadModes.AnyColor);
                }
            }
            catch (Exception e)
            {
            }
            finally
            {
                if (s2_ms != null)
                {
                    s2_ms.Close();
                    s2_ms = null;
                }
                GC.Collect();
            }
            return source;
        }

    }
}
