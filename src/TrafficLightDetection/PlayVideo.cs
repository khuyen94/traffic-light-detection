using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using AForge.Video;

namespace TrafficLightDetection
{
    public partial class PlayVideo : Form
    {
        UC_Video UCVideo = new UC_Video();
        UC_Camera UCCamera = new UC_Camera();

        public PlayVideo()
        {
            InitializeComponent();
        }

        private void videoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Controls.Remove(UCCamera);
            UCCamera.CloseVideoSource();
            this.Controls.Add(UCVideo);
        }

        private void cameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Controls.Remove(UCVideo);
            UCVideo.CloseVideo();
            this.Controls.Add(UCCamera);
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void PlayVideo_FormClosed(object sender, FormClosedEventArgs e)
        {
            UCCamera.CloseVideoSource();
        }
        
    }
}
