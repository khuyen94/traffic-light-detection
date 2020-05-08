using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            label1.Text = "Not Red";
        }

        private void button1_Click(object sender, EventArgs e)
        {
        //        //Load the image from file and resize it for display
        //    Image<Bgr, Byte> img =
        //       new Image<Bgr, byte>(imageBox1.Image.Bitmap);

        //        //Convert the image to grayscale and filter out the noise
        //        UMat uimage = new UMat();
        //        CvInvoke.CvtColor(img, uimage, ColorConversion.Bgr2Gray);

        //        //use image pyr to remove noise
        //        UMat pyrDown = new UMat();
        //        CvInvoke.PyrDown(uimage, pyrDown);
        //        CvInvoke.PyrUp(pyrDown, uimage);


        //        //Image<Gray, Byte> gray = img.Convert<Gray, Byte>().PyrDown().PyrUp();

        //        #region circle detection
        //        Stopwatch watch = Stopwatch.StartNew();
        //        double cannyThreshold = 100;// uimage.Rows / 8; //200
        //        double circleAccumulatorThreshold = 50;// 100; //100
        //        CircleF[] circles = CvInvoke.HoughCircles(uimage, HoughType.Gradient, 1.0, 50.0, cannyThreshold, circleAccumulatorThreshold,5);//1.0,50.0 -> 2.0,20.0


        //        watch.Stop();
        //        //msgBuilder.Append(String.Format("Hough circles - {0} ms; ", watch.ElapsedMilliseconds));
        //        #endregion

        //        #region draw circles
        //        Mat circleImage = new Mat(img.Size, DepthType.Cv8U, 3);
        //        circleImage.SetTo(new MCvScalar(0));
        //        foreach (CircleF circle in circles)
        //            CvInvoke.Circle(circleImage, Point.Round(circle.Center), (int)circle.Radius, new Bgr(Color.Brown).MCvScalar, 2);

        //        imageBox2.Image = circleImage;
        //        #endregion
        //        if (circles.Length > 0)
        //        {
        //            foreach(CircleF circle in circles){
        //                if (uimage.ToMat(AccessType.Fast).Bitmap.GetPixel((int)circle.Center.X, (int)circle.Center.Y).R > 250)
        //                {
        //                    label1.Text = "Red";
        //                }
        //            }
        //        }
        }

        private void btnScaleImage_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            DialogResult result = openFileDialog1.ShowDialog();
              if (result == DialogResult.OK || result == DialogResult.Yes)
              {
                  Image<Bgr, Byte> bgrImage =
                   new Image<Bgr, byte>(openFileDialog1.FileName);

                  Mat hsvImage = new Mat();
                  
                  //Convert input to hsv image
                  CvInvoke.CvtColor(bgrImage, hsvImage, ColorConversion.Bgr2Hsv);

                  //Threshold image, keep only the red pixel
                  Mat lower_red_hue_range = new Mat();
                  Mat upper_red_hue_range = new Mat();

                  CvInvoke.InRange(hsvImage, new ScalarArray(new MCvScalar(0, 100, 100)), new ScalarArray(new MCvScalar(180, 255, 255)), lower_red_hue_range); //80(multi color) -> 10(only red)
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
                  {
                      CvInvoke.Circle(bgrImage, Point.Round(circle.Center), (int)circle.Radius, new Bgr(Color.Violet).MCvScalar, 2);
                      
                  }

                  #endregion
                  if (circles.Length > 0)
                  {
                      foreach (CircleF circle in circles)
                      {
                          Byte rValue = red_hue_image.Bitmap.GetPixel((int)circle.Center.X, (int)circle.Center.Y).R;
                          if (rValue > 100 && rValue < 200)
                          {
                              label1.Text += "Green";
                          }
                          else if (rValue > 200 && rValue < 250)
                          {
                              label1.Text += "Yellow";
                          }
                          else if (rValue > 250)
                          {
                              label1.Text += "Red";
                          }
                      }
                  }
                  imageBox1.Image = red_hue_image;
                  imageBox2.Image = bgrImage;
                  //label1.Text = TrafficLightDetector.Detect(bgrImage);
              }
        }
    }
}
