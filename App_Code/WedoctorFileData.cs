using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///WedoctorFileData 的摘要说明
/// </summary>
public class WedoctorFileData
{
    private string jysj;
    
    public string Jysj
    {
        get { return jysj; }
        set { jysj = value; }
    }

    private string ywptls;//业务平台流水

    public string Ywptls
    {
        get { return ywptls; }
        set { ywptls = value; }
    }
    private string ptjyls;//平台交易流水

    public string Ptjyls
    {
        get { return ptjyls; }
        set { ptjyls = value; }
    }
    private string ywtkls;//业务退款流水

    public string Ywtkls
    {
        get { return ywtkls; }
        set { ywtkls = value; }
    }
    private string pttkls;//平台退款流水
    private string fjydd;//反交易订单

    public string Fjydd
    {
        get { return fjydd; }
        set { fjydd = value; }
    }
    public string Pttkls
    {
        get { return pttkls; }
        set { pttkls = value; }
    }
    private string jsye;//交易金额

    public string Jsye
    {
        get { return jsye; }
        set { jsye = value; }
    }
    private string tkje;//退款金额

    public string Tkje
    {
        get { return tkje; }
        set { tkje = value; }
    }
    private string btje;//补贴金额

    public string Btje
    {
        get { return btje; }
        set { btje = value; }
    }
    private string sfje;//实付金额

    public string Sfje
    {
        get { return sfje; }
        set { sfje = value; }
    }
    private string stje;//实退金额

    public string Stje
    {
        get { return stje; }
        set { stje = value; }
    }
    private string dsfzfjylsh;//第三方支付交易流水号

    public string Dsfzfjylsh
    {
        get { return dsfzfjylsh; }
        set { dsfzfjylsh = value; }
    }
    private string dsfzftklsh;//第三方支付退款流水号

    public string Dsfzftklsh
    {
        get { return dsfzftklsh; }
        set { dsfzftklsh = value; }
    }
    private string zflx;//支付类型

    public string Zflx
    {
        get { return zflx; }
        set { zflx = value; }
    }
    private string jyzt;//交易状态

    public string Jyzt
    {
        get { return jyzt; }
        set { jyzt = value; }
    }
    private string tkzt;//退款状态

    public string Tkzt
    {
        get { return tkzt; }
        set { tkzt = value; }
    }
    private string shmc;//商户名称

    public string Shmc
    {
        get { return shmc; }
        set { shmc = value; }
    }
    private string spmc;//商品名称

    public string Spmc
    {
        get { return spmc; }
        set { spmc = value; }
    }
    private string shh;//商户号

    public string Shh
    {
        get { return shh; }
        set { shh = value; }
    }
    private string jryyid;//接入应用ID

    public string Jryyid
    {
        get { return jryyid; }
        set { jryyid = value; }
    }
    private string dsfshh;//第三方商户号

    public string Dsfshh
    {
        get { return dsfshh; }
        set { dsfshh = value; }
    }
	public WedoctorFileData()
	{
		//
		//TODO: 在此处添加构造函数逻辑
		//
	}
}