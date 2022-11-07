using System;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Threading;
using System.Web.Script.Serialization;
using Penta.EeWin.ServerApi.Data.Response;
using Penta.EeWin.ServerApi.Data;

namespace Penta.EeWin.ServerApi
{
    public class GetAuthList
    {
        RequestInfo requestInfo;

        public GetAuthList(in RequestInfo requestInfo)
        {
            this.requestInfo = requestInfo;
        }

        public void Request(in string agentId, in string userId, out AuthList response)
        {
            response = new AuthList();
            int retryCount = this.requestInfo.RetryCount;

            do
            {
                try
                {
                    Uri uri = new Uri(this.requestInfo.Url + ":" + this.requestInfo.Port.ToString() + UrlDefinition.ISIGN_GET_AUTH_LIST
                        + "?agentId=" + agentId + "&login=" + userId);
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
                    httpWebRequest.Method = HttpMethod.Get.ToString();
                    httpWebRequest.ContentType = "application/json";

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
                            response = javaScriptSerializer.Deserialize<AuthList>(responseJsonText);

                            if (response.resultCode != ResponseCodeDefinition.APC_SUCCESS)
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
                catch (Exception ex)
                {
                    if (retryCount <= 0)
                        throw ex;

                    Thread.Sleep(1000);
                }
            } while (retryCount-- > 0);
        }
    }
}