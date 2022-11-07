
namespace Penta.EeWin.Common.Registry.Data
{
    internal class KeyPathDefinition
    {
        public const string EE_WIN = @"SOFTWARE\Penta Security Systems\Isign\EE-WIN";
        public const string EE_WIN_CP_LIST = @"SOFTWARE\\Penta Security Systems\\Isign\\EE-WIN\\LogonManager\\Credential Providers";
        public const string WINDOWS_AUTHENTICATION = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Authentication";
    }

    internal class SubKeyNameDefinition
    {
        public const string LOGON_MANAGER = "LogonManager";
        public const string LOGON_UI = "LogonUI";
    }

    internal class ValueNameDefinition
    {
        public const string RETRY_COUNT = "RetryCount";
        public const string NETWORK_SERVICE_WAITING_TIME = "NetworkServiceWaitingTime";
        public const string APC_ADDR = "SvrAddr";
        public const string APC_PORT = "SvrPort";
        public const string HOST_ADDR = "HOST_ADDR";
        public const string PORT_NUM = "PORT_NUM";
        public const string LAST_LOGGED_ON_SSO_ID = "LastLoggedOnSSOID";
        public const string LAST_LOGGED_ON_USER = "LastLoggedOnUser";
    }
}
