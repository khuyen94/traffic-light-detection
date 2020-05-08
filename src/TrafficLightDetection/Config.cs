using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrafficLightDetection
{
    public partial class Config : Form
    {
        private Point startPos, endPos;
        Image<Bgr, Byte> img2;
        int iScale = 1;
        UC_Video formVideo;
        UC_Camera formCamera;

        public Config(UC_Video playVideoForm, Mat img, RedLightDetector redlight)
        {
            InitializeComponent();
            formVideo = playVideoForm;
            img2 = new Image<Bgr, byte>(img.Bitmap);
            picConfig.Image = img.Bitmap;
            iScale = img2.Width / 480;
            txtXRedLight.Text = redlight.RedPoint.X + "";
            txtYRedLight.Text = redlight.RedPoint.Y + "";
            txtXYellowLight.Text = redlight.YellowPoint.X + "";
            txtYYellowLight.Text = redlight.YellowPoint.Y + "";
            txtXGreenLight.Text = redlight.GreenPoint.X + "";
            txtYGreenLight.Text = redlight.GreenPoint.Y + "";
            txtWidthLight.Text = redlight.WidthPixel + "";
            txtThreshold.Text = redlight.Threshold + "";
        }

        public Config(UC_Camera playVideoForm, Mat img, RedLightDetector redlight)
        {
            InitializeComponent();
            formCamera = playVideoForm;
            img2 = new Image<Bgr, byte>(img.Bitmap);
            picConfig.Image = img.Bitmap;
            iScale = img2.Width / 480;
            txtXRedLight.Text = redlight.RedPoint.X + "";
            txtYRedLight.Text = redlight.RedPoint.Y + "";
            txtXYellowLight.Text = redlight.YellowPoint.X + "";
            txtYYellowLight.Text = redlight.YellowPoint.Y + "";
            txtXGreenLight.Text = redlight.GreenPoint.X + "";
            txtYGreenLight.Text = redlight.GreenPoint.Y + "";
        }

        public Boolean Check()
        {
            if (string.IsNullOrEmpty(txtXRedLight.Text) || string.IsNullOrEmpty(txtXRedLight.Text) || string.IsNullOrEmpty(txtXYellowLight.Text) || string.IsNullOrEmpty(txtYYellowLight.Text) || string.IsNullOrEmpty(txtXGreenLight.Text) || string.IsNullOrEmpty(txtYGreenLight.Text))
            {
                MessageBox.Show("Không được để trống vị trí của đèn ");
                return false;
            }
            else
                if (string.IsNullOrEmpty(txtWidthLight.Text))
                {
                    MessageBox.Show("Không được để trống độ rộng của đèn");
                    return false;
                }
                else
                    if (string.IsNullOrEmpty(txtThreshold.Text))
                    {
                        MessageBox.Show("Không được để trống ngưỡng sáng");
                        return false;
                    }
            return true;
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            if (Check())
            {
                if (formVideo != null)
                {
                    formVideo.redlight.RedPoint = new Point(Int32.Parse(txtXRedLight.Text), Int32.Parse(txtYRedLight.Text));
                    formVideo.redlight.YellowPoint = new Point(Int32.Parse(txtXYellowLight.Text), Int32.Parse(txtYYellowLight.Text));
                    formVideo.redlight.GreenPoint = new Point(Int32.Parse(txtXGreenLight.Text), Int32.Parse(txtYGreenLight.Text));
                    formVideo.redlight.Threshold = Int32.Parse(txtThreshold.Text);
                    formVideo.redlight.WidthPixel = Int32.Parse(txtWidthLight.Text);
                }
                else
                {
                    formCamera.redlight.RedPoint = new Point(Int32.Parse(txtXRedLight.Text), Int32.Parse(txtYRedLight.Text));
                    formCamera.redlight.YellowPoint = new Point(Int32.Parse(txtXYellowLight.Text), Int32.Parse(txtYYellowLight.Text));
                    formCamera.redlight.GreenPoint = new Point(Int32.Parse(txtXGreenLight.Text), Int32.Parse(txtYGreenLight.Text));
                    formCamera.redlight.Threshold = Int32.Parse(txtThreshold.Text);
                    formCamera.redlight.WidthPixel = Int32.Parse(txtWidthLight.Text);
                }
                this.Dispose();
            }
        }


        private void picConfig_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                startPos.X = e.X*iScale;
                startPos.Y = e.Y*iScale;
            }
        }

        private void picConfig_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Image<Bgr, Byte> img = new Image<Bgr, byte>(img2.Bitmap);
                endPos.X = e.X*iScale;
                endPos.Y = e.Y*iScale;
                Point width = new Point((endPos.X - startPos.X), (endPos.Y - startPos.Y));
                #region calculate and draw circle
                //int R = (int)Math.Sqrt(((width.X * width.X) + (width.Y * width.Y)))/2;
                //Point center = new Point();
                //center.X = startPos.X + R;
                //center.Y = startPos.Y + R;
                //txtWidthLight.Text = width.X + "";
                //CvInvoke.Circle(img, center, R, new Bgr(Color.Red).MCvScalar, 2);
                #endregion
                CvInvoke.Rectangle(img, new Rectangle(startPos, new Size(width.X, width.Y)), new Bgr(Color.Red).MCvScalar, 2);
                picConfig.Image = img.Bitmap;
            }
        }

        private void btnRedlight_Click(object sender, EventArgs e)
        {
            txtXRedLight.Text = startPos.X.ToString();
            txtYRedLight.Text = startPos.Y.ToString();
        }

        private void btnYellowLight_Click(object sender, EventArgs e)
        {
            txtXYellowLight.Text = startPos.X.ToString();
            txtYYellowLight.Text = startPos.Y.ToString();
        }

        private void btnGreenLight_Click(object sender, EventArgs e)
        {
            txtXGreenLight.Text = startPos.X.ToString();
            txtYGreenLight.Text = startPos.Y.ToString();
        }
    }
}
