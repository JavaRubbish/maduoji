using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace 码垛机
{
    class WebServiceRq
    {
        private static readonly string WebServiceurl = "http://erpprd2.cnppump.com:8000/sap/bc/srt/rfc/sap/zws_zqm_stm/800/zws_zqm_stm/zqm_stm";

        /// <summary>
        ///  从WebService接口中读取 包裹的信息
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static PackageInfo GetPackinfoFromWeb(string pid)
        {
            if (string.IsNullOrEmpty(pid))
                return null;

            string strId = string.Empty;
            // strId = "1909073152"; // debug
            string strtemp1 = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:urn=\"urn:sap-com:document:sap:soap:functions:mc-style\">" +
                              "<soapenv:Header/> " +
                              " <soapenv:Body> " +
                              " <urn:ZqmStm> " +
                              "<Sernr>" + pid + " </Sernr > " +   //1909076709
                              "</urn:ZqmStm> " +
                              " </soapenv:Body> " +
                              " </soapenv:Envelope> ";



            byte[] dataArray = Encoding.Default.GetBytes(strtemp1);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(WebServiceurl);
            request.Method = "POST";
            request.UserAgent =
               "Mozilla/5.0 (compatible;Windows NT 6.1; WOW64;Trident/6.0;MSIE 9.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/33.0.1750.27 Safari/537.36";
            request.ContentLength = dataArray.Length;
            request.ContentType = "text/xml;charset=UTF-8";
            request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"sap_cnp:12345678")));
            CredentialCache credentialCache = new CredentialCache();
            credentialCache.Add(new Uri(WebServiceurl), "Basic", new NetworkCredential("sap_cnp", "12345678"));
            request.Credentials = credentialCache;
            request.Headers.Add("SOAPAction", "urn:sap-com:document:sap:soap:functions:mc-style:ZWS_ZQM_STM:ZqmStmRequest");
            Stream dataStream = null;
            try
            {
                dataStream = request.GetRequestStream();
            }
            catch
            {
                return null;
            }

            dataStream.Write(dataArray, 0, dataArray.Length);
            dataStream.Close();
            //读取返回消息
            string res = string.Empty;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                res = reader.ReadToEnd();
                reader.Close();
            }
            catch (Exception ex)
            {
                return null;
            }
            return GetPackinfoFromWeb(res);
        }



        private PackageInfo getPackInfoFromXML(string xml)
        {
            PackageInfo pi = new PackageInfo();
            try
            {
                XmlDocument x = new XmlDocument();
                x.LoadXml(xml);

                pi.Maktx = x.GetElementsByTagName("Maktx")[0].InnerText;
                pi.Matnr = x.GetElementsByTagName("Matnr")[0].InnerText;
                pi.Laeng = x.GetElementsByTagName("Laeng")[0].InnerText;
                pi.Breit = x.GetElementsByTagName("Breit")[0].InnerText;
                pi.Hoehe = x.GetElementsByTagName("Hoehe")[0].InnerText;
                pi.Meabm = x.GetElementsByTagName("Meabm")[0].InnerText;
                pi.Brgew = x.GetElementsByTagName("Brgew")[0].InnerText;
                pi.Ntgew = x.GetElementsByTagName("Ntgew")[0].InnerText;
                pi.Gewei = x.GetElementsByTagName("Gewei")[0].InnerText;
                pi.Ppaufnr = x.GetElementsByTagName("Ppaufnr")[0].InnerText;
                pi.Ppposnr = x.GetElementsByTagName("Ppposnr")[0].InnerText;
                pi.Obknr = x.GetElementsByTagName("Obknr")[0].InnerText;
            }
            catch
            {

            }
            return pi;
        }
    }


    public class PackageInfo
    {
        //物料编号
        private string _Matnr;
        //物料描述
        private string _Maktx;
        //长度
        private string _Laeng;
        //宽度
        private string _Breit;
        //高度
        private string _Hoehe;
        //长度/宽度/高度的尺寸单位
        private string _Meabm;
        //毛重
        private string _Brgew;
        //净重
        private string _Ntgew;
        //重量单位
        private string _Gewei;
        //订单号
        private string _Ppaufnr;
        //订单项目号
        private string _Ppposnr;
        //对象列表编号
        private string _Obknr;

        public string Matnr { get => _Matnr; set => _Matnr = value; }
        public string Maktx { get => _Maktx; set => _Maktx = value; }
        public string Laeng { get => _Laeng; set => _Laeng = value; }
        public string Breit { get => _Breit; set => _Breit = value; }
        public string Hoehe { get => _Hoehe; set => _Hoehe = value; }
        public string Meabm { get => _Meabm; set => _Meabm = value; }
        public string Brgew { get => _Brgew; set => _Brgew = value; }
        public string Ntgew { get => _Ntgew; set => _Ntgew = value; }
        public string Gewei { get => _Gewei; set => _Gewei = value; }
        public string Ppaufnr { get => _Ppaufnr; set => _Ppaufnr = value; }
        public string Ppposnr { get => _Ppposnr; set => _Ppposnr = value; }
        public string Obknr { get => _Obknr; set => _Obknr = value; }
    }
}
