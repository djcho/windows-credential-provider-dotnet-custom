using CredentialProvider.Interop;
using Penta.EeWin.Cp.Base.Exception;
using Penta.EeWin.Cp.Base.Field;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static Penta.EeWin.Cp.Base.Constants;

namespace Penta.EeWin.Cp.Base.CredentialContext
{
    public class PasswordChangeCredentialContext : CredentialContext
    {
        private string oldPassword = string.Empty;
        private string newPassword = string.Empty;

        public PasswordChangeCredentialContext(in Guid cpClsid, in _CREDENTIAL_PROVIDER_USAGE_SCENARIO cpus)
            : base(cpClsid, cpus, CREDENTIAL_CONTEXT_SCENARIO.CCS_CHANGE_PASSWORD)
        {
        }

        public override void Initialize(
            in FieldController fieldController,
            in ICredentialProviderUser cpUser)
        {
            base.Initialize(fieldController, cpUser);
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
            PInvoke.KERB_CHANGEPASSWORD_REQUEST kcpr = new PInvoke.KERB_CHANGEPASSWORD_REQUEST();
            kcpr.DomainName = new PInvoke.UNICODE_STRING(this.DomainName);
            kcpr.AccountName = new PInvoke.UNICODE_STRING(this.userName);
            kcpr.OldPassword = new PInvoke.UNICODE_STRING(this.oldPassword);
            kcpr.NewPassword = new PInvoke.UNICODE_STRING(this.newPassword);
            kcpr.MessageType = PInvoke.KERB_PROTOCOL_MESSAGE_TYPE.KerbChangePasswordMessage;
            kcpr.Impersonating = 0;

            int hr = Helpers.KerbChangePasswordPack(kcpr, out pcpcs.rgbSerialization, out pcpcs.cbSerialization);
            if(hr < 0)
                throw new CredentialException(CredentialError.ErrorCode.PackageKerbPasswordFailed);

            hr = Helpers.RetrieveNegotiateAuthPackage(out var authPackage);
            if (hr < 0)
                throw new CredentialException(CredentialError.ErrorCode.RetrieveAuthPackageFailed);

            pcpcs.clsidCredentialProvider = this.cpClsid;
            pcpcs.ulAuthenticationPackage = authPackage;
            pcpgsr = _CREDENTIAL_PROVIDER_GET_SERIALIZATION_RESPONSE.CPGSR_RETURN_CREDENTIAL_FINISHED;
        }

        public override void ReportResult(in long status, 
            in long substatus, 
            ref string optionalStatusText, 
            ref _CREDENTIAL_PROVIDER_STATUS_ICON optionalStatusIcon,
            in FieldController fieldController,
            ref CREDENTIAL_CONTEXT_SCENARIO nextCcs)
        {
            base.ReportResult(status, substatus, ref optionalStatusText, ref optionalStatusIcon, fieldController, ref nextCcs);
        }

        public override CredentialProviderConnectResult Connect(ref IQueryContinueWithStatus qcws, in FieldController fieldController,
            ref CREDENTIAL_CONTEXT_SCENARIO ccs)
        {
            CredentialProviderConnectResult cpConnectResult = CredentialProviderConnectResult.ConnectResultDefault;
            Window window = fieldController.GetCredentialHwnd();

            Thread thread = new Thread(() => 
            {
                MessageBox.Show(window, "패스워드 변경에 성공하였습니다.", "비밀번호 변경");
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
    }
}
