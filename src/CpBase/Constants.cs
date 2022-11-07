using CredentialProvider.Interop;
using System;
using System.Runtime.InteropServices;

namespace Penta.EeWin.Cp.Base
{
    public static class Constants
    {        
        public enum CredentialProviderConnectResult
        {
            ConnectResultSuccess = 0,
            ConnectResultDefault,
            ConnectResultUserCanceled,
            ConnectResultInputEmpty,
            ConnectResultInvalidInput,
            ConnectResultFailed,
        };

        public static readonly int CredentialProviderNoDefault = -1;
    }
}

