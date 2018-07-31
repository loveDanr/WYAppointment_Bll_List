<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<%@ Register Assembly="DevExpress.Web.v15.2, Version=15.2.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>微医预约挂号平台对账</title>
    <link id="cssfile" href="Css/style.css" rel="stylesheet" type="text/css" />
    <link id="cssfile2" href="layui/css/layui.css" rel="stylesheet" type="text/css" />
    <link rel="shortcut icon" href="Images/favicon.ico" type="image/x-icon" />
    <script type="text/javascript" src="layui/layui.js"></script>
     
   
<%-- <script type="text/javascript">
    
         $(document).ready(function () {
             $("#Button1").click(function () {
                 var options = {
                     url: 'Default.aspx?Action=SaveInfo',
                     type: 'post',
                     dataType: 'text',
                     data: $("#form1").serialize(),
                     success: function (data) {
                         if (data.length > 0)
                             $("#responseText").text(data);
                         //                            alert("保存成功！");
                         //                        else
                         //                            alert("保存失败");
                     }
                 };
                 $.ajax(options);
                 return false;
             });
         });
     
    </script>--%>

</head>
<body onload="remove_loading();">
    <form id="form1" runat="server">
    <div class="Main" align="center">
     <div class="top">
          <table border="0" class="tftableTop">
                <tr>
                    <td colspan="2" align="right">
                        <img src="Images/公司.jpg">
                        </td>
                        <td colspan="6" align="left" style="font-family:'Microsoft YaHei UI'; font-weight: bold; align-content:center; font-size: 35px;padding-bottom:30px; padding-left:100px;text-align:left;padding-left:10px;"><p style="padding-top:30px;">预约挂号平台对账</p>
                    </td>
                </tr>
                <tr style="width:auto;text-align:left">
                    <td align="left">
                        <label style="font-family:'Microsoft YaHei UI';font-weight: bold;width:100px">
                            开始时间:</label>
                         
                    </td>
                    <td align="left">
                     <dx:ASPxTimeEdit id="txt_startDate"   runat="server" displayformatstring="yyyy-MM-dd" editformat="Custom" editformatstring="yyyy-MM-dd" width="120px"></dx:ASPxTimeEdit>
                     
                    </td>
                    <td align="left">
                     <label style="font-family:'Microsoft YaHei UI';font-weight: bold">
                            结束时间:</label>
                    </td>
                        <td>
                            
                          <dx:ASPxTimeEdit id="txt_endDate"  runat="server" displayformatstring="yyyy-MM-dd" editformat="Custom" editformatstring="yyyy-MM-dd" width="120px"></dx:ASPxTimeEdit>
                    </td>
                    <div class="layui-btn-container">
   <td>   <asp:Button ID="Button1" runat="server" Text="查询" CssClass="layui-btn" OnClick="btnSearch_Click" /><td><asp:Button ID="Button2" runat="server" Text="下载" 
           CssClass="layui-btn"     onclick="download_btn_Click" />
</div>
                     <%--   <td>
                            <asp:Button ID="btnSearch" runat="server" Text="查询" onclick="btnSearch_Click" CssClass="layui-btn" /></td><td><asp:Button ID="download_btn" runat="server" Text="下载" 
             CssClass="layui-btn" onclick="download_btn_Click" /></td>--%>
                </tr>
                        
             <%-- <tr align="center">
                  <td colspan="4" align="center" style="font-weight: bold; font-size: 15px;padding-bottom:5px; padding-left:100px;text-align:center;color:#ff0000"><dx:ASPxLabel runat="server" ID="TotalDealCount" Font-Bold="True" Font-Size="15px" ></dx:ASPxLabel>   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<dx:ASPxLabel runat="server" ID="TotalFeeCount" Font-Size="15px" Font-Bold="True"></dx:ASPxLabel></td>
                  <td colspan="4" align="center" style="font-weight: bold; font-size: 15px;padding-bottom:5px; padding-left:100px;text-align:center;color:#ff0000"><dx:ASPxLabel runat="server" ID="HIS_TotalDealCount" Font-Bold="True" Font-Size="15px"></dx:ASPxLabel>   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<dx:ASPxLabel runat="server" ID="HIS_TotalFeeCount" Font-Size="15px" Font-Bold="True"></dx:ASPxLabel></td>
              </tr>--%>
              <tr align="center">
                  
                
                  <td colspan="4" align="center" style="font-weight: bold; font-size: 15px;padding-bottom:5px; padding-left:100px;text-align:center;color:#ff0000"><asp:Label runat="server" ID="Notice" Font-Bold="True" Font-Size="15px" Text=""></asp:Label></td>
              </tr>
            </table>
   
    </div>
        <div class="bottom2">
    
        <asp:GridView ID="GridView_Count" CssClass="layui-table" lay-size="lg" runat="server" 
            EmptyDataText="无" ShowFooter="True" Visible="true" OnRowDataBound="GridView_Count_RowDataBound" OnRowCreated="GridView_Count_RowCreated" AutoGenerateColumns="False" OnRowCommand="GridView_Count_RowCommand">
 
                <Columns>
                    <asp:BoundField DataField="TradeDatehis" HeaderText="挂号日期" />
                    <asp:BoundField DataField="Counthis" HeaderText="HIS交易记录数" />
                    <asp:BoundField DataField="Amounthis" HeaderText="HIS交易金额" />
                    <asp:BoundField DataField="TradeDate" HeaderText="微医交易日期" />
                    <asp:BoundField DataField="wxCount" HeaderText="微信交易数" />
                    <asp:BoundField DataField="wxAmount" HeaderText="微医交易金额" />
                    <asp:BoundField DataField="different" HeaderText="双方差额" />
                    <asp:ButtonField ButtonType="Button"  ItemStyle-CssClass="add" HeaderText="查看详情" ControlStyle-Font-Size="12px"  Text="查看明细" CommandName="Check" />
                </Columns>
 
               <%-- <HeaderStyle CssClass="DataGridFixedHeader" BorderStyle="Solid" />--%>
        </asp:GridView>
     
    </div>
    <div class="bottom">
      <table border="0" class="tftableTop">
          <tr>
              <td colspan="4"  style="font-weight: bold; font-size: 15px;padding-bottom:5px; padding-left:100px;text-align:left; float:left;color:#ff0000"><dx:ASPxLabel runat="server" ID="DetailsListTitle" Font-Bold="True" Font-Size="15px" Text=""></dx:ASPxLabel></td>
              
              </tr>
          </table>
        <asp:GridView ID="GridView" CssClass="msgtable" runat="server" 
            EmptyDataText="无" ShowFooter="True" OnRowDataBound="GridView_RowDataBound" 
            OnRowCreated="GridView_RowCreated" Visible="False">
                <HeaderStyle CssClass="DataGridFixedHeader" BorderStyle="Solid" />
        </asp:GridView>
      
    </div>
      <%--OnRowDataBound="GridView_RowDataBound" 
            OnRowCreated="GridView_RowCreated"--%>
        
    </div>
    
    </form>
</body>
</html>
