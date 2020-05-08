using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Video.DirectShow;
using AForge.Video;
using System.Timers;
using Emgu.CV.CvEnum;
using System.Diagnostics;

namespace TrafficLightDetection
{
    /// <summary>
    /// Lop xu ly viec do tin hieu den do
    /// </summary>
    /// 

    public class RedLightDetector
    {
        #region Properties
        //Tu dong bat trang thai change
        /// <summary>
        /// Vị trí xác nhận đèn đỏ
        /// </summary>
        /// 
        private Point _redPoint;

        public Point RedPoint
        {
            get { return _redPoint; }
            set { _redPoint = value; }
        }

        private Point _yellowPoint;

        public Point YellowPoint
        {
            get { return _yellowPoint; }
            set { _yellowPoint = value; }
        }

        private Point _greenPoint;

        public Point GreenPoint
        {
            get { return _greenPoint; }
            set { _greenPoint = value; }
        }

        private static Image<Bgr, Byte> _originImage;

        public Image<Bgr, Byte> OriginImage
        {
            get { return _originImage; }
            set { _originImage = value; }
        }

        private int _widthPixel;

        public int WidthPixel
        {
            get { return _widthPixel; }
            set { _widthPixel = value; }
        }

        private int _threshold;

        public int Threshold
        {
            get { return _threshold; }
            set { _threshold = value; }
        }
        /// <summary>
        /// Địa chỉ camera TC
        /// </summary>
        public string OverviewCameraIP { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Kiểm tra có là đèn đỏ
        /// True: on
        /// False: off
        /// </summary>
        ///

        //public bool CheckRedLightStatus()
        //{
        //    //GetPicSnap();
        //    if (OriginImage == null)
        //    {
        //        CheckRedLightStatus();
        //    }
        //    //Image<Gray, Byte> grayImg = new Image<Gray, byte>(BitMap.Bitmap);
        //    Mat grayImg = new Mat();
        //    CvInvoke.CvtColor(OriginImage, grayImg, ColorConversion.Bgr2Gray);

        //    Bitmap img = grayImg.Bitmap;
        //    int R = img.GetPixel(X, Y).R;
        //    int xTemp1, xTemp2, xTemp3, xTemp4;
        //    int yTemp1, yTemp2, yTemp3, yTemp4;
        //    xTemp1 = xTemp2 = xTemp3 = xTemp4 = X;
        //    yTemp1 = yTemp2 = yTemp3 = yTemp4 = Y;
        //    int widthPixel = 3; //Lấy trung bình của bao nhiêu pixel từ điểm trung tâm


        //    for (int i = 0; i < widthPixel; i++)
        //    {
        //        xTemp1 += 1;
        //        xTemp2 -= 1;
        //        xTemp3 -= 1;
        //        xTemp4 += 1;
        //        for (int j = 0; j < widthPixel; j++)
        //        {
        //            yTemp1 += 1;
        //            yTemp2 += 1;
        //            yTemp3 -= 1;
        //            yTemp4 -= 1;
        //            Color pixelColor1, pixelColor2, pixelColor3, pixelColor4;
        //            pixelColor1 = img.GetPixel(xTemp1, yTemp1);
        //            pixelColor2 = img.GetPixel(xTemp2, yTemp2);
        //            pixelColor3 = img.GetPixel(xTemp3, yTemp3);
        //            pixelColor4 = img.GetPixel(xTemp4, yTemp4);
        //            R += (int)(pixelColor1.R + pixelColor2.R + pixelColor3.R + pixelColor4.R);
        //        }
        //    }


        //    int avgR = (R / ((widthPixel * widthPixel * 4) + 1));

        //    if ((avgR > 200))
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        /// <summary>
        /// Check red light status.
        /// Return: 1, 2, 3 via red, yellow, green status.
        /// </summary>
        /// <returns>1, 2, 3 via red, yellow, green status.</returns>
        public int CheckLightStatusV3()
        {
            int statusLight = 0;
            bool redResult = false, yellowResult = false, greenResult = false;
            Image<Bgr, Byte> redImg = OriginImage.Clone();
            Image<Bgr, Byte> greenImg = OriginImage.Clone();
            Image<Bgr, Byte> yellowImg = OriginImage.Clone();
            redImg.ROI = new Rectangle(RedPoint.X, RedPoint.Y, WidthPixel, WidthPixel); 
            yellowImg.ROI = new Rectangle(YellowPoint.X, YellowPoint.Y, WidthPixel, WidthPixel);
            greenImg.ROI = new Rectangle(GreenPoint.X, GreenPoint.Y, WidthPixel, WidthPixel);

            Image<Gray, Byte> redProcessed = processImage(redImg);
            Image<Gray, Byte> yellowProcessed = processImage(yellowImg);
            Image<Gray, Byte> greenProcessed = processImage(greenImg);

            double redValue = redProcessed.GetAverage().MCvScalar.V0;
            double yellowValue = yellowProcessed.GetAverage().MCvScalar.V0;
            double greenValue = greenProcessed.GetAverage().MCvScalar.V0;

            if (redProcessed.GetAverage().MCvScalar.V0 >= Threshold)
            {
                redResult = true;
            }
            if (yellowProcessed.GetAverage().MCvScalar.V0 >= Threshold)
            {
                yellowResult = true;
            }
            if (greenProcessed.GetAverage().MCvScalar.V0 >= Threshold)
            {
                greenResult = true;
            }

            if (redResult == true && yellowResult == false && greenResult == false)
            {
                statusLight = 1;
            }
            else if (redResult == false && yellowResult == true && greenResult == false)
            {
                statusLight = 2;
            }
            else if (redResult == false && yellowResult == false && greenResult == true)
            {
                statusLight = 3;
            }
            else
            {
                statusLight = 0;
            }
            return statusLight;
        }
        private Image<Gray,Byte> processImage(Image<Bgr, Byte> image)
        {
            Mat hsvImg = new Mat();
            CvInvoke.CvtColor(image, hsvImg, ColorConversion.Bgr2Hsv);
            //Threshold image, keep only the red pixel
            Mat lower_hue_range = new Mat();
            Mat upper_hue_range = new Mat();

            CvInvoke.InRange(hsvImg, new ScalarArray(new MCvScalar(0, 100, 30)), new ScalarArray(new MCvScalar(80, 255, 255)), lower_hue_range); //80(multi color) -> 10(only red)
            CvInvoke.InRange(hsvImg, new ScalarArray(new MCvScalar(160, 100, 30)), new ScalarArray(new MCvScalar(179, 255, 255)), upper_hue_range);
            //// Combine the above two images
            CvInvoke.AddWeighted(lower_hue_range, 1.0, upper_hue_range, 1.0, 0.0, hsvImg);
            //CvInvoke.MedianBlur(hue_image, hue_image, 9);
            return new Image<Gray, Byte>(hsvImg.Bitmap);
        }
        #endregion
    }
}
