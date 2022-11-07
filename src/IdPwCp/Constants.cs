using CredentialProvider.Interop;
using System;
using System.Runtime.InteropServices;

namespace Penta.EeWin.Cp.IdPwCp
{
    public static class Constants
    {
        public const string IdPwCpGuid = "CDF18C31-0387-4B77-9306-2D6057D8AC13";
        public enum CpFiledId
        {
            TileImage= 0,
            Tooltip,
            IpText,
            StatusText,
            SsoId,
            SsoPw,
            OldPassword,           // for change password
            NewPassword,           // for change password
            ConfirmNewPassword,   // for change password
            SubmitButton,
            CpType,
            EtcComboBox,
            NumField
        };
    }
}

