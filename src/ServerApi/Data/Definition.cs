
namespace Penta.EeWin.ServerApi.Data
{
    internal class UrlDefinition
    {
        internal const string ISIGN_CHECK_SERVER = "/api/v1/sso/checkserver";
        internal const string ISIGN_GET_AUTH_LIST = "/api/v1/policy";
    }

    internal class JsonKeyDefinition
    {
    }

    internal class JsonValueDefinition
    {
        internal const string CMD_CHECK_SERVER = "checkserver";
        internal const string CMD_GET_EEWIN_AUTH_LIST = "getEeWinAuthList";
    }

    internal class ResponseCodeDefinition
    {
        internal const string SUCCESS = "000000";

        internal const string APC_STATUS_OK = "S200.000";
        internal const string APC_SUCCESS = "0200.000";
    }
}
