using System;
using System.IO;
using System.Net;
using System.Net.Http;
using Penta.EeWin.ServerApi.Data;
using System.Threading;
using System.Web.Script.Serialization;
using Penta.EeWin.ServerApi.Data.Response;

namespace Penta.EeWin.ServerApi
{
    public class CheckServer
    {
        RequestInfo requestInfo;

        public CheckServer(in RequestInfo requestInfo)            
        {
            this.requestInfo = requestInfo;
        }

        public void Request()
        {
            int retryCount = this.requestInfo.RetryCount;

            do
            {
                try
                {
                    Uri uri = new Uri(this.requestInfo.Url + ":" + this.requestInfo.Port.ToString() + UrlDefinition.ISIGN_CHECK_SERVER);
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
                    httpWebRequest.Method = HttpMethod.Get.ToString();

                    using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                    {
                        if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                        {
                            string responseJsonText;
                            Stream respStream = httpWebResponse.GetResponseStream();
                            using (StreamReader sr = new StreamReader(respStream))
                            {
                                responseJsonText = sr.ReadToEnd();
                            }

                            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                            Response response = javaScriptSerializer.Deserialize<AuthList>(responseJsonText);

                            if (response.resultCode != ResponseCodeDefinition.APC_STATUS_OK)
                            {
                                throw new WebException();
                            }

                            respStream.Close();
                            httpWebResponse.Close();
                        }
                        else
                        {
                            throw new WebException();
                        }
                    }
                } 
                catch(Exception ex)
                {
                    if (retryCount <= 0)
                        throw ex;

                    Thread.Sleep(1000);
                }
            } while (retryCount-- > 0);
        }
    }
}
