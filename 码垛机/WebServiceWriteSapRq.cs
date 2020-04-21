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
    class WebServiceWriteSapRq
    {
        private static readonly string WebServiceDataToSap =

            "http://erpdev.cnppump.com:8000/sap/bc/srt/rfc/sap/zrfc_qm_sernr/400/zrfc_qm_sernr/zrfc_qm_sernr";//
                                                                                                              //  "http://erpdev.cnppump.com:8000/sap/bc/srt/wsdl/flv_10002A111AD1/bndg_url/sap/bc/srt/rfc/sap/zrfc_qm_sernr/400/zrfc_qm_sernr/zrfc_qm_sernr?sap-client=400";//"http://erpprd2.cnppump.com:8000/sap/bc/srt/rfc/sap/zws_zqm_stm/800/zws_zqm_stm/zqm_stm";

        /// <summary>
        ///  从WebService接口中读取 包裹的信息
        /// </summary>
        /// <param name="sernr"></param>
        ///   <param name="zaufnr"></param>
        ///    <param name="zmatnr"></param>
        /// <returns></returns>
        /// //SERNR

        //  后期将参数修改为 List 列表 同时匹配真是数据
        public static SapRetInfo WritePackinfoToSap(List<string> sernr, List<string> zaufnr, List<string> zmatnr)
        {
            int sLen = sernr.Count;
            int aLen = zaufnr.Count;
            int mLen = zmatnr.Count;


            if ((0 == sLen) || (0 == aLen) || (0 == mLen))
                return null;
            if ((sLen != aLen) || (mLen != aLen))
                return null;

            string strId = string.Empty;
            StringBuilder sbTemp = new StringBuilder();
            StringBuilder RequstSb = new StringBuilder();
            string strHeader =
                "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:urn=\"urn:sap-com:document:sap:soap:functions:mc-style\">" +
                              "<soapenv:Header/>" +
                              " <soapenv:Body>" +
                               "<urn:ZrfcQmSernr>" +
                                "<OtTab>";

            string strBoy = "<item>" +
                                 " <Mandt></Mandt>" +
                                 " <Znumber></Znumber>" +
                                 " <Zposnr></Zposnr>" +
                                 " <Sernr>{0}</Sernr> " +
                                  " <Zaufnr>{1}</Zaufnr> " +
                                  " <Zmatnr>{2}</Zmatnr> " +
                                  " <Zmaktx></Zmaktx> " +
                                  " <Zwerks></Zwerks> " +
                                  " <Zjsdat></Zjsdat> " +
                                  " <Zjssj></Zjssj> " +
                                  " <Zbgdat></Zbgdat> " +
                                  " <Zbgsj></Zbgsj> " +
                                  " <Zbgzt></Zbgzt> " +
                                  " <Zbgxx></Zbgxx> " +
                                  " <Zrueck></Zrueck> " +
                                  " <Zrkdat></Zrkdat> " +
                                   "<Zrksj></Zrksj> " +  //
                                  " <Zrkzt></Zrkzt> " +
                                  " <Zrkxx></Zrkxx> " +
                                  " <Zmblnr></Zmblnr> " +
                                  " <Zzeile></Zzeile> " +
                                  " </item>";


            for (int i = 0; i < sLen; i++)
            {
                if (string.IsNullOrEmpty(sernr[i]))
                {
                    continue;
                }
                if (string.IsNullOrEmpty(zaufnr[i]))
                {
                    continue;
                }
                if (string.IsNullOrEmpty(zmatnr[i]))
                {
                    continue;
                }
                sbTemp.Append(string.Format(strBoy, sernr[i], zaufnr[i], zmatnr[i]));
            }


            string strEnd = "</OtTab>" +
                                "</urn:ZrfcQmSernr>" +
                                "</soapenv:Body>" +
                               "</soapenv:Envelope>";

            RequstSb.Append(strHeader);
            RequstSb.Append(sbTemp);
            RequstSb.Append(strEnd);

            byte[] dataArray = Encoding.Default.GetBytes(RequstSb.ToString());
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(WebServiceDataToSap);
            request.Method = "POST";
            request.UserAgent =
               "Mozilla/5.0 (compatible;Windows NT 6.1; WOW64;Trident/6.0;MSIE 9.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/33.0.1750.27 Safari/537.36";
            request.ContentLength = dataArray.Length;
            request.ContentType = "text/xml;charset=UTF-8";
            request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"CNP_01:ABC123")));
            CredentialCache credentialCache = new CredentialCache();
            credentialCache.Add(new Uri(WebServiceDataToSap), "Basic", new NetworkCredential("CNP_01", "ABC123"));
            request.Credentials = credentialCache;
            request.Headers.Add("SOAPAction", "urn:sap-com:document:sap:soap:functions:mc-style:ZRFC_QM_SERNR:ZrfcQmSernrRequest");
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
            return GetSapReturnDataFromXML(res);
        }



        private static SapRetInfo GetSapReturnDataFromXML(string xml)
        {
            SapRetInfo retMsg = new SapRetInfo();
            try
            {
                XmlDocument x = new XmlDocument();
                x.LoadXml(xml);

                retMsg.Retmsg = x.GetElementsByTagName("Retmsg")[0].InnerText;
                return retMsg;
            }
            catch (Exception ex)
            {
                return retMsg;
            }

        }

        public class SapRetInfo
        {
            private string _Retmsg;

            public string Retmsg { get => _Retmsg; set => _Retmsg = value; }
        }
    }
}
