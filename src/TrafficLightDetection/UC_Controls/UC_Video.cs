using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using AForge.Video.DirectShow;
using AForge.Video;
using System.Timers;
using System.Diagnostics;
using System.Resources;
using System.Threading;


namespace TrafficLightDetection
{
    public partial class UC_Video : UserControl
    {
        Capture _capture;
        Mat frame;
        Mat frameCheck;
        public RedLightDetector redlight;
        System.Windows.Forms.Timer timer2 = new System.Windows.Forms.Timer();
        int fps;
        public UC_Video()
        {
            InitializeComponent();
        }

        private void ProcessFrame(object sender, EventArgs e)
        {
            if (_capture != null)
            {
                _capture.Retrieve(frame, 0);
                if (frame != null)
                {
                    frameCheck = frame;
                    //draw red light
                    CvInvoke.Rectangle(frame, new Rectangle(redlight.RedPoint, new Size(redlight.WidthPixel, redlight.WidthPixel)), new Bgr(Color.Violet).MCvScalar, 2);
                    //draw yellow light
                    CvInvoke.Rectangle(frame, new Rectangle(redlight.YellowPoint, new Size(redlight.WidthPixel, redlight.WidthPixel)), new Bgr(Color.Violet).MCvScalar, 2);
                    //draw green light
                    CvInvoke.Rectangle(frame, new Rectangle(redlight.GreenPoint, new Size(redlight.WidthPixel, redlight.WidthPixel)), new Bgr(Color.Violet).MCvScalar, 2);
                    imageBoxVideo.Image = frame;
                    Thread.Sleep(1000 / fps);
                }
            }
        }

        private void CheckLightProcessing(object sender, EventArgs e)
        {
            if (frameCheck != null)
            {
                redlight.OriginImage = new Image<Bgr, byte>(frameCheck.Bitmap);
                Stopwatch watch = new Stopwatch();
                watch.Start();
                int result = redlight.CheckLightStatusV3();
                watch.Stop();
                label1.Text = watch.ElapsedMilliseconds + " mls";
                switch (result)
                {
                    case 1:
                        pictureBoxStatus.Image = TrafficLightDetection.Properties.Resources.red;
                        break;
                    case 2:
                        pictureBoxStatus.Image = TrafficLightDetection.Properties.Resources.orange;
                        break;
                    case 3:
                        pictureBoxStatus.Image = TrafficLightDetection.Properties.Resources.green;
                        break;
                    default:
                        pictureBoxStatus.Image = TrafficLightDetection.Properties.Resources.none;
                        break;
                }
            }
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofdVideo = new OpenFileDialog();
            ofdVideo.InitialDirectory = @"E:\g\Media";
            ofdVideo.Filter = "All files(*.*)|*.*|mp4 files(*.mp4)|*.mp4|All files (*.avi)|*.avi";
            ofdVideo.FilterIndex = 1;
            ofdVideo.Multiselect = true;
            ofdVideo.RestoreDirectory = true;

            if (ofdVideo.ShowDialog() == DialogResult.OK)
            {
                _capture = new Capture(ofdVideo.FileName);
                //fps = (int)_capture.GetCaptureProperty(CapProp.Fps);
                fps = 40;
                textBox1.Text = ofdVideo.FileName;
                redlight = new RedLightDetector();
            }
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            try
            {
                Mat img = _capture.QueryFrame();
                if (img != null)
                {
                    Config form = new Config(this, img, redlight);
                    form.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (!("".Equals(textBox1.Text)))
            {
                frameCheck = new Mat();
                frame = new Mat();
                frame = _capture.QueryFrame();
                frameCheck = frame;
                _capture = new Capture(textBox1.Text);
                _capture.ImageGrabbed += ProcessFrame;
                _capture.Start();

                //check light
                timer2.Interval = 5; // in miliseconds
                timer2.Tick += new EventHandler(CheckLightProcessing);
                timer2.Start();
            }
            else
            {
                MessageBox.Show("Chưa chọn video");
            }
        }

        public void CloseVideo()
        {
            if (_capture != null)
            {
                _capture.Dispose();
                frame = null;
                imageBoxVideo = null;
                timer2.Stop();
            }
        }

    }
}
