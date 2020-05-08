namespace TrafficLightDetection
{
    partial class UC_Camera
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grbIpCamera = new System.Windows.Forms.GroupBox();
            this.txtCameraAxisIp = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.btnOpen = new System.Windows.Forms.Button();
            this.pictureBoxStatus = new System.Windows.Forms.PictureBox();
            this.btnConfig = new System.Windows.Forms.Button();
            this.vspCameraAxis = new AForge.Controls.VideoSourcePlayer();
            this.grbIpCamera.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // grbIpCamera
            // 
            this.grbIpCamera.Controls.Add(this.txtCameraAxisIp);
            this.grbIpCamera.Controls.Add(this.label11);
            this.grbIpCamera.Location = new System.Drawing.Point(592, 36);
            this.grbIpCamera.Name = "grbIpCamera";
            this.grbIpCamera.Size = new System.Drawing.Size(170, 121);
            this.grbIpCamera.TabIndex = 34;
            this.grbIpCamera.TabStop = false;
            this.grbIpCamera.Text = "Config IP Camera";
            // 
            // txtCameraAxisIp
            // 
            this.txtCameraAxisIp.Location = new System.Drawing.Point(28, 70);
            this.txtCameraAxisIp.Name = "txtCameraAxisIp";
            this.txtCameraAxisIp.Size = new System.Drawing.Size(100, 20);
            this.txtCameraAxisIp.TabIndex = 1;
            this.txtCameraAxisIp.Text = "192.0.2.10";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(25, 35);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(56, 13);
            this.label11.TabIndex = 0;
            this.label11.Text = "Camera IP";
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(603, 200);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(75, 23);
            this.btnOpen.TabIndex = 35;
            this.btnOpen.Text = "Play";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // pictureBoxStatus
            // 
            this.pictureBoxStatus.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pictureBoxStatus.Location = new System.Drawing.Point(620, 270);
            this.pictureBoxStatus.Name = "pictureBoxStatus";
            this.pictureBoxStatus.Size = new System.Drawing.Size(110, 50);
            this.pictureBoxStatus.TabIndex = 52;
            this.pictureBoxStatus.TabStop = false;
            // 
            // btnConfig
            // 
            this.btnConfig.Location = new System.Drawing.Point(687, 200);
            this.btnConfig.Name = "btnConfig";
            this.btnConfig.Size = new System.Drawing.Size(75, 23);
            this.btnConfig.TabIndex = 53;
            this.btnConfig.Text = "Config";
            this.btnConfig.UseVisualStyleBackColor = true;
            this.btnConfig.Click += new System.EventHandler(this.btnConfig_Click);
            // 
            // vspCameraAxis
            // 
            this.vspCameraAxis.Location = new System.Drawing.Point(17, 36);
            this.vspCameraAxis.Name = "vspCameraAxis";
            this.vspCameraAxis.Size = new System.Drawing.Size(547, 315);
            this.vspCameraAxis.TabIndex = 54;
            this.vspCameraAxis.Text = "videoSourcePlayer1";
            this.vspCameraAxis.VideoSource = null;
            this.vspCameraAxis.NewFrame += new AForge.Controls.VideoSourcePlayer.NewFrameHandler(this.vspCameraAxis_NewFrame);
            // 
            // UC_Camera
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.vspCameraAxis);
            this.Controls.Add(this.btnConfig);
            this.Controls.Add(this.pictureBoxStatus);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.grbIpCamera);
            this.Name = "UC_Camera";
            this.Size = new System.Drawing.Size(798, 372);
            this.grbIpCamera.ResumeLayout(false);
            this.grbIpCamera.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStatus)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grbIpCamera;
        private System.Windows.Forms.TextBox txtCameraAxisIp;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.PictureBox pictureBoxStatus;
        private System.Windows.Forms.Button btnConfig;
        private AForge.Controls.VideoSourcePlayer vspCameraAxis;
    }
}
