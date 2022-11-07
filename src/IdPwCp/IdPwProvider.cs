using CredentialProvider.Interop;
using Penta.EeWin.Cp.Base;
using Penta.EeWin.Cp.Base.Field;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Penta.EeWin.Cp.IdPwCp.Constants;

namespace Penta.EeWin.Cp.IdPwCp
{
    [ComVisible(true)]
    [Guid(Constants.IdPwCpGuid)]
    [ClassInterface(ClassInterfaceType.None)]
    public class IdPwProvider : Provider
    {
        public override Credential CreateCredential()
        {
            Log.LogMethodCall();
            return new IdPwCredential();
        }

        public override List<Field> CreateFieldList()
        {
            Log.LogMethodCall();
            List<Field> fields = new List<Field>();
            fields.Add(new TileImageField((uint)CpFiledId.TileImage, "image", _CREDENTIAL_PROVIDER_FIELD_STATE.CPFS_DISPLAY_IN_SELECTED_TILE,
                _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE.CPFIS_NONE, Properties.Resources.tileimage));
            fields.Add(new SmallTextField((uint)CpFiledId.Tooltip, "Password", _CREDENTIAL_PROVIDER_FIELD_STATE.CPFS_HIDDEN,
                _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE.CPFIS_NONE));
            fields.Add(new LargeTextField((uint)CpFiledId.IpText, " ", _CREDENTIAL_PROVIDER_FIELD_STATE.CPFS_HIDDEN,
                _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE.CPFIS_NONE));
            fields.Add(new LargeTextField((uint)CpFiledId.StatusText, "SSO 아이디/패스워드를 입력해 주세요.", _CREDENTIAL_PROVIDER_FIELD_STATE.CPFS_DISPLAY_IN_SELECTED_TILE,
                _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE.CPFIS_NONE));
            fields.Add(new EditTextField((uint)CpFiledId.SsoId, "SSO 아이디", _CREDENTIAL_PROVIDER_FIELD_STATE.CPFS_DISPLAY_IN_SELECTED_TILE,
                _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE.CPFIS_NONE));
            fields.Add(new PasswordTextField((uint)CpFiledId.SsoPw, "SSO 패스워드", _CREDENTIAL_PROVIDER_FIELD_STATE.CPFS_DISPLAY_IN_SELECTED_TILE,
                _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE.CPFIS_NONE));
            fields.Add(new PasswordTextField((uint)CpFiledId.OldPassword, "현재 패스워드", _CREDENTIAL_PROVIDER_FIELD_STATE.CPFS_HIDDEN,
                _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE.CPFIS_NONE));
            fields.Add(new PasswordTextField((uint)CpFiledId.NewPassword, "새 패스워드", _CREDENTIAL_PROVIDER_FIELD_STATE.CPFS_HIDDEN,
                _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE.CPFIS_NONE));
            fields.Add(new PasswordTextField((uint)CpFiledId.ConfirmNewPassword, "새 패스워드 확인", _CREDENTIAL_PROVIDER_FIELD_STATE.CPFS_HIDDEN,
                _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE.CPFIS_NONE));
            fields.Add(new SubmitButtonField((uint)CpFiledId.SubmitButton, "submit", _CREDENTIAL_PROVIDER_FIELD_STATE.CPFS_DISPLAY_IN_SELECTED_TILE,
                _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE.CPFIS_NONE, (int)CpFiledId.SsoPw));
            fields.Add(new LargeTextField((uint)CpFiledId.CpType, "현재 선택된 로그인 옵션 : PASSWORD", _CREDENTIAL_PROVIDER_FIELD_STATE.CPFS_DISPLAY_IN_SELECTED_TILE,
                _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE.CPFIS_NONE));
            fields.Add(new ComboBoxField((uint)CpFiledId.EtcComboBox, "기타 메뉴", _CREDENTIAL_PROVIDER_FIELD_STATE.CPFS_DISPLAY_IN_SELECTED_TILE,
                _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE.CPFIS_NONE));

            return fields;
        }
    }
}
