
namespace Penta.EeWin.ServerApi.Data
{
    public class RequestInfo
    {
        private string url;
        private string port;
        private int retryCount;

        public string Url { get => url; }
        public string Port { get => port; }
        public int RetryCount { get => retryCount; }

        public RequestInfo(string url, string port, int retryCount = 0)
        {
            this.url = url;
            this.port = port;
            this.retryCount = retryCount;
        }
    }
}
