using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;
using System.Collections;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Threading;
 

public partial class _Default : System.Web.UI.Page
{    
    System.Data.DataTable dt_wy = new System.Data.DataTable();
    System.Data.DataTable dt_his = new System.Data.DataTable();
    System.Data.DataTable dt_his_Result = new System.Data.DataTable();
    System.Data.DataTable dt_wy_Result = new System.Data.DataTable();
    System.Data.DataTable DtAll = new System.Data.DataTable();
    public delegate string tLoadHISRequest(string request, string request2);//定义个获取his请求信息的委托
    public delegate string tLoadWYRequest(string request, string request2);//定义个获取微医请求信息的委托
    public delegate string tLoadHISData(string request);//定义个获取his信息的委托
    public delegate string tLoadWYData(string request);//定义个获取微医信息的委托
    protected void Page_Load(object sender, EventArgs e)
    { 
        if (!IsPostBack)
        {
            txt_startDate.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            txt_endDate.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
           
        }
        
    }
    private void LoadFromWebservice()
    {
        //定义一个定时器，并开启和配置相关属性
        System.Timers.Timer Wtimer = new System.Timers.Timer(1);//执行任务的周期
        Wtimer.Elapsed += new System.Timers.ElapsedEventHandler(GetDate);
        Wtimer.Enabled = true;
        Wtimer.AutoReset = true;
    }
    void GetDate(object sender, System.Timers.ElapsedEventArgs e)
    {
        int intHour = e.SignalTime.Hour;
        int intMinute = e.SignalTime.Minute;
        int intSecond = e.SignalTime.Second;
        // 定制时间； 比如 在10：30 ：00 的时候执行某个函数
        int iHour = 14;
        int iMinute = 17;
        int iSecond = 00;
        // 设置　每天的１０：３０：００开始执行程序
        if (intHour == iHour && intMinute == iMinute && intSecond == iSecond)
        {
            txt_startDate.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            txt_endDate.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            tLoadHISRequest tloadhisRequest = new tLoadHISRequest(gethisReq);
            IAsyncResult result = tloadhisRequest.BeginInvoke(txt_startDate.Text.Trim(), txt_endDate.Text.Trim(), null, null);
            tLoadWYRequest tLoadwyRequest = new tLoadWYRequest(getReq);
            IAsyncResult result2 = tLoadwyRequest.BeginInvoke(txt_startDate.Text.Trim(), txt_endDate.Text.Trim(), null, null);

            string _requesthis = tloadhisRequest.EndInvoke(result);
            string _request = tLoadwyRequest.EndInvoke(result2);


            RefreshCheckData(_request, _requesthis);
        }

    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        DateTime t1 = Convert.ToDateTime(txt_startDate.Text.Trim());
        DateTime t2 = Convert.ToDateTime(txt_endDate.Text.Trim());
        DateTime t3 = Convert.ToDateTime(System.DateTime.Now.ToString("yyyy-MM-dd"));
        if (DateTime.Compare(t1, t2) > 0)
        {
            //strErr += "截止日期必须在发布日期之后！";
            //HttpContext.Current.Response.Write(" <script>alert('没有数据可导出！');");
            // Response.Write(" <script>function window.onload() {alert( ' 弹出的消息' ); } </script> ");
            Page.ClientScript.RegisterStartupScript(this.GetType(), "", "<script>alert('开始时间大于结束时间啦！');</script>");
        }
        //else if (DateTime.Compare(t2, t3) < 0)
        //{
        //    Page.ClientScript.RegisterStartupScript(this.GetType(), "", "<script>alert('时间选择有误，请重新选择！');</script>");
        //}
       
        else
        {

            tLoadHISRequest tloadhisRequest = new tLoadHISRequest(gethisReq);
            IAsyncResult result = tloadhisRequest.BeginInvoke(txt_startDate.Text.Trim(), txt_endDate.Text.Trim(), null, null);
            tLoadWYRequest tLoadwyRequest = new tLoadWYRequest(getReq);
            IAsyncResult result2 = tLoadwyRequest.BeginInvoke(txt_startDate.Text.Trim(), txt_endDate.Text.Trim(), null, null);

            string _requesthis = tloadhisRequest.EndInvoke(result);
            string _request = tLoadwyRequest.EndInvoke(result2);


            RefreshCheckData(_request, _requesthis);

        }

    }
    #region  获取his入参
    /// <summary>
    /// 获取his入参
    /// </summary>
    public string gethisReq(string begindata, string enddata)
    {
        string request = string.Empty;
        request = @" <Request>
                         <StartDate>" + begindata + @" 00:00:00</StartDate>
                         <EndDate>" + enddata + @" 23:59:59</EndDate>
                         </Request>";

        return request;
    }
    #endregion

    private int sum_totalCount = 0;
    private decimal sum_TotalGet = 0;
    private decimal sum_his_TotalGet = 0;
    protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
        {
            //保持列不变形
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Wrap = false;
                //方法二：
                //e.Row.Cells[i].Text = "<nobr>&nbsp;" + e.Row.Cells[i].Text + "&nbsp;</nobr>";            
            }
            if (e.Row.RowType == DataControlRowType.Header)
            {
            e.Row.Cells[0].Text= "平台订单号";
            e.Row.Cells[1].Text = "医院订单号";
            e.Row.Cells[2].Text = "订单类型";
            e.Row.Cells[3].Text = "订单状态";
            e.Row.Cells[4].Text = "医院侧交易单号";
            e.Row.Cells[5].Text = "支付渠道";
            e.Row.Cells[6].Text = "交易流水号";
            e.Row.Cells[7].Text = "交易金额";
            e.Row.Cells[8].Text = "交易时间";
            e.Row.Cells[9].Text = "旧医院订单号";
            e.Row.Cells[10].Text = "旧序列号";
            e.Row.Cells[11].Text = "用户ID";
            e.Row.Cells[12].Text = "用户姓名";
            e.Row.Cells[13].Text = "用户证件号码";
            e.Row.Cells[14].Text = "其他";
            }
          

        }
        //如果是绑定数据行 
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            ////鼠标经过时，行背景色变 
            //e.Row.Attributes.Add("onmouseover", "this.style.backgroundColor='#E6F5FA'");
            ////鼠标移出时，行背景色变 
            //e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor='#FFFFFF'");
            #region 订单类型
            if (e.Row.Cells[2].Text == "1")
            {
                e.Row.Cells[2].Text = "支付";
            }
            if (e.Row.Cells[2].Text == "2")
            {
                e.Row.Cells[2].Text = "退费";
            }
            #endregion

            #region 订单状态
            if (e.Row.Cells[3].Text == "1")
            {
                e.Row.Cells[3].Text = "审核中";
            }
            if (e.Row.Cells[3].Text == "2")
            {
                e.Row.Cells[3].Text = "审核通过";
            }
            if (e.Row.Cells[3].Text == "3")
            {
                e.Row.Cells[3].Text = "审核驳回";
            }
            if (e.Row.Cells[3].Text == "4")
            {
                e.Row.Cells[3].Text = "退费中";
            }
            #endregion

            #region 支付渠道
            if (e.Row.Cells[5].Text == "1")
            {
                e.Row.Cells[5].Text = "微信支付";
            }
            if (e.Row.Cells[5].Text == "2")
            {
                e.Row.Cells[5].Text = "支付宝支付";
            }
            if (e.Row.Cells[5].Text == "3")
            {
                e.Row.Cells[5].Text = "银联支付";
            }
            if (e.Row.Cells[5].Text == "4")
            {
                e.Row.Cells[5].Text = "医院窗口";
            }
            if (e.Row.Cells[5].Text == "5")
            {
                e.Row.Cells[5].Text = "支付宝WAP支付";
            }
            if (e.Row.Cells[5].Text == "6")
            {
                e.Row.Cells[5].Text = "支付宝WEB支付";
            }
            if (e.Row.Cells[5].Text == "7")
            {
                e.Row.Cells[5].Text = "微信扫码支付";
            }
            if (e.Row.Cells[5].Text == "11")
            {
                e.Row.Cells[5].Text = "代收";
            }
            #endregion

            #region 金额正负
            if(e.Row.Cells[2].Text=="退费")
                {
                    double x= Convert.ToDouble(e.Row.Cells[7].Text.ToString());
                    e.Row.Cells[7].Text = (-(Convert.ToDouble(x)) / 100).ToString("f2");
                }
                if (e.Row.Cells[2].Text == "支付")
                {
                    double x = Convert.ToDouble(e.Row.Cells[7].Text.ToString());
                    e.Row.Cells[7].Text = ((Convert.ToDouble(x)) / 100).ToString("f2");
                }
            #endregion
            //方法二：
            //e.Row.Cells[i].Text = "<nobr>&nbsp;" + e.Row.Cells[i].Text + "&nbsp;</nobr>";

            //for (int i = dt_his.Rows.Count - 1; i >= 0; i--)
            //{
            //    for (int k = 0; k < dt_wy.Rows.Count - 1; k++)
            //    {
            //        if (dt_his.Rows[i][3].ToString() == dt_wy.Rows[k + 1][11].ToString() && dt_his.Rows[i][13].ToString() == dt_wy.Rows[k + 1][1].ToString() && dt_his.Rows[i][1].ToString() == dt_wy.Rows[k + 1][2].ToString())
            //        {
                       
            //            e.Row.BackColor = System.Drawing.Color.Green;
            //        }

            //    }
            //}
            //if (dt_wy.Rows.Count!=0)
            //{
            //    for (int x =0;x<dt_wy.Rows.Count -1;x++)
            //    {

            //    }
            //}
        }

        if (e.Row.RowIndex >= 0)
        {


            sum_TotalGet += Convert.ToDecimal(e.Row.Cells[7].Text);

        }
        else if (e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[1].Text = dt_wy.Rows.Count.ToString();// RowsCount.ToString();
            e.Row.Cells[3].Text = sum_TotalGet.ToString("F2");
            //TotalDealCount.Text = "微医总交易数：" + e.Row.Cells[1].Text+"笔";
            //TotalFeeCount.Text = "微医总金额：" + e.Row.Cells[3].Text+"元";
            e.Row.Cells[1].Font.Bold = true;
            e.Row.Cells[3].Font.Bold = true;
        }
    }
    protected void GridView_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            TableCellCollection tcc = e.Row.Cells;
            tcc.Clear();
            e.Row.Cells.Add(new TableCell());
            e.Row.Cells[0].Attributes.Add("colspan", "3");
            e.Row.Cells[0].Text = "总交易数";
            
            e.Row.Cells[0].Font.Bold = true;
            e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center;
            e.Row.Cells.Add(new TableCell());

            e.Row.Cells[1].Text = "";//
            e.Row.Cells.Add(new TableCell());

            e.Row.Cells[2].Text = "总收入";//
            
            e.Row.Cells[2].Font.Bold = true;
            e.Row.Cells.Add(new TableCell());

            e.Row.Cells[3].Text = "";//
            e.Row.Cells.Add(new TableCell());

            e.Row.Cells[4].Attributes.Add("colspan", "5");
            e.Row.Cells[4].Text = "";
            e.Row.Cells[4].HorizontalAlign = HorizontalAlign.Center;
            e.Row.Cells.Add(new TableCell());
        }
        if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
        {
            #region 设置GridView列的属性为文本，便于导出excel时显示正常
            e.Row.Cells[0].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
            e.Row.Cells[1].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
            e.Row.Cells[2].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
            e.Row.Cells[3].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
            e.Row.Cells[4].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
            e.Row.Cells[6].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
            e.Row.Cells[10].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
            e.Row.Cells[11].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
            e.Row.Cells[13].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
            e.Row.Cells[14].Visible = false;
            e.Row.Cells[9].Visible = false;
            e.Row.Cells[10].Visible = false;
            #endregion
        }
    }

    #region 导出到Excel的方法
    /// <summary>
    /// 导出到Excel
    /// </summary>
    /// <param name="gv"></param>
    void toExcel(GridView gv, string fileName)
    {
        Response.Charset = "UTF-8";
        Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
        string style = @"<style> .text { mso-number-format:\@; } </script> ";
        Response.ClearContent();
        Response.AddHeader("content-disposition", "attachment; filename=" + fileName + ".xls");
        Response.ContentType = "application/excel";
        StringWriter sw = new StringWriter();
        HtmlTextWriter htw = new HtmlTextWriter(sw);
        gv.RenderControl(htw);
        Response.Write(style);
        Response.Write(sw.ToString());
        Response.End();
    }
    #endregion
    /// <summary>
    /// 这个重写貌似是必须的
    /// </summary>
    /// <param name="control"></param>
    public override void VerifyRenderingInServerForm(Control control) { }
    #region "页面加载中效果"
    /// <summary>
    /// 页面加载中效果
    /// </summary>
    public static void initJavascript()
    {
        HttpContext.Current.Response.Write(" <script language=JavaScript type=text/javascript>");
        HttpContext.Current.Response.Write("var t_id = setInterval(animate,20);");
        HttpContext.Current.Response.Write("var pos=0;var dir=2;var len=0;");
        HttpContext.Current.Response.Write("function animate(){");
        HttpContext.Current.Response.Write("var elem = document.getElementById('progress');");
        HttpContext.Current.Response.Write("if(elem != null) {");
        HttpContext.Current.Response.Write("if (pos==0) len += dir;");
        HttpContext.Current.Response.Write("if (len>32 || pos>79) pos += dir;");
        HttpContext.Current.Response.Write("if (pos>79) len -= dir;");
        HttpContext.Current.Response.Write(" if (pos>79 && len==0) pos=0;");
        HttpContext.Current.Response.Write("elem.style.left = pos;");
        HttpContext.Current.Response.Write("elem.style.width = len;");
        HttpContext.Current.Response.Write("}}");
        HttpContext.Current.Response.Write("function remove_loading() {");
        HttpContext.Current.Response.Write(" this.clearInterval(t_id);");
        HttpContext.Current.Response.Write("var targelem = document.getElementById('loader_container');");
        HttpContext.Current.Response.Write("targelem.style.display='none';");
        HttpContext.Current.Response.Write("targelem.style.visibility='hidden';");
        HttpContext.Current.Response.Write("}");
        HttpContext.Current.Response.Write("</script>");
        HttpContext.Current.Response.Write("<style>");
        HttpContext.Current.Response.Write("#loader_container {text-align:center; position:absolute; top:20%;width:100%; left: 0;}");
        //HttpContext.Current.Response.Write("#loader {font-family:Tahoma, Helvetica, sans; font-size:13.5px; color:#000000; background-color:#FFFFFF; padding:10px 0 16px 0; margin:0 auto; display:block; width:130px;height: 40px; border:1px solid #5a667b; text-align:left; z-index:2;}");
        //HttpContext.Current.Response.Write("#progress {height:5px; font-size:5px; width:5px; position:relative; top:1px; left:0px; background-color:#8894a8;}");
        HttpContext.Current.Response.Write("#loader_bg {background-color:#e4e7eb; position:relative; top:8px; left:8px; height:7px; width:113px; font-size:1px;}");
        HttpContext.Current.Response.Write("</style>");
        HttpContext.Current.Response.Write("<div id=loader_container>");
        //HttpContext.Current.Response.Write("<div id=loader>");
        HttpContext.Current.Response.Write("<div align=center><img src='/Images/loading.gif'/></div>");
        //HttpContext.Current.Response.Write("<div id=loader_bg> <div id=progress></div> </div>"); 
        HttpContext.Current.Response.Write("</div></div>");
        HttpContext.Current.Response.Flush();
    }
    //
    #endregion

        /// <summary>
        /// 下载按钮的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
    protected void download_btn_Click(object sender, EventArgs e)
    {
        if (Notice.Text.Contains("恭"))
        {
            if (this.GridView_Count.Rows.Count != 0)
            {
                if (txt_startDate.Text.Trim()==txt_endDate.Text.Trim())
                {
                    toExcel(this.GridView_Count, txt_startDate.Text.Trim() + "的对账总表");
                }
                else if (txt_startDate.Text.Trim() != txt_endDate.Text.Trim())
                {
                    toExcel(this.GridView_Count, txt_startDate.Text.Trim() + "至" + txt_endDate.Text.Trim() + "的对账总表");
                }
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "", "<script>alert('没有数据可导出！');</script>");

            }
        }
        else
        {
            if (Notice.Text.Contains("不"))
            {
                if (this.GridView.Rows.Count != 0)
                {
                    if (txt_startDate.Text.Trim() == txt_endDate.Text.Trim())
                    {
                        toExcel(this.GridView, txt_startDate.Text.Trim() + "的对账明细表");
                    }
                    else if (txt_startDate.Text.Trim() != txt_endDate.Text.Trim())
                    {
                        toExcel(this.GridView, txt_startDate.Text.Trim() + "至" + txt_endDate.Text.Trim() + "的对账明细表");
                    }
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "", "<script>alert('没有数据可导出！');</script>");

                }
            }
        }
       
    }
    #region  获取his交易数据
    /// <summary>
    /// 获取his交易数据
    /// </summary>
    /// <returns></returns>
    public string gethisXml(string request)
    {
        #region 调用HIS webservices地址
        string response = string.Empty;
        string hisUri = ConfigurationManager.AppSettings["HISURL"];//平台密匙

        try
        {
            
            HIS.HYServicesSoapClient his = new HIS.HYServicesSoapClient();
            //his.Endpoint.Address = new System.ServiceModel.EndpointAddress("http://10.1.10.74:81/HYServices.asmx");
            his.Endpoint.Address = new System.ServiceModel.EndpointAddress(hisUri);

            response = his.GetRegINfor(request);
            return response;
        }
        catch (Exception e)
        {
            //MessageBox.Show(e.Message.ToString());
            return response;
        }
        #endregion
    }
    #endregion
    double ColumnSum(DataTable dt, string ColumnName)
    {
        double d = 0;
        foreach (DataRow row in dt.Rows)
        {
            d += double.Parse(row[ColumnName].ToString());
        }
        return d;
    }
    #region  刷新数据明细数据
    /// <summary>
    /// 刷新数据明细数据
    /// </summary>
    public void RefreshCheckData(String request,String request_his)
    {
        #region 查询明细数据

        initJavascript();
        tLoadHISData tloadhis = new tLoadHISData(gethisXml);
        IAsyncResult result = tloadhis.BeginInvoke(request_his, null, null);
        tLoadWYData tloadwy = new tLoadWYData(getXml);
        IAsyncResult result_WY = tloadwy.BeginInvoke(request, null, null);
        string strhisxml = tloadhis.EndInvoke(result);
        string strxml = tloadwy.EndInvoke(result_WY);

        dt_his = GetDBdata.XmlToDataTable(strhisxml);
        dt_wy = GetDBdata.XmlToDataTable(strxml);
        if (dt_wy!=null&&dt_his!=null)
        { 
            try
            {
                //this.GridView.DataSource = dt_wy.DefaultView;
                //this.GridView.DataBind();
                //for (int i = dt_his.Rows.Count-1;i>=0; i--)
                //{
                //    for (int k =0; k < dt_wy.Rows.Count; k++)
                //    {
                //        if (dt_his.Rows[i][3].ToString() == dt_wy.Rows[k][11].ToString() && dt_his.Rows[i][13].ToString() == dt_wy.Rows[k][1].ToString() && dt_his.Rows[i][1].ToString() == dt_wy.Rows[k][2].ToString())
                //        {
                //            GridView.Rows[k].BackColor = System.Drawing.Color.Green;
                //        }

                //    }
                    
                //}

                #region 统计所有数据
                dt_his_Result = GetDBdata.GetResult(dt_his);
                dt_wy_Result = GetDBdata.GetResult(dt_wy);
                DtAll = GetDBdata.UniteDataTable(dt_his_Result, dt_wy_Result, "合并Dt");
                DtAll.Columns.AddRange(new DataColumn[] { new DataColumn("different", typeof(double)) });
                DataRow drw = DtAll.NewRow();
                double different_money=0.00;
                for (int i = 0; i < DtAll.Rows.Count; i++)
                {
                    foreach (DataRow dr in DtAll.Rows)
                    {
                        different_money= different_money+( Convert.ToDouble(DtAll.Rows[i]["Amounthis"]) -Convert.ToDouble(DtAll.Rows[i]["wxAmount"]));
                        DtAll.Rows[i]["different"] = different_money.ToString("f2");
                        different_money = 0;
                    }

                }

                this.GridView.Visible = false;
                this.GridView_Count.DataSource = DtAll.DefaultView;
                this.GridView_Count.DataBind();
                #endregion
                #region 账不平的变红色

                for (int k = 0; k < DtAll.Rows.Count; k++)
                {
                    if (DtAll.Rows[k][6].ToString() != "0")
                    {
                        GridView_Count.Rows[k].BackColor = System.Drawing.Color.Red;
                        //Logging.WriteHISlog("记录日志：","HIS的CARD_NO="+dt_his.Rows[i][3].ToString()+ "\r\n"+"微医的HOSP_PATIENT_ID="+ dt_wy.Rows[k][11].ToString() + "\r\n" + "HIS的TRANS_NO="+dt_his.Rows[i][13].ToString()+"微医的HOSP_ORDER_ID = "+ dt_wy.Rows[k][1].ToString() + "\r\n" + "HIS的TRANS_TYPE=" + dt_his.Rows[i][1].ToString() + "\r\n" + "微医的ORDER_TYPE=" + dt_wy.Rows[k][2].ToString() + "");
                    }

                }

                #endregion


            }

            catch (Exception Exc)
            {
                Logging.WriteBuglog(Exc);
       
            }
            finally
            {
                Logging.WriteWYlog(txt_startDate.Text + "至" + txt_endDate.Text + "的日志", strxml);
                Logging.WriteHISlog(txt_startDate.Text + "至" + txt_endDate.Text + "的日志", strhisxml);
            }
        }
        else
        {
            this.GridView.DataSource = "";
            this.GridView.DataBind();
        }
        #endregion

    }
    #endregion
    #region  获取平台入参
    /// <summary>
    /// 获取平台入参
    /// </summary>
    public string getReq(string begindata, string enddata)
    {
        string user_id = ConfigurationManager.AppSettings["USER_ID"];
        string hos_id = ConfigurationManager.AppSettings["HOS_ID"];
        string hos_foreign_id = ConfigurationManager.AppSettings["HOS_FOREIGN_ID"];
        string request = string.Empty;
        request = @"<ROOT>
                        <FUN_CODE></FUN_CODE>
                        <USER_ID>" + user_id + @"</USER_ID>
                        <SIGN_TYPE></SIGN_TYPE>
                        <SIGN></SIGN>
                        <REQ_ENCRYPTED></REQ_ENCRYPTED>
                        <REQ>
                        <HOS_ID>" + hos_id + @"</HOS_ID>
                        <HOS_FOREIGN_ID>" + hos_foreign_id + @"</HOS_FOREIGN_ID>
                        <START_DATE>" + begindata + @"</START_DATE>
                        <END_DATE>" + enddata + @"</END_DATE>
                        <PASSWORD></PASSWORD>
                        <PAGE_CURRENT>1</PAGE_CURRENT>
                        <PAGE_SIZE>100</PAGE_SIZE>
                        </REQ>
                        </ROOT>";
        //request = @"<ROOT>
        //                <FUN_CODE></FUN_CODE>
        //                <USER_ID>" + user_id + @"</USER_ID>
        //                <SIGN_TYPE></SIGN_TYPE>
        //                <SIGN></SIGN>
        //                <REQ_ENCRYPTED></REQ_ENCRYPTED>
        //                <REQ>
        //                <HOS_ID>" + hos_id + @"</HOS_ID>
        //                <HOS_FOREIGN_ID>" + hos_foreign_id + @"</HOS_FOREIGN_ID>
        //                <ORDER_ID>201806292833760885</ORDER_ID>
        //                <HOSP_ORDER_ID>WY201806292833760885</HOSP_ORDER_ID>
        //                </REQ>
        //                </ROOT>";

        return request;
    }
    #endregion
    #region  获取平台交易数据
    /// <summary>
    /// 获取平台交易数据
    /// </summary>
    /// <returns></returns>
    public string getXml(string request)
    {
        #region 调用平台webservices地址
        string response = string.Empty;
        try
        {
            string HY = ConfigurationManager.AppSettings["WY"];
            GHW.UnifyAccountCheckServiceClient pt = new GHW.UnifyAccountCheckServiceClient();
            //pt.Endpoint.Address = new System.ServiceModel.EndpointAddress("http://192.168.1.29:8080/hrs/accountCheckAuth");
            pt.Endpoint.Address = new System.ServiceModel.EndpointAddress(HY);

            //response = pt.AccountCheck(request);
           response = pt.AccountCheck(request);
            
           
            return response;
        }
        catch (Exception e)
        {
            Logging.WriteBuglog(e);
            return response = string.Empty ;
        }
        #endregion
    }
    #endregion

    private int sum_Total_count, sum_Total_count_wy;
    private double sum_Total_his_amount, sum_Total_amoun_wy,different_money;
    protected void GridView_Count_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
        {
            //保持列不变形
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Wrap = false;
                //方法二：
                //e.Row.Cells[i].Text = "<nobr>&nbsp;" + e.Row.Cells[i].Text + "&nbsp;</nobr>";            
            }
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[0].Text = "挂号日期";
                e.Row.Cells[1].Text = "HIS交易记录数";
                e.Row.Cells[2].Text = "HIS交易金额";
                e.Row.Cells[3].Text = "微医交易日期";
                e.Row.Cells[4].Text = "微医交易记录数";
                e.Row.Cells[5].Text = "微医交易金额";
                e.Row.Cells[6].Text = "双方差额";
            }
            if (e.Row.RowIndex >= 0)
            {


                sum_Total_count += Convert.ToInt32(e.Row.Cells[1].Text);
                sum_Total_his_amount += Convert.ToDouble(e.Row.Cells[2].Text);
                sum_Total_count_wy += Convert.ToInt32(e.Row.Cells[4].Text);
                sum_Total_amoun_wy += Convert.ToDouble(e.Row.Cells[5].Text);
                different_money+= Convert.ToDouble(e.Row.Cells[6].Text);

            }
          

        }
        else if (e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[1].Text = sum_Total_count.ToString();
            e.Row.Cells[2].Text = sum_Total_his_amount.ToString("F2");
            e.Row.Cells[4].Text = sum_Total_count_wy.ToString();
            e.Row.Cells[5].Text = sum_Total_amoun_wy.ToString("f2");
            e.Row.Cells[6].Text = different_money.ToString("f2");

        }
        #region 提升语内容设置
        if (e.Row.Cells[6].Text == "0.00")
        {
            DetailsListTitle.Visible = false;
            //TotalDealCount.Visible = false;
            //TotalFeeCount.Visible = false;
            //HIS_TotalDealCount.Visible = false;
            //HIS_TotalFeeCount.Visible = false;
          //  GridView.Visible = false;
            string str = "HIS总金额" + sum_Total_his_amount.ToString("f2") + "元";//HIS_TotalFeeCount.Text.Trim();
            if (txt_startDate.Text == txt_endDate.Text)
            {
                Notice.Text = "恭喜！" + txt_startDate.Text.Trim() + "的账平啦！\r\r\n 总金额是：" + str.Replace("HIS总金额", "￥");
            }
            if (txt_startDate.Text != txt_endDate.Text)
            {
                Notice.Text = "恭喜！" + txt_startDate.Text.Trim() + "至" + txt_endDate.Text.Trim() + "的账平啦！\r\r\n 总金额是：" + str.Replace("HIS总金额", "￥");
            }
        }
        else if (e.Row.Cells[6].Text != "0.00")
        {
            //TotalDealCount.Visible = false;
            //TotalFeeCount.Visible = false;
            //HIS_TotalDealCount.Visible = false;
            //HIS_TotalFeeCount.Visible = false;
            Notice.Text = "注意！有账不平啦！";
            GridView.Visible = true;
            DetailsListTitle.Visible = true;
            if (txt_startDate.Text == txt_endDate.Text)
            {
                DetailsListTitle.Text = txt_startDate.Text.Trim() + "的明细单。 备注：绿色是匹配成功的订单，白色是HIS没有的订单";
            }
            else if (txt_startDate.Text != txt_endDate.Text)
            {
                DetailsListTitle.Text = txt_startDate.Text.Trim() + "至" + txt_endDate.Text.Trim() + "的明细单。 备注：绿色是匹配成功的订单，白色是HIS没有的订单";
            }
        }

        #endregion

    }

    protected void GridView_Count_RowCreated(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.Footer)
        {  
            //e.Row.Cells[0].Attributes.Add("colspan", "3");
            e.Row.Cells[0].Text = "合计";

            //e.Row.Cells[1].Text = "";//
           
             

            //e.Row.Cells[2].Font.Bold = true;
           

            //e.Row.Cells[3].Text = "";//
         

            //e.Row.Cells[4].Text = "";//
         

            //e.Row.Cells[5].Text = "";//
            
             
            //e.Row.Cells[6].Text = "";
            //e.Row.Cells[6].HorizontalAlign = HorizontalAlign.Center;
 
        }
        if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
        {
            #region 设置GridView列的属性为文本，便于导出excel时显示正常
            e.Row.Cells[0].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
            e.Row.Cells[1].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
            e.Row.Cells[2].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
            e.Row.Cells[3].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
            e.Row.Cells[4].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
            e.Row.Cells[6].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
            #endregion
        }
    }

     

    protected void GridView_Count_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        GridView.Visible = false;
        if (e.CommandName == "Check")
        {
            initJavascript();
            int id =Convert.ToInt32(e.CommandArgument);
            GridViewRow row = GridView_Count.Rows[id];
            string CheckDate = row.Cells[0].Text.ToString();//Convert.ToString(this.GridView_Count.Rows[id].Cells[0].ToString());

                if (dt_wy != null && dt_his != null)
                {
                    try
                    {
                        tLoadHISRequest tloadhisRequest = new tLoadHISRequest(gethisReq);
                        IAsyncResult result = tloadhisRequest.BeginInvoke(CheckDate, CheckDate, null, null);
                        tLoadWYRequest tLoadwyRequest = new tLoadWYRequest(getReq);
                        IAsyncResult result2 = tLoadwyRequest.BeginInvoke(CheckDate, CheckDate, null, null);

                        string _requesthis = tloadhisRequest.EndInvoke(result);
                        string _request = tLoadwyRequest.EndInvoke(result2);

                        tLoadHISData tloadhis = new tLoadHISData(gethisXml);
                        IAsyncResult resulthis = tloadhis.BeginInvoke(_requesthis, null, null);
                        tLoadWYData tloadwy = new tLoadWYData(getXml);
                        IAsyncResult result_WY = tloadwy.BeginInvoke(_request, null, null);
                        string strhisxml = tloadhis.EndInvoke(resulthis);
                        string strxml = tloadwy.EndInvoke(result_WY);

                        dt_his = GetDBdata.XmlToDataTable(strhisxml);
                        dt_wy = GetDBdata.XmlToDataTable(strxml);
                        this.GridView.DataSource = dt_wy.DefaultView;
                        this.GridView.DataBind();
                        for (int i = dt_his.Rows.Count - 1; i >= 0; i--)
                        {
                            for (int k = 0; k < dt_wy.Rows.Count; k++)
                            {
                                if (dt_his.Rows[i][3].ToString() == dt_wy.Rows[k][11].ToString() && dt_his.Rows[i][13].ToString() == dt_wy.Rows[k][1].ToString() && dt_his.Rows[i][1].ToString() == dt_wy.Rows[k][2].ToString())
                                {
                                    GridView.Rows[k].BackColor = System.Drawing.Color.Green;
                                }

                            }

                        }
                    DetailsListTitle.Visible = true;
                    DetailsListTitle.Text = CheckDate + "的明细单。 备注：绿色是匹配成功的订单，白色是HIS没有的订单";
                }
                    catch (Exception ex)
                    {
                        Logging.WriteBuglog(ex);
                    }
                    finally
                    {
                   this.GridView.Visible = true;
                }

            }
            else
            {
                //this.GridView.Visible = true;
                
            }
        }
    }
}

