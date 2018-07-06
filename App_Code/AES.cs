using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

    public class AES
    {
        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="key"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string Encrypt(string key, string request)
        {
            string result = string.Empty;
            string encryptKey = key;
            string encryptString = request;
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(encryptKey);
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(encryptString);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            result = Convert.ToBase64String(resultArray, 0, resultArray.Length);
            return result;
        }


        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="key"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public static string Decrypt(string key, string response)
        {
            string result = string.Empty;
            string decryptKey = key;
            String decryptString = response;
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(decryptKey);
            byte[] toEncryptArray = Convert.FromBase64String(decryptString);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            ICryptoTransform cTransform1 = rDel.CreateDecryptor();
            byte[] resultArray = cTransform1.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            result = UTF8Encoding.UTF8.GetString(resultArray);
            return result;
        }
    }

    public class ptData
    {
        public string fileUrl;
        private string FileUrl
        {
            get { return this.fileUrl; }
            set { this.fileUrl = value; }
        }

        public string fileContent;
        private string FileContent
        {
            get { return this.fileContent; }
            set { this.fileContent = value; }
        }


        public string returnCode;
        private string ReturnCode
        {
            get { return this.returnCode; }
            set { this.returnCode = value; }
        }

        public string returnMsg;
        private string ReturnMsg
        {
            get { return this.returnMsg; }
            set { this.returnMsg = value; }
        }
    }

    public class QueryRequest
    {
        public string billDate;
        private string BillDate
        {
            get { return this.billDate; }
            set { this.billDate = value; }
        }
    }

    public class Request
    {
        public string vision;
        private string Vision
        {
            get { return this.vision; }
            set { this.vision = value; }
        }

        public string mchid;
        private string Mchid
        {
            get { return this.mchid; }
            set { this.mchid = value; }
        }


        public string appCode;
        private string AppCode
        {
            get { return this.appCode; }
            set { this.appCode = value; }
        }

        public string sign;
        private string Sign
        {
            get { return this.sign; }
            set { this.sign = value; }
        }

        public string bizContent;
        private string BizContent
        {
            get { return this.bizContent; }
            set { this.bizContent = value; }
        }
    }

