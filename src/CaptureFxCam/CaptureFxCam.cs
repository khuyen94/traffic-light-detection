using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gx;
using System.Threading;
using System.Drawing;
using System.IO;

namespace CaptureFxCam
{
    /// <summary>
    /// Lop quan ly viec chup hinh su dung FxCam
    /// </summary>
    public class CaptureFxCam
    {
        /// <summary>
        /// Đối tượng FX Camera 
        /// </summary>
        private fxCamera mFxCamera;

        /// <summary>
        /// Số ảnh chụp mỗi lần
        /// </summary>
        public int NumPictures = 1;

        /// <summary>
        /// Thư mục chứa hình
        /// </summary>
        public string RootFolder { get; set; }

        /// <summary>
        /// Tên file mặc định khi chụp (lưu tạm)
        /// </summary>
        public string DefaultFileName = "TempRecogImage.jpg";

        /// <summary>
        /// Thời gian chờ để capture image (ms)
        /// </summary>
        public const int DELAY_DURATION = 50;

        /// <summary>
        /// Có khởi tạo cấu hình camera hay không
        /// </summary>
        public const bool IS_CONFIG_CAMERA = false;

        private static string LOG_FILE = "./LogFolder/CaptureLog_{0}.txt";

        public CaptureFxCam()
        {
            mFxCamera = new fxCamera();
        }

        /// <summary>
        /// Khoi tao camera IP
        /// </summary>
        /// <param name="pIP"></param>
        /// <returns>1. OK, 0. Error</returns>
        public int InitCamera(string pIP)
        {
            try
            {
                // Init new object
                //mFxCamera = new fxCamera();      

                mFxCamera.IsValid();

                // Connect to the camera
                mFxCamera.SetProperty("connect/devname", pIP);
                mFxCamera.Connect();

                // Stop capture
                mFxCamera.StopCapture();

                // Turn on automatic synchronization (synchronize the DSP time to PC time)
                // Don't use this if you have NTP synchronization in the camera
                mFxCamera.SetProperty("time/sync", 1);

                // Query camera information
                int img_width = mFxCamera.GetPropertyInt("info/capture/xsize");
                int img_height = mFxCamera.GetPropertyInt("info/capture/ysize");

                if ((img_width == 0) || (img_height == 0))
                {
                    WriteLogFile(this.GetType() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                            string.Format("Unknown camera type: {0}", mFxCamera.GetPropertyInt("info/camera_type")));
                    return 0;
                }

                // Setup camera
                if (IS_CONFIG_CAMERA)
                {
                    mFxCamera.MPStartTransaction();

                    mFxCamera.SetProperty("capture/lights", 0);
                    mFxCamera.SetProperty("capture/again", 100);
                    mFxCamera.SetProperty("capture/dgain", 100);
                    mFxCamera.SetProperty("capture/shutter", 5000);
                    mFxCamera.SetProperty("capture/fps", mFxCamera.GetPropertyInt("info/capture/fps"));
                    mFxCamera.SetProperty("capture/gamma", 1);			// 0: linear, 1: gamma

                    mFxCamera.SetProperty("camera/led_infra", 1);		// 1 = turn on all LEDs (0=off)
                    mFxCamera.SetProperty("camera/led_power", 1);		// normal: 16V
                    mFxCamera.SetProperty("camera/led_timeus", 100);	// 100us

                    mFxCamera.SetProperty("imgproc/shutter/max", 10000);
                    mFxCamera.SetProperty("imgproc/again/max", 200);
                    mFxCamera.SetProperty("capture/color", 0);

                    mFxCamera.MPCommit();
                }

                // Switch to automatic mode 
                mFxCamera.SwitchMode((int)FXCAM_MODES.FXCAM_MODE_AUTOMATIC);

                // Unlock all locked frames if there is any
                mFxCamera.UnlockFrames((ushort)FXCAM_UNLOCK_FLAGS.FXCAM_UNLOCK_RELEASE_ALL, 0, 0);

                return 1;
            }
            catch (Exception ex)
            {
                WriteLogFile(this.GetType() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// Chup hinh nhan dang
        /// </summary>
        /// <param name="pTransactionID"></param>
        /// <param name="pLaneID"></param>
        public int CaptureImage(string pTransactionID, string pLaneID)
        {
            int retValue = 0;

            try
            {
                // Unlock all locked frames if there is any
                //mFxCamera.UnlockFrames((ushort)FXCAM_UNLOCK_FLAGS.FXCAM_UNLOCK_RELEASE_ALL, 0, 0);

                // Start capture
                mFxCamera.StartCapture();

                //Waiting for the end of the camera initialization
                Thread.Sleep(DELAY_DURATION);

                // Wait for valid frame 
                //mFxCamera.GetCaptureInfo();
                int cap_frameix = mFxCamera.GetPropertyInt("captureinfo/newestframeix");

                gxImage image = new gxImage();
                for (int i = 1; i <= NumPictures; i++)
                {
                    // Save image at local folder
                    mFxCamera.GetFrame((int)FXCAM_GETFRAME_FLAGS.FXCAM_GETFRAME_NEWEST, 0, image);
                    if (image.GetPixelFormat() != (int)GX_PIXELFORMATS.GX_GRAY)
                        image.Convert((int)GX_PIXELFORMATS.GX_RGB);	// color image

                    image.Save(DefaultFileName, (int)GX_IMGFILEFORMATS.GX_JPEG);

                    ///TEST LOG
                    //WriteLogFile(this.GetType() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Start save image at server");

                    // Save image at server
                    string pFileName = pTransactionID + "_" + pLaneID + "_" + i.ToString();
                    string subfolder = string.Empty;
                    string[] temp = pFileName.Split('_');
                    string path = string.Empty;
                    if (System.IO.Directory.Exists(RootFolder))
                    {
                        path = RootFolder;
                        path += @"\" + pFileName.Substring(0, 10);
                        path += @"\" + temp[1];//004
                        if (!System.IO.Directory.Exists(path))
                        {
                            //Neu thu muc chua ton tai->tao thu muc do'
                            Directory.CreateDirectory(path);
                        }

                        ///TEST LOG
                        //WriteLogFile(this.GetType() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Check exists recog folder");

                        if (System.IO.Directory.Exists(path))
                        {
                            using (Image tempPic = Image.FromFile(DefaultFileName)) //chỗ này nếu ko chụp dc hình thì phải tạo 1 ảnh tạm     
                            {
                                string pFullPath = path + @"\" + pFileName + ".jpg";
                                tempPic.Save(pFullPath);
                            }
                        }
                        else
                        {
                            WriteLogFile(this.GetType() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Cannot create folder: " + path);
                            return -1; //Khong copy hinh len server duoc
                        }
                    }
                    else
                    {
                        WriteLogFile(this.GetType() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Folder " + RootFolder + " doesn't exist");
                    }
                }
                // Stop capture
                mFxCamera.StopCapture();

                ///TEST LOG
                //WriteLogFile(this.GetType() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Stop Capture");

                retValue = 1;
            }
            catch (Exception ex)
            {
                WriteLogFile(this.GetType() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return retValue;
        }

        public static void WriteLogFile(string strFuncName, string strMsg)
        {
            string strDate = String.Format("{0:yyyy/MM/dd}", DateTime.Now).Replace("/", "");
            string filename = string.Format(LOG_FILE, strDate);

            try
            {
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(filename))
                {
                    string logLine = System.String.Format("{0:G}: {1}.", System.DateTime.Now.ToString("dd/MM/yyy HH:mm:ss:fff"), "[" + strFuncName + " - " + strMsg + "] ");
                    sw.WriteLine(logLine);
                    sw.WriteLine("-------------------------------------------");
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
