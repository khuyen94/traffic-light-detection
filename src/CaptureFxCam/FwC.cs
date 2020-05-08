using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Net;
using System.IO;
using System.Threading;

namespace CaptureFxCam
{
    public class RequestState
    {
        // This class stores the State of the request. 
        const int BUFFER_SIZE = 1024;
        public StringBuilder requestData;
        public byte[] BufferRead;
        public HttpWebRequest request;
        public HttpWebResponse response;
        public Stream streamResponse;
        public RequestState()
        {
            BufferRead = new byte[BUFFER_SIZE];
            requestData = new StringBuilder("");
            request = null;
            streamResponse = null;
        }
    }

   public class FwC
    {
        #region Fields
        /// <summary>
        /// Định dạng file hình
        /// </summary>
        public const string FILE_IMAGE_EXT = ".jpg";
  
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        const int BUFFER_SIZE = 1024;
        const int DefaultTimeout = 2 * 60 * 1000; // 2 minutes timeout 
        /// <summary>
        /// url đến camera ex: htt:\\192.168.2.221
        /// </summary>
        public string strUrlCameraIP;

        /// <summary>
        /// Lỗi trả về
        /// </summary>
        public string alertMess = "";

        /// <summary>
        /// Thư mục gốc 
        /// </summary>
        public string RootFolder ;//= System.Environment.CurrentDirectory;//ConfigurationSettings.AppSettings["PathSaveImg"];//Directory.GetCurrentDirectory();//

        //string appPath = Path.GetDirectoryName(Application.ExecutablePath);

        // Đường dẫn lưu vào DB
        string PathSaveDB = string.Empty;


        #endregion

        #region DownloadImageFromUrl
        /// <summary>
        /// Dowload file form CFServer
        /// </summary>
        /// <param name="strURL">ex: http://192.168.2.221/cfserver.fcgi?cmd=getimage&id=1344311451138&type=normal </param>
        /// <returns></returns>
        public Image DownloadImageFromUrl(string strURL, ref string exMess)
        {
            Image imagesDowload = null;
            HttpWebRequest ImageWebRequest;
            HttpWebResponse ImageWebResponse = null;
            Stream responseStream = null;
            try
            {
                ImageWebRequest = (HttpWebRequest)WebRequest.Create(strURL);
                ImageWebRequest.AllowWriteStreamBuffering = true;
                ImageWebRequest.Timeout = 3000;

                ImageWebResponse = (HttpWebResponse)ImageWebRequest.GetResponse();
                responseStream = ImageWebResponse.GetResponseStream();

                imagesDowload = Image.FromStream(responseStream);

                ImageWebResponse.Close();

            }
            catch (Exception ex)
            {
                exMess += ex.Message;
                exMess += "\n";
                return null;
            }
            finally
            {
                //Close connections

                //Release objects
                ImageWebRequest = null;
                ImageWebResponse = null;
                responseStream = null;
            }
            return imagesDowload;
        }
        #endregion

        public string CaptureImageFWC(string pFileName, string pLine1, string pLine2, string localPath, string RootPath, string Url)
        {
            int retValue = 1;
            string fileTemp = string.Empty; // Duong dan file temp  
            string DefaultFileName = "NoImageND.jpg";
            RootFolder = RootPath;
            Image img = null;
            try
            {
                //string imgUrl = this.strUrlCameraIP + "/scapture"; 
                //Tải hình về
                img = this.DownloadImageFromUrl(Url, ref alertMess);
            }
            catch (Exception ex)
            {
                LogUtility.WriteLogFile(this.GetType() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Can not capture" + ex.ToString()); 
                string curDir = System.IO.Directory.GetCurrentDirectory(); 
                retValue = 0;
            }
        SaveImgAgain:
            try
            {
                // IntPtr handle=( IntPtr ) mImage._get_handle();will be tested
                //sau đó lưu xuống thư mục chính
                string subfolder = string.Empty;
                //20090428150022_001
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

                    if (System.IO.Directory.Exists(path))
                    {
                        string curDir = System.IO.Directory.GetCurrentDirectory();
                        fileTemp = curDir + @"\" + DefaultFileName;
                        if (System.IO.File.Exists(fileTemp)&& img != null)
                        {
                            img.Save(fileTemp);
                            img.Save(path + @"\" + pFileName + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                            //doc file image len va write text     
                            //using (Image tempPic = Image.FromFile(fileTemp))    //chỗ này nếu ko chụp dc hình thì phải tạo 1 ảnh tạm
                            //{
                            //    Bitmap tmp = new Bitmap(tempPic);
                            //    //Graphics grp = Graphics.FromImage(ImageImage.FromHbitmap(handle));
                            //    Graphics grp = Graphics.FromImage(tmp);
                            //    //Font f = new Font("Arial", 25F);
                            //    //SolidBrush whiteBrush = new SolidBrush(Color.Red);
                            //    //SolidBrush blackBrush = new SolidBrush(Color.White);
                            //    //grp.DrawString(pLine1, f, whiteBrush, 10, 10);
                            //    //grp.DrawString(pLine2, f, whiteBrush, 10, 70);
                            //    Font font = new Font("Arial", 25F);
                            //    SolidBrush whiteBrush = new SolidBrush(Color.Red);
                            //    SolidBrush blackBrush = new SolidBrush(Color.White);
                            //    grp.DrawString(pLine1, font, whiteBrush, 5, 10);
                            //    grp.DrawString(pLine2, font, whiteBrush, 5, 40);
                            //    tmp.Save(path + @"\" + pFileName + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                            //}
                        }
                        else
                        {
                            LogUtility.WriteLogFile(this.GetType().ToString(), "Not exists file:" + fileTemp);
                        }
                    }
                    else
                    {
                        LogUtility.WriteLogFile(this.GetType() + ".CaptureImage2:", "Cannot create folder:" + path);
                    }
                }
                else
                {
                    LogUtility.WriteLogFile(this.GetType() + ".CaptureImage2:" + pFileName, "Directory " + RootFolder + " doesn't exist");
                }
            }
            catch (Exception ex)
            {
                LogUtility.WriteLogFile(this.GetType() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
                //string curDir = System.IO.Directory.GetCurrentDirectory();
                //DefaultFileName = fileTemp = curDir + @"\" + "NoImage.jpg";
                RootFolder = localPath;
                LogUtility.WriteLogFile(this.GetType() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, "Try save image on local");
                goto SaveImgAgain;
                // retValue = 0;
            }
            return retValue.ToString();
        }

        public string FreewayCAMSaveFile()
        {
            //Tạo đường dẫn đến file image
            string imgUrl = this.strUrlCameraIP + "/scapture";
            string pTransactionID = DateTime.Now.ToString();
            string pLaneCode = "001";
            //Tải hình về
            Image img = this.DownloadImageFromUrl(imgUrl, ref alertMess);
            if (img != null)
            {
                //lấy thời gian khi chụp hình để tạo folder lưu hình
                //string getTimeImageByID = this.strUrlCameraIP + "/cfserver.fcgi?cmd=getdata" + "&id=" + _ID;

                if (!System.IO.Directory.Exists(RootFolder))
                {
                    try
                    {
                        System.IO.Directory.CreateDirectory(RootFolder);
                    }
                    catch (Exception ex)
                    {
                        // Utility.Log.WriteLogFile("SaveFile - GetImagesFromCFServer.cs", "Tạo folder bị Lỗi: " + RootFolder + " -" + ex.ToString());   
                    }
                }
                //kiểm tra pTransactionID truyền vào phải đủ ngày giờ phut giây để đảm bảo tên hình ko trùng nhau
                if (pTransactionID.Length < 14)
                {
                    //write log file: pTransactionID ko đúng định dạng
                    // Utility.Log.WriteLogFile("SaveFile - GetImagesFromCFServer.cs", "pTransactionID ko đúng định dạng: " + pTransactionID);  
                    //lưu hình tạo vào rootfolder tránh bị mất hình
                    string tempFileName = "Images/" + pTransactionID + "_" + FILE_IMAGE_EXT;// pLaneCode + "_" + ID + FILE_IMAGE_EXT;
                    try
                    {
                        img.Save(RootFolder + @"/" + tempFileName);
                    }
                    catch (Exception ex)
                    {
                        alertMess += ex.Message;
                        return null;
                    }
                    return tempFileName;
                }
                //Tạo đường dẫn theo TransactionID và LaneCode
                string PathByTL = "Images";
                PathByTL += @"/" + pTransactionID.Substring(0, 4);
                PathByTL += @"/" + pTransactionID.Substring(4, 2);
                PathByTL += @"/" + pTransactionID.Substring(6, 2);
                PathByTL += @"/" + pTransactionID.Substring(8, 2);
                PathByTL += @"/" + "pLaneCode";
                //Tạo folder chứa hình
                string PathSaveImage = RootFolder + @"/" + PathByTL;
                if (!System.IO.Directory.Exists(PathSaveImage))
                {
                    try
                    {
                        Directory.CreateDirectory(PathSaveImage);
                    }
                    catch (Exception ex)
                    {
                        //write log file
                        //  Utility.Log.WriteLogFile("SaveFile - GetImagesFromCFServer.cs", "Tạo folder chứa hình lỗi: " + PathSaveImage +" -" + ex.ToString());                 
                    }
                }

                if (System.IO.Directory.Exists(PathSaveImage))
                {
                    //tạo tên hình
                    string FileNameImage = pTransactionID + "_" + pLaneCode + "_" + "1" + FILE_IMAGE_EXT;
                    // ImgName = FileNameImage;
                    PathSaveDB = PathByTL + @"/" + FileNameImage;
                    // Đường dẫn lưu hình
                    string FullPath = PathSaveImage + @"/" + FileNameImage;
                    //kiểm tra xem có chọn mode kiểm tra file đã được ghi hay chưa ko
                    //if (CheckExist == true)
                    //{
                    //    //kiểm tra nếu file đã tồn tại thì return null ngược lại thì lưu file
                    //    if (File.Exists(FullPath) == true) return null;
                    //}
                    try
                    {
                        img.Save(FullPath);
                    }
                    catch (Exception ex)
                    {
                        alertMess += ex.Message;
                        return null;
                    }
                }
                return PathSaveDB;
            }
            else
            {
                //write log không lấy được hình
                // Utility.Log.WriteLogFile("SaveFile - GetImagesFromCFServer.cs", "Không lấy được hình từ SpeedCAM "+this.strUrlCameraIP+"(Lỗi: " + alertMess + ")");                 

                return null;
            }
        }

        public string GetResult()
        {
            //http://192.168.2.221/cfserver.fcgi?cmd=getresultlist&begin_date=2012.08.10%10.10.10&end_date=2012.08.10%10.10.10
            // string url = @"http://192.0.2.3/trigger/gpiotrigger?getall&wfilter=1";
            using (WebClient client = new WebClient())
            {
                string htmlCode = client.DownloadString("http://192.0.2.3/trigger/gpiotrigger?getall&wfilter=1");
                int index = htmlCode.IndexOf("\ngpin=");
                string value = htmlCode[index + 6].ToString();
                return value;
            }

        }

        // Abort the request if the timer fires. 
        private static void TimeoutCallback(object state, bool timedOut)
        {
            if (timedOut)
            {
                HttpWebRequest request = state as HttpWebRequest;
                if (request != null)
                {
                    request.Abort();
                }
            }
        }

        public static void CreateTaskListening(string url)
        {
            try
            {
                // Create a HttpWebrequest object to the desired URL. 
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                myHttpWebRequest.Accept = "text/xml,text/plain,text/html";
                myHttpWebRequest.Method = "GET";
               
                RequestState myRequestState = new RequestState();
                myRequestState.request = myHttpWebRequest;

                // Start the asynchronous request.
                IAsyncResult result =
                  (IAsyncResult)myHttpWebRequest.BeginGetResponse(new AsyncCallback(RespCallback), myRequestState);

                // this line implements the timeout, if there is a timeout, the callback fires and the request becomes aborted
                ThreadPool.RegisterWaitForSingleObject(result.AsyncWaitHandle, new WaitOrTimerCallback(TimeoutCallback), myHttpWebRequest, DefaultTimeout, true);

                // The response came in the allowed time. The work processing will happen in the  
                // callback function.
                allDone.WaitOne(new TimeSpan());

                // Release the HttpWebResponse resource.
                myRequestState.response.Close();
            }
            catch (WebException e)
            {
                Console.WriteLine("\nMain Exception raised!");
                Console.WriteLine("\nMessage:{0}", e.Message);
                Console.WriteLine("\nStatus:{0}", e.Status);
                Console.WriteLine("Press any key to continue..........");
            }
            catch (Exception e)
            {
                Console.WriteLine("\nMain Exception raised!");
                Console.WriteLine("Source :{0} ", e.Source);
                Console.WriteLine("Message :{0} ", e.Message);
                Console.WriteLine("Press any key to continue..........");
                Console.Read();
            }
        }

        private static void RespCallback(IAsyncResult asynchronousResult)
        {
            try
            {
                // State of request is asynchronous.
                RequestState myRequestState = (RequestState)asynchronousResult.AsyncState;
                HttpWebRequest myHttpWebRequest = myRequestState.request;
                myRequestState.response = (HttpWebResponse)myHttpWebRequest.EndGetResponse(asynchronousResult);

                // Read the response into a Stream object.
                Stream responseStream = myRequestState.response.GetResponseStream();
                myRequestState.streamResponse = responseStream;

                // Begin the Reading of the contents of the HTML page and print it to the console.
                IAsyncResult asynchronousInputRead = responseStream.BeginRead(myRequestState.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), myRequestState);
                return;
            }
            catch (WebException e)
            {
                Console.WriteLine("\nRespCallback Exception raised!");
                Console.WriteLine("\nMessage:{0}", e.Message);
                Console.WriteLine("\nStatus:{0}", e.Status);
            }
            allDone.Set();
        }
        private static void ReadCallBack(IAsyncResult asyncResult)
        {
            try
            {

                RequestState myRequestState = (RequestState)asyncResult.AsyncState;
                Stream responseStream = myRequestState.streamResponse;
                int read = responseStream.EndRead(asyncResult);
                // Read the HTML page and then print it to the console. 
                if (read > 0)
                {
                    myRequestState.requestData.Append(Encoding.ASCII.GetString(myRequestState.BufferRead, 0, read));
                    IAsyncResult asynchronousResult = responseStream.BeginRead(myRequestState.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), myRequestState);
                    return;
                }
                else
                {
                    Console.WriteLine("\nThe contents of the Html page are : ");
                    if (myRequestState.requestData.Length > 1)
                    {
                        string stringContent;
                        stringContent = myRequestState.requestData.ToString();
                        Console.WriteLine(stringContent);
                    }
                    Console.WriteLine("Press any key to continue..........");
                    Console.ReadLine();

                    responseStream.Close();
                }

            }
            catch (WebException e)
            {
                Console.WriteLine("\nReadCallBack Exception raised!");
                Console.WriteLine("\nMessage:{0}", e.Message);
                Console.WriteLine("\nStatus:{0}", e.Status);
            }
            allDone.Set();

        }     

        private static Stream CopyAndClose(Stream inputStream)
        {
            const int readSize = 256;
            byte[] buffer = new byte[readSize];
            MemoryStream ms = new MemoryStream();

            int count = inputStream.Read(buffer, 0, readSize);
            while (count > 0)
            {
                ms.Write(buffer, 0, count);
                count = inputStream.Read(buffer, 0, readSize);
            }
            ms.Position = 0;
            inputStream.Close();
            return ms;
        }
       /// <summary>
       /// Lay trang thay GPin cua FreewayCAM
       /// </summary>
        /// <param name="URL">http://192.0.2.3/trigger/gpiotrigger?getall&wfilter=1</param>
       /// <returns></returns>
        public static bool CheckStatusFWC(string url)
        {

            bool flag = false;
            HttpWebResponse result;
           
            try
            {

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Accept = "text/xml,text/plain,text/html";
                req.Method = "GET";
                req.Proxy = null;

                result = (HttpWebResponse)req.GetResponse();
                Stream ReceiveStream = result.GetResponseStream();
                // Do something with the stream
               
                StreamReader reader = new StreamReader(ReceiveStream, System.Text.Encoding.ASCII);
                string respHTML = reader.ReadToEnd();
                reader.Close();                
                if (respHTML.IndexOf("gpin=1") == 0)
                    flag = true;
                                
                ReceiveStream.Close();                
                result.Close();
                req.Abort();
                
            }
            catch (Exception ex)
            {
                LogUtility.WriteLogFile(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return flag;
           
        }
        //"sections=default,output_0\r\ngpin=0\r\ngpout=0\r\nhldelay=0\r\nlhdelay=0\r\nact_level=1\r\nreqsamples=100\r\nsamplerate=10000\r\ntarget0=trigger/eventman\r\nresolution=2\r\nsendtimeout=100\r\nntargets=1\r\nnoutputs=1\r\nmimetype=text/plain\r\n"

    }
}
