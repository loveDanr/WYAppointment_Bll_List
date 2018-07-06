using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Net;
using System.Configuration;
using System.IO;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Web;
using System.Text.RegularExpressions;


    /// <summary>
    /// 保存日志
    /// </summary>
    public class Logging
    {

        #region 日志分类
        /// <summary>
        /// 保存普通日志
        /// </summary>
        /// <param name="message"></param>
        public static void WriteWYlog(string filename ,string message)
        {
            string logContent = string.Format(message);
            SetFile(filename+@"___微医Log.xml", "接口请求时间：" +System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "→→→→→" + logContent);
        }
    /// <summary>
    /// 保存普通日志
    /// </summary>
    /// <param name="message"></param>
    public static void WriteHISlog(string filename, string message)
    {
        string logContent = string.Format(message);
        SetFile(filename + @"___HISLog.xml", "接口请求时间："+System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+ "→→→→→" + logContent);
    }
    /// <summary>
    /// 保存普通日志
    /// </summary>
    /// <param name="message"></param>
    public static void WriteListlog(string filename, List<string[]> message)
        {
            List<string[]> logContent = message;
            SetFile(filename + @"__微医Log.xml", Convert.ToString(logContent));
        }

        /// <summary>
        /// 保存关键日志
        /// </summary>
        /// <param name="message"></param>
        public static void WriteKeylog(string filename,string message)
        {
            var logContent = string.Format(message);
            SetFile(filename + "KeyLog.xml", logContent);
        }
      
       
         /// <summary>
        /// 保存错误信息日志
        /// </summary>
        /// <param name="ex"></param>
        public static void WriteBuglog(Exception ex)
        {
            var logContent = string.Format("[{0}]错误发生在：{1}，\r\n 内容：{2}",
                DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), ex.Source, ex.Message);
            logContent += string.Format("\r\n [{0}] 跟踪：{1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"),
                ex.StackTrace);
            SetFile(@"BugLog.txt", logContent);
        }
        #endregion

        #region 通用操作
        /// <summary>
        /// 标准化写入过程，继承之后可自定义写入内容
        /// 默认保存在debug目录的Log目录下
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="logContent">写入内容</param>
        protected static void SetFile(string filename, string logContent)
        {
            Isexist(); // 判断Log目录是否存在
            string RootPath = GetRootPath();
            string errLogFilePath = RootPath + "\\"+filename.Trim();
            StreamWriter sw;
            if (!File.Exists(errLogFilePath))
            {
                FileStream fs1 = new FileStream(errLogFilePath, FileMode.Create, FileAccess.Write);
                sw = new StreamWriter(fs1);

            }
            else
            {
                sw = new StreamWriter(errLogFilePath, true);
            }
            sw.WriteLine(logContent);
            sw.Flush();
            sw.Close();
        }
        /// <summary>
        /// 取得网站根目录的物理路径
        /// </summary>
        /// <returns></returns>
        public static string GetRootPath()
        {
            string AppPath = "";
            HttpContext HttpCurrent = HttpContext.Current;
            if (HttpCurrent != null)
            {
                AppPath = HttpCurrent.Server.MapPath("~");
            }
            else
            {
                AppPath = AppDomain.CurrentDomain.BaseDirectory;
                if (Regex.Match(AppPath, @"\\$", RegexOptions.Compiled).Success)
                    AppPath = AppPath.Substring(0, AppPath.Length - 1);
            }
            return AppPath+"Log";
        }
        // 判断是否存在日志文件
        private static void Isexist()
        {
            string path = GetRootPath();
          //  string path = HttpRuntime.AppDomainAppVirtualPath + "Log";
            if (!File.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        
        #endregion
    }
