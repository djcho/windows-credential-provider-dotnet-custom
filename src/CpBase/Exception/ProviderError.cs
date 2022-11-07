using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penta.EeWin.Cp.Base.Exception
{
    public static class ProviderError
    {
        public enum ErrorCode
        {
            CreateCredentialFailed = 1000,
            CreateFieldsFailed,
            EnumerateCredentialFailed,
            InitializeCredentialFailed,
        }
        public static string Parse(in ErrorCode errorCode)
        {
            switch (errorCode)
            { 
                case ErrorCode.CreateCredentialFailed: return "Failed creation credential.";
                case ErrorCode.CreateFieldsFailed: return "Field list creation failed.";
                case ErrorCode.EnumerateCredentialFailed: return "Failed to enumerate credentials.";
                case ErrorCode.InitializeCredentialFailed: return "Failed to initialize credentials.";
            }
            return string.Empty;
        }
    }
}
