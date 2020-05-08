using System;
namespace CaptureFxCam
{
    class LogUtility
    {
        #region Fields
        /// <summary>
        /// Log file chung
        /// </summary>
        private static string LOG_FILE = "./LogFolder/LogDevice_{0}.txt";

        #endregion

        /// <summary>
        /// Ham ghi log file chung
        /// </summary>
        /// <param name="strFuncName"></param>
        /// <param name="strMsg"></param>
        public static void WriteLogFile(Exception ex)
        {
//#if DEBUG
//            ShowError(ex);
//#endif
            string strDate = String.Format("{0:yyyy/MM/dd}", DateTime.Now).Replace("/", "");
            string filename = string.Format(LOG_FILE, strDate);
            try 
	        {
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(filename))
                {
                    string strFuncName = string.Format("{0}.{1}()", ex.TargetSite.DeclaringType.FullName, ex.TargetSite.Name);
                    string logLine = System.String.Format("{0:G}: [{1}].", System.DateTime.Now, strFuncName);
                    sw.WriteLine(logLine);
                    sw.WriteLine(ex.Message);
                    sw.WriteLine("-------------------------------------------");
                }
	        }	            
            catch (Exception)
            {

            }
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

//#if DEBUG
//            Utility.ShowError(strFuncName, strMsg);
//#endif
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pSource">Noi phat sinh loi</param>
        /// <param name="pMessage">Noi dung loi</param>
        public static void ShowError(string pSource, string pMessage)
        {
            if (pSource != null && pMessage != null)
            {
               // System.Windows.Forms.MessageBox.Show(pMessage, pSource, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
