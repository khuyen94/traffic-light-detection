using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Drawing;
using System.Diagnostics;

namespace TrafficLightDetection
{
    public static class TrafficLightDetector
    {
        public static string Detect(Image<Bgr,Byte> bgrImg)
        {
            string result = "Unknown";
            Mat hsvImage = new Mat();

            //Convert input to hsv image
            CvInvoke.CvtColor(bgrImg, hsvImage, ColorConversion.Bgr2Hsv);

            //Threshold image, keep only the red pixel
            Mat lower_red_hue_range = new Mat();
            Mat upper_red_hue_range = new Mat();

            CvInvoke.InRange(hsvImage, new ScalarArray(new MCvScalar(0, 100, 100)), new ScalarArray(new MCvScalar(80, 255, 255)), lower_red_hue_range); //80(multi color) -> 10(only red)
            CvInvoke.InRange(hsvImage, new ScalarArray(new MCvScalar(160, 100, 100)), new ScalarArray(new MCvScalar(179, 255, 255)), upper_red_hue_range);

            //Scale for yellow
            //CvInvoke.InRange(hsvImage, new ScalarArray(new MCvScalar(20, 100, 100)), new ScalarArray(new MCvScalar(30, 255, 255)), upper_red_hue_range);
            //imgHSV, cvScalar(20, 100, 100), cvScalar(30, 255, 255), imgThreshed)


            // Combine the above two images
            Mat red_hue_image = new Mat();
            CvInvoke.AddWeighted(lower_red_hue_range, 1.0, upper_red_hue_range, 1.0, 0.0, red_hue_image);
            CvInvoke.GaussianBlur(red_hue_image, red_hue_image, new Size(9, 9), 2, 2);

            //use image pyr to remove noise
            UMat pyrDown = new UMat();
            CvInvoke.PyrDown(red_hue_image, pyrDown);
            CvInvoke.PyrUp(pyrDown, red_hue_image);


            //Image<Gray, Byte> gray = img.Convert<Gray, Byte>().PyrDown().PyrUp();

            #region circle detection
            Stopwatch watch = Stopwatch.StartNew();
            double cannyThreshold = 350;//red_hue_image.Rows / 8; //200
            double circleAccumulatorThreshold = 80;//100; //100
            CircleF[] circles = CvInvoke.HoughCircles(red_hue_image, HoughType.Gradient, 2.0, 20.0, cannyThreshold, circleAccumulatorThreshold, 5);


            watch.Stop();
            //msgBuilder.Append(String.Format("Hough circles - {0} ms; ", watch.ElapsedMilliseconds));
            #endregion

            #region draw circles
            //Draw new image only circle.
            //Mat circleImage = new Mat(bgrImage.Size, DepthType.Cv8U, 3);
            //circleImage.SetTo(new MCvScalar(0));
            foreach (CircleF circle in circles)
                CvInvoke.Circle(bgrImg, Point.Round(circle.Center), (int)circle.Radius, new Bgr(Color.Violet).MCvScalar, 2);

            #endregion
            if (circles.Length > 0)
            {
                foreach (CircleF circle in circles)
                {
                    Byte redValue = red_hue_image.Bitmap.GetPixel((int)circle.Center.X, (int)circle.Center.Y).R;
                    if (redValue > 100 && redValue < 200)
                    {
                        result = "Green";
                    }
                    else if (redValue > 200 && redValue < 250)
                    {
                        result = "Yellow";
                    }
                    else if (redValue > 250)
                    {
                        result = "Red";
                    }
                }
            }
            return result;
        }
    }
}
