using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penta.EeWin.Cp.Base.Exception
{
    public static class CredentialError
    {
        public enum ErrorCode
        {
            InitializeKerbUnLockLogonFailed = 2000,
            ProtecPasswordFailed,
            PackageKerbUnlockLogonFailed,
            PackageKerbPasswordFailed,
            RetrieveAuthPackageFailed,
            PasswordExpired,
        }
        public static string Parse(in ErrorCode errorCode)
        {
            switch (errorCode)
            { 
                case ErrorCode.InitializeKerbUnLockLogonFailed: return "Failed to initialize KERB_INTERACTIVE_UNLOCK_LOGON struct.";
                case ErrorCode.ProtecPasswordFailed: return "Failed to protect password.";
                case ErrorCode.PackageKerbUnlockLogonFailed: return "Failed to package KERB_INTERACTIVE_UNLOCK_LOGON struct.";
                case ErrorCode.PackageKerbPasswordFailed: return "Failed to package KERB_CHANGEPASSWORD_REQUEST struct.";
                case ErrorCode.RetrieveAuthPackageFailed: return "Failed to retrieve auth pacakge.";
                case ErrorCode.PasswordExpired: return "Password expired.";
            }
            return string.Empty;
        }
    }
}
