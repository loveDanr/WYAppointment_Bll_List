using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;
using System.Web.Script.Serialization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Web.Security;
using System.Configuration;
using Microsoft.Office.Interop.Excel;
using System.Data;
using System.IO.Compression;

    public class GetDBdata
    {
        /// <summary>
        /// 解压缩字符串
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Decompress(string input)
        {
            byte[] inputBytes = Convert.FromBase64String(input);
            byte[] depressBytes = Decompress(inputBytes);
            return Encoding.Default.GetString(depressBytes);
        }
        /// <summary>
        /// 压缩字节数组
        /// </summary>
        /// <param name="str"></param>
        public static byte[] Decompress(byte[] inputBytes)
        {

            using (MemoryStream inputStream = new MemoryStream(inputBytes))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    using (GZipStream zipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                    {
                        zipStream.CopyTo(outStream);
                        zipStream.Close();
                        return outStream.ToArray();
                    }
                }

            }
        }
        #region   获取平台一天的数据
        /// <summary>
        /// 获取平台一天的数据
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static List<string[]> getPtData(string date)
        {
            // 密钥       
            string key = ConfigurationManager.AppSettings["Ptkey"];//平台密匙
            string vision = ConfigurationManager.AppSettings["Vision"];//版本号
            string appCode = ConfigurationManager.AppSettings["AppCode"];//应用标号
            string mchid = ConfigurationManager.AppSettings["Mchid"];//商户编号
            string pturl = ConfigurationManager.AppSettings["Wyurl"];//平台服务器访问地址
            QueryRequest qr = new QueryRequest(); 
            qr.billDate = date;

            //海口市妇幼保健院
            //key = "NDYV32674JEKWXTV6563CKMLMYFE8423";
            //appCode = "QD113201703011044410001";
            //mchid = "m113000004";

            // 序列化查询条件
            JavaScriptSerializer serializar = new JavaScriptSerializer();
            string result = serializar.Serialize(qr);
            string bizContent = AES.Encrypt(key, result);
            Request request = new Request();
            request.vision = vision;
            request.mchid = mchid;
            request.appCode = appCode;
            request.bizContent = bizContent;
            request.sign = VerifySignatures(key, request);
            string jsonstr = serializar.Serialize(request);

            string strJson = HttpPost(pturl, jsonstr);//正式地址
            Request req = serializar.Deserialize<Request>(strJson);
            string response = AES.Decrypt(key, req.bizContent);

            serializar = new JavaScriptSerializer();
            ptData ptdata = serializar.Deserialize<ptData>(response);

            List<string[]> listData = new List<string[]>();
            //List<WedoctorFileData> listData = new List<WedoctorFileData>();
            
            if (ptdata.returnCode == "0000")
            {
                string str = ptdata.fileContent;
                var arr = str.Split(Environment.NewLine.ToCharArray());
                arr = arr.Where(s => !string.IsNullOrEmpty(s)).ToArray();
                
                
                foreach (string s in arr)
                {
                    //WedoctorFileData wd = new WedoctorFileData();
                    var wd = s.Replace("'","").Split(',');
                    listData.Add(wd);
                }
                
               
            }
            else
            {
                string msg = date.Substring(0, 4) + "年" + date.Substring(4, 2) + "月" + date.Substring(6, 2) + "日";
                msg = msg + "平台账单由于微信后台账单生成时间限制导致账单" + ptdata.returnMsg + "，请于11点后查看！";
                //MessageBox.Show(msg);
               // HttpContext.Current.Response.Write(" <script>alert('没有数据可导出！');");
               // return listData = null ;
                
            }

            return listData;
        }
        #endregion
        #region  将xml转为Datable
        /// <summary>
        /// 将xml转为Datable
        /// </summary>
        /// <param name="xmlStr"></param>
        /// <returns></returns>
        public static System.Data.DataTable XmlToDataTable(string xmlStr)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            if (!string.IsNullOrEmpty(xmlStr))
            {
                StringReader StrStream = null;
                XmlTextReader Xmlrdr = null;
                try
                {
                    DataSet ds = new DataSet();
                    //读取字符串中的信息  
                    StrStream = new StringReader(xmlStr);
                    //获取StrStream中的数据  
                    Xmlrdr = new XmlTextReader(StrStream);
                    //ds获取Xmlrdr中的数据
                    ds.ReadXml(Xmlrdr);
                    if (ds.Tables.Count < 3)
                    {
                        dt = null;
                    }
                    else
                    {
                        dt = ds.Tables[2];
                    }
                    //dt.DefaultView.Sort = "orderTime  ASC";//排序
                    //dt = dt.DefaultView.ToTable();
                    return dt;
                }
                catch (Exception e)
                {
                    return null;
                }
                finally
                {
                    //释放资源  
                    if (Xmlrdr != null)
                    {
                        Xmlrdr.Close();
                        StrStream.Close();
                        StrStream.Dispose();
                    }
                }
            }
            return null;
        }
        #endregion
        #region   从平台服务器获取数据
        /// <summary>
        /// 从服务器上获取数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public static string HttpPost(string url, string postData)
        {
            string text = string.Empty;
            byte[] bytes = Encoding.UTF8.GetBytes(postData);
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "post";
                httpWebRequest.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
                httpWebRequest.ContentLength = (long)bytes.Length;
                Stream requestStream = httpWebRequest.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
                using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    Stream responseStream = httpWebResponse.GetResponseStream();
                    StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    text += streamReader.ReadToEnd();
                    responseStream.Close();
                    responseStream = null;
                    streamReader.Close();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return text;
        }
        #endregion

        #region   生成sign签名串
        /// <summary>
        /// 生成sign签名串
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string VerifySignatures(string key, Request request)
        {
            IDictionary<string, string> dict = new Dictionary<string, string>();
            string sign = string.Empty;

            dict.Add("vision", request.vision);
            dict.Add("mchid", request.mchid);
            dict.Add("appCode", request.appCode);
            dict.Add("bizContent", request.bizContent);
            string signString = CreateLinkString(SortDictPara(dict));
            signString += "&key=" + key;
            return sign = Encode(signString).ToUpper();
        }
        #endregion

        #region  键值集合对按A~Z排序
        /// <summary>
        /// 键值集合对按A~Z排序
        /// </summary>
        /// <param name="dicArrayPre"></param>
        /// <returns></returns>
        public static IDictionary<string, string> SortDictPara(IDictionary<string, string> dicArrayPre)
        {
            SortedDictionary<string, string> dicTemp = new SortedDictionary<string, string>(dicArrayPre);
            Dictionary<string, string> dicArray = new Dictionary<string, string>(dicTemp);
            return dicArray;
        }
        #endregion

        #region   md5加签
        /// <summary>
        /// md5加签
        /// </summary>
        /// <param name="encryptString"></param>
        /// <returns></returns>
        public static string Encode(string encryptString)
        {
            MD5CryptoServiceProvider md5CSP = new MD5CryptoServiceProvider();
            //获取要加密的字段，并转化为Byte[]数组  
            byte[] testEncrypt = Encoding.Default.GetBytes(encryptString);
            //加密Byte[]数组  
            byte[] resultEncrypt = md5CSP.ComputeHash(testEncrypt);
            //将加密后的数组转化为字段(普通加密)  
            string testResult = Encoding.Unicode.GetString(resultEncrypt);
            //作为密码方式加密   
            return FormsAuthentication.HashPasswordForStoringInConfigFile(encryptString, "MD5");
        }
        #endregion

        #region   将键值对转换成规则（key = nvlue）格式字符串
        /// <summary>
        /// 将键值对转换成规则（key = nvlue）格式字符串
        /// </summary>
        /// <param name="dicArray"></param>
        /// <returns></returns>
        public static string CreateLinkString(IDictionary<string, string> dicArray)
        {
            StringBuilder prestr = new StringBuilder();
            foreach (KeyValuePair<string, string> temp in dicArray)
            {
                prestr.Append(temp.Key + "=" + temp.Value + "&");
            }
            //去掉最後一個&字符
            int nLen = prestr.Length;
            prestr.Remove(nLen - 1, 1);
            return prestr.ToString();
        }
    #endregion

    #region 获取统计数据
    public static System.Data.DataTable GetResult(System.Data.DataTable dt)
    {
        
        double amount = 0.00;
        System.Data.DataTable dt_Result = new System.Data.DataTable();
        #region 获取his的统计数据
        if (dt!=null&&dt.TableName=="RegList")
        {
            dt_Result.Columns.AddRange(new DataColumn[] { new DataColumn("TradeDatehis",typeof(string)),
            new DataColumn("Counthis",typeof(int)), new DataColumn("Amounthis",typeof(string))});
            foreach (DataRow dr in dt.Rows)
            {
                dr["OPERDATE"] = dr["OPERDATE"].ToString().Substring(0,10);
            }
            if (dt!=null&&dt.Rows.Count!=0)
            {
                var query = from t in dt.AsEnumerable()
                            group t by new
                            { t1 = t.Field<string>("OPERDATE") } into m
                            select new
                            {
                                operdate = m.Key.t1,
                                rowcount = m.Count()
                            };
                query.ToList().ForEach(q=>dt_Result.Rows.Add(q.operdate,q.rowcount));

                amount = 0;
                for (int i=0;i<dt_Result.Rows.Count;i++)
                {
                    string date1 = dt_Result.Rows[i]["TradeDatehis"].ToString().Trim();
                    foreach(DataRow dr in dt.Rows)
                    {
                        string date2 = dr["OPERDATE"].ToString().Trim(); 
                        if (date1==date2)
                        {
                            amount = amount + Convert.ToDouble(dr["OWN_COST"]);
                        }
                    }
                    dt_Result.Rows[i]["Amounthis"] = amount.ToString("f2");
                    amount = 0;
                }
            }
        }
        #endregion
        #region 获取微医统计数据
        if (dt != null && dt.TableName == "ACCOUNT_LIST")
        {
            dt_Result.Columns.AddRange(new DataColumn[] { new DataColumn("TradeDate",typeof(string)),
            new DataColumn("wxCount",typeof(int)), new DataColumn("wxAmount",typeof(string))});
            foreach (DataRow dr in dt.Rows)
            {
                dr["PAY_DATETIME"] = dr["PAY_DATETIME"].ToString().Substring(0, 10);
                
            }
            if (dt != null && dt.Rows.Count != 0)
            {
                var query = from t in dt.AsEnumerable()
                            group t by new
                            { t1 = t.Field<string>("PAY_DATETIME") } into m
                            select new
                            {
                                PAY_DATETIME = m.Key.t1,
                                rowcount = m.Count()
                            };
                query.ToList().ForEach(q => dt_Result.Rows.Add(q.PAY_DATETIME, q.rowcount));

                amount = 0;
                for (int i = 0; i < dt_Result.Rows.Count; i++)
                {
                    string date1 = dt_Result.Rows[i]["TradeDate"].ToString().Trim();
                    foreach (DataRow dr in dt.Rows)
                    {
                        string date2 = dr["PAY_DATETIME"].ToString().Trim();
                        if (date1 == date2)
                        {
                            if (dr["ORDER_TYPE"].ToString().Trim() == "1")
                            {
                                amount = amount + Convert.ToDouble(dr["PAY_FEE"]) / 100;
                            }

                             if (dr["ORDER_TYPE"].ToString().Trim() == "2" && dr["AUDIT_STATUS"].ToString().Trim() == "3")
                            {
                                amount = amount + Convert.ToDouble(dr["PAY_FEE"]) / 100;
                            }
                             if (dr["ORDER_TYPE"].ToString().Trim() == "2")
                            {
                                amount = amount - Convert.ToDouble(dr["PAY_FEE"]) / 100;
                            }
                            
                        }
                    }
                    dt_Result.Rows[i]["wxAmount"] = amount.ToString("f2");
                    amount = 0;
                }
            }
        }
        #endregion
        return dt_Result;
    }
    #endregion

    #region 将两个列不同的DataTable合并成一个新的DataTable
    /// <summary>
    /// 将两个列不同的DataTable合并成一个新的DataTable
    /// </summary>
    /// <param name="dt1">表1</param>
    /// <param name="dt2">表2</param>
    /// <param name="DTName">合并后新的表名</param>
    /// <returns></returns>
    public static System.Data.DataTable UniteDataTable(System.Data.DataTable dt1, System.Data.DataTable dt2, string DTName)
    {
        System.Data.DataTable dt3 = dt1.Clone();
        for (int i = 0; i < dt2.Columns.Count; i++)
        {
            dt3.Columns.Add(dt2.Columns[i].ColumnName);
        }
        object[] obj = new object[dt3.Columns.Count];

        for (int i = 0; i < dt1.Rows.Count; i++)
        {
            dt1.Rows[i].ItemArray.CopyTo(obj, 0);
            dt3.Rows.Add(obj);
        }

        if (dt1.Rows.Count >= dt2.Rows.Count)
        {
            for (int i = 0; i < dt2.Rows.Count; i++)
            {
                for (int j = 0; j < dt2.Columns.Count; j++)
                {
                    dt3.Rows[i][j + dt1.Columns.Count] = dt2.Rows[i][j].ToString();
                }
            }
        }
        else
        {
            DataRow dr3;
            for (int i = 0; i < dt2.Rows.Count - dt1.Rows.Count; i++)
            {
                dr3 = dt3.NewRow();
                dt3.Rows.Add(dr3);
            }
            for (int i = 0; i < dt2.Rows.Count; i++)
            {
                for (int j = 0; j < dt2.Columns.Count; j++)
                {
                    dt3.Rows[i][j + dt1.Columns.Count] = dt2.Rows[i][j].ToString();
                } 
            }
            
        }
        dt3.TableName = DTName; //设置DT的名字
        return dt3;
    }
#endregion
}
