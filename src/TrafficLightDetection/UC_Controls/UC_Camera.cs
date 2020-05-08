using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using AForge.Video;
using Emgu.CV;
using Emgu.CV.Structure;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Video.FFMPEG;

namespace TrafficLightDetection
{
    public partial class UC_Camera : UserControl
    {
        public RedLightDetector redlight = new RedLightDetector();

        public UC_Camera()
        {
            InitializeComponent(); 
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCameraAxisIp.Text))
            {
                try
                {
                    AxisM1145();
                    System.Timers.Timer timer = new System.Timers.Timer();
                    timer.Interval = 100;
                    timer.Elapsed += CheckLightProcessing;
                    timer.Start();
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Không được để trống ip");
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public void AxisM1145()
        {
            MJPEGStream mjpeg = new MJPEGStream("http://" + txtCameraAxisIp.Text + "/axis-cgi/mjpg/video.cgi");
            OpenVideoSource(mjpeg);
        }

        private void OpenVideoSource(IVideoSource source)
        {
            vspCameraAxis.VideoSource = source;
            vspCameraAxis.Start();
            this.Cursor = Cursors.Default;
        }

        public void CloseVideoSource()
        {
            if (vspCameraAxis.IsRunning)
            {
                vspCameraAxis.Stop();
                vspCameraAxis.Dispose();
                this.Dispose();
            }

            vspCameraAxis.BorderColor = Color.Black;
            Cursor = Cursors.Default;
        }

        private void CheckLightProcessing(Object sender, EventArgs e)
        {
                int result = redlight.CheckLightStatusV3();
                switch (result)
                {
                    case 1:
                        pictureBoxStatus.BackColor = Color.Red;
                        break;
                    case 2:
                        pictureBoxStatus.BackColor = Color.Yellow;
                        break;
                    case 3:
                        pictureBoxStatus.BackColor = Color.Green;
                        break;
                    default:
                        pictureBoxStatus.BackColor = Color.Black;
                        break;
                }
                redlight.OriginImage.Dispose();
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            try
            {
                Bitmap bmp = vspCameraAxis.GetCurrentVideoFrame();
                Image<Bgr, Byte> img4 = new Image<Bgr, byte>(vspCameraAxis.GetCurrentVideoFrame());

                Mat img = img4.Mat;
                

                if (img != null)
                {
                    Config form = new Config(this, img, redlight);
                    form.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //MessageBox.Show("Chưa kết nối được camera ");
            }
        }

        private void vspCameraAxis_NewFrame(object sender, ref Bitmap image)
        {
            if (image != null)
            {
                Image<Bgr, Byte> img = new Image<Bgr, byte>(image);
                redlight.OriginImage = img;
                //draw red light
                CvInvoke.Rectangle(img, new Rectangle(redlight.RedPoint, new Size(redlight.WidthPixel, redlight.WidthPixel)), new Bgr(Color.Violet).MCvScalar, 2);
                //draw yellow light
                CvInvoke.Rectangle(img, new Rectangle(redlight.YellowPoint, new Size(redlight.WidthPixel, redlight.WidthPixel)), new Bgr(Color.Violet).MCvScalar, 2);
                //draw green light
                CvInvoke.Rectangle(img, new Rectangle(redlight.GreenPoint, new Size(redlight.WidthPixel, redlight.WidthPixel)), new Bgr(Color.Violet).MCvScalar, 2);
            }
        }
    }
}
