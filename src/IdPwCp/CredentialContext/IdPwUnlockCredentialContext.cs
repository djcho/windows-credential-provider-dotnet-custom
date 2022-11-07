using CredentialProvider.Interop;
using Penta.EeWin.Cp.Base;
using Penta.EeWin.Cp.Base.CredentialContext;
using Penta.EeWin.Cp.Base.Field;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Penta.EeWin.Cp.Base.Constants;
using static Penta.EeWin.Cp.IdPwCp.Constants;
using System.Windows;

namespace Penta.EeWin.Cp.IdPwCp.CredentialContext
{
    public class IdPwUnlockCredentialContext : UnlockCredentialContext
    {
        public IdPwUnlockCredentialContext(in Guid cpClsid, in _CREDENTIAL_PROVIDER_USAGE_SCENARIO cpus) : base(cpClsid, cpus)
        {
        }
        public override void Initialize(in FieldController fieldController, in ICredentialProviderUser cpUser)
        {
            base.Initialize(fieldController, cpUser);

            fieldController.BeginFieldUpdated();
            fieldController.ShowField((uint)CpFiledId.SsoPw, true);
            fieldController.ShowField((uint)CpFiledId.OldPassword, false);
            fieldController.ShowField((uint)CpFiledId.NewPassword, false);
            fieldController.ShowField((uint)CpFiledId.ConfirmNewPassword, false);
            fieldController.FocusField((uint)CpFiledId.SsoId);
            fieldController.SetSubmitButtonValue((uint)CpFiledId.SsoPw);
            fieldController.SetFieldString((uint)CpFiledId.SsoPw, string.Empty);
            fieldController.SetFieldString((uint)CpFiledId.OldPassword, string.Empty);
            fieldController.SetFieldString((uint)CpFiledId.NewPassword, string.Empty);
            fieldController.SetFieldString((uint)CpFiledId.ConfirmNewPassword, string.Empty);

            //string ssoId;
            if (this.isDomainJoined && this.isLocalUser)
            {
                fieldController.SetFieldString((uint)CpFiledId.StatusText, "로컬 계정을 선택했습니다. SSO 로그인을 위해서는 기타사용자를 선택해 AD 계정으로 로그인하시기 바랍니다.");
                fieldController.ShowField((uint)CpFiledId.SsoId, false);
                fieldController.ShowField((uint)CpFiledId.SsoPw, false);
                fieldController.ShowField((uint)CpFiledId.SubmitButton, false);
            }
            else if (!this.isOtherUser)
            {
                if (this.isDomainJoined)
                {
                    fieldController.SetFieldString((uint)CpFiledId.StatusText, "SSO 아이디/패스워드를 입력해 주세요.");
                    fieldController.SetFieldString((uint)CpFiledId.SsoId, this.userName);
                    fieldController.ShowField((uint)CpFiledId.SsoId, false);
                    fieldController.FocusField((uint)CpFiledId.SsoPw);
                }

                /*
                 * if (AccountRepo::getLastLogonSsoId(ssoId) == ACCOUNTREPO_SUCCESS)
                 * {
                 * }
                 */

            }

            fieldController.EndFieldUpdated();
        }

        public override CredentialProviderConnectResult Connect(ref IQueryContinueWithStatus qcws, in FieldController fieldController,
                    ref CREDENTIAL_CONTEXT_SCENARIO ccs)
        {
            CredentialProviderConnectResult cpConnectResult = CredentialProviderConnectResult.ConnectResultDefault;
            Window window = fieldController.GetCredentialHwnd();

            IQueryContinueWithStatus queryContinueWithStatus = qcws;

            Thread thread = new Thread(() =>
            {
                queryContinueWithStatus.SetStatusMessage("Password 인증 진행 중...");

                // 블라블라

                MessageBox.Show(window, "로그인 성공.", "비밀번호 변경");
                cpConnectResult = CredentialProviderConnectResult.ConnectResultSuccess;
            });

            thread.Start();

            while (qcws.QueryContinue() == HResultValues.S_OK)
            {
                if (cpConnectResult != CredentialProviderConnectResult.ConnectResultDefault)
                    break;
                Thread.Sleep(100);
            }

            // 취소나 인증 실패 시 DisConnect 버튼이 뜨지 않도록 처리한 것.
            // S_OK를 return 하면 CP화면에 DisConnect 버튼이 생성된다.
            // GetSerialization 함수는 Connect 의 return과 관계없이 호출되며 FAIL로 고정해도 다른 문제가 일어나지 않는 것을 확인함

            return CredentialProviderConnectResult.ConnectResultSuccess;
        }

        public override void GetSerialization(
            ref _CREDENTIAL_PROVIDER_GET_SERIALIZATION_RESPONSE pcpgsr,
            ref _CREDENTIAL_PROVIDER_CREDENTIAL_SERIALIZATION pcpcs,
            ref string optionalStatusText,
            ref _CREDENTIAL_PROVIDER_STATUS_ICON optionalStatusIcon,
            ref CredentialProviderConnectResult cpConnectResult,
            in FieldController fieldController,
            ref CREDENTIAL_CONTEXT_SCENARIO nextCcs)
        {
            string userPassword = string.Empty;
            if (cpConnectResult == CredentialProviderConnectResult.ConnectResultSuccess)
            {
                userPassword = fieldController.GetfieldString((uint)CpFiledId.SsoPw);
                if (!this.isDomainJoined)
                {

                }
            }

            base.userPassword = userPassword;
            base.GetSerialization(ref pcpgsr, ref pcpcs, ref optionalStatusText, ref optionalStatusIcon, ref cpConnectResult, fieldController, ref nextCcs);
        }

        public override void ReportResult(
            in long status,
            in long substatus,
            ref string optionalStatusText,
            ref _CREDENTIAL_PROVIDER_STATUS_ICON optionalStatusIcon,
            in FieldController fieldController,
            ref CREDENTIAL_CONTEXT_SCENARIO nextCcs)
        {
            try
            {
                base.ReportResult(status, substatus, ref optionalStatusText, ref optionalStatusIcon, fieldController, ref nextCcs);

                if (!string.IsNullOrEmpty(fieldController.GetfieldString((uint)CpFiledId.SsoId)) &&
                    string.IsNullOrEmpty(fieldController.GetfieldString((uint)CpFiledId.SsoPw)))
                {
                    //setLastLogonSsoId
                    //encryptAndSaveSsoPw
                }
            }
            catch (Exception ex)
            {
                Log.LogText(ex.Message);
                fieldController.SetFieldString((uint)CpFiledId.SsoPw, string.Empty);
            }
        }
    }
}
