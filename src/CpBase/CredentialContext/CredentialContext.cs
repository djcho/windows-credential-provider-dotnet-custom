using CredentialProvider.Interop;
using Penta.EeWin.Cp.Base.Exception;
using Penta.EeWin.Cp.Base.Field;
using System;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static Penta.EeWin.Cp.Base.Constants;
using static Penta.EeWin.Cp.Base.PInvoke;

namespace Penta.EeWin.Cp.Base.CredentialContext
{
    public enum CREDENTIAL_CONTEXT_SCENARIO
    {
        CCS_NONE = 0,
        CCS_LOGON = (CCS_NONE + 1),
        CCS_UNLOCK_WORKSTATION = (CCS_LOGON + 1),
        CCS_CHANGE_PASSWORD = (CCS_UNLOCK_WORKSTATION + 1),
    }

    public abstract class CredentialContext
    {
        protected Guid cpClsid;
        protected CREDENTIAL_CONTEXT_SCENARIO ccs;
        protected _CREDENTIAL_PROVIDER_USAGE_SCENARIO cpus;
        protected string userName;
        protected string domainName;
        protected string qualifiedUserName;
        protected string userSid;
        protected string logonStatus;
        protected string userPassword;
        protected bool isDomainJoined;
        protected bool isLocalUser;
        protected bool isOtherUser;
        protected bool isLockedUser;

        public CREDENTIAL_CONTEXT_SCENARIO Ccs { get { return ccs; } set { ccs = value; } }
        public _CREDENTIAL_PROVIDER_USAGE_SCENARIO Cpus { get { return cpus; } set { cpus = value; } }
        public string UserName { get { return userName; } set { userName = value; } }
        public string DomainName { get { return domainName; } set { domainName = value; } }
        public string QualifiedUsernName { get { return qualifiedUserName; } set { qualifiedUserName = value; } }
        public string UserSid { get { return userSid; } set { userSid = value; } }
        public string LogonStatus { get { return logonStatus; } set { logonStatus = value; } }
        public string UserPassword { set { userPassword = value; } }
        public bool IsDomainJoined { get { return isDomainJoined; } set { isDomainJoined = value; } }
        public bool IsLocalUser { get { return isLocalUser; } set { isLocalUser = value; } }
        public bool IsOtherUser { get { return isOtherUser; } set { isOtherUser = value; } }
        public bool IsLockUser { get { return isLockedUser; } set { isLockedUser = value; } }

        public CredentialContext(in Guid cpClsid, in _CREDENTIAL_PROVIDER_USAGE_SCENARIO cpus, in CREDENTIAL_CONTEXT_SCENARIO ccs)
        {
            this.cpClsid = cpClsid;
            this.cpus = cpus;
            this.ccs = ccs;
            this.isDomainJoined = false;
            this.isLocalUser = false;
            this.isOtherUser = false;
            this.isLockedUser = false;
        }

        public virtual void Initialize(in FieldController fieldController, in ICredentialProviderUser cpUser)
        {
            Log.LogMethodCall();
            // 기타 사용자 여부 얻기
            this.isOtherUser = true;
            this.isLocalUser = false;
            
            if (cpUser != null)            
            {
                this.isOtherUser = false;
                cpUser.GetStringValue(PKEY_Identity_UserName, out this.userName);
                cpUser.GetStringValue(PKEY_Identity_QualifiedUserName, out this.qualifiedUserName);
                cpUser.GetStringValue(PKEY_Identity_LogonStatusString, out this.logonStatus);
                cpUser.GetSid(out this.userSid);

                this.isLockedUser = Helpers.ParseLogonStatus(this.logonStatus);

                if (Helpers.SplitDomainAndUsername(this.qualifiedUserName, out this.domainName, out this.userName))
                {
                    //사용자 정보로 넘어온 도메인명이 컴퓨터 이름일 경우 : 로컬 사용자
                    if (this.domainName == Environment.MachineName)
                        isLocalUser = true;
                }
            }

            //도메인 이름, 도메인 조인 여부 얻기
            this.domainName = Environment.MachineName;
            this.isDomainJoined = SysInfo.SysInfo.IsInDomain();
            if(!isDomainJoined)
            {
                this.domainName = Environment.MachineName;
                this.isLocalUser = true;
            }

           // this.domainName = SysInfo.SysInfo.GetDomainName(); 
        }

        public abstract CredentialProviderConnectResult Connect(ref IQueryContinueWithStatus qcws, in FieldController fieldController,
            ref CREDENTIAL_CONTEXT_SCENARIO ccs);

        public virtual void GetSerialization(
            ref _CREDENTIAL_PROVIDER_GET_SERIALIZATION_RESPONSE pcpgsr,
            ref _CREDENTIAL_PROVIDER_CREDENTIAL_SERIALIZATION pcpcs,
            ref string optionalStatusText,
            ref _CREDENTIAL_PROVIDER_STATUS_ICON optionalStatusIcon,
            ref CredentialProviderConnectResult cpConnectResult,
            in FieldController fieldController,
            ref CREDENTIAL_CONTEXT_SCENARIO nextCcs)
        {
            if (cpConnectResult != CredentialProviderConnectResult.ConnectResultSuccess)
                throw new InvalidOperationException();

            string protectedPassword;
            int hr = Helpers.ProtectIfNecessaryAndCopyPassword(this.userPassword, this.cpus, out protectedPassword);
            if (hr < 0)
                throw new CredentialException(CredentialError.ErrorCode.ProtecPasswordFailed);

            PInvoke.KERB_INTERACTIVE_UNLOCK_LOGON kerbInteractiveUnlockLogon;
            hr = Helpers.KerbInteractiveUnlockLogonInit(this.domainName, this.userName, protectedPassword, this.cpus, out kerbInteractiveUnlockLogon);
            if (hr < 0)
                throw new CredentialException(CredentialError.ErrorCode.InitializeKerbUnLockLogonFailed);

            hr = Helpers.KerbInteractiveUnlockLogonPack(kerbInteractiveUnlockLogon, out pcpcs.rgbSerialization, out pcpcs.cbSerialization);
            if (hr < 0)
                throw new CredentialException(CredentialError.ErrorCode.PackageKerbUnlockLogonFailed);

            hr = Helpers.RetrieveNegotiateAuthPackage(out var authPackage);
            if (hr < 0)
                throw new CredentialException(CredentialError.ErrorCode.RetrieveAuthPackageFailed);

            pcpcs.clsidCredentialProvider = this.cpClsid;
            pcpcs.ulAuthenticationPackage = authPackage;
            pcpgsr = _CREDENTIAL_PROVIDER_GET_SERIALIZATION_RESPONSE.CPGSR_RETURN_CREDENTIAL_FINISHED;
        }

        public virtual void ReportResult(in long status, 
            in long substatus, 
            ref string optionalStatusText, 
            ref _CREDENTIAL_PROVIDER_STATUS_ICON optionalStatusIcon,
            in FieldController fieldController,
            ref CREDENTIAL_CONTEXT_SCENARIO nextCcs)
        {
            optionalStatusText = string.Empty;
            optionalStatusIcon = _CREDENTIAL_PROVIDER_STATUS_ICON.CPSI_NONE;

            if((long)NTSTATUS.STATUS_LOGON_FAILURE == status && (long)NTSTATUS.STATUS_SUCCESS == substatus)
            {
                optionalStatusText = "비밀번호 또는 사용자 이름이 올바르지 않습니다.";
                optionalStatusIcon = _CREDENTIAL_PROVIDER_STATUS_ICON.CPSI_ERROR;
            }
            else if ((long)NTSTATUS.STATUS_ACCOUNT_RESTRICTION == status && (long)NTSTATUS.STATUS_ACCOUNT_DISABLED == substatus)
            {
                optionalStatusText = "잠긴 계정입니다.";
                optionalStatusIcon = _CREDENTIAL_PROVIDER_STATUS_ICON.CPSI_WARNING;
            }
            else if ((long)NTSTATUS.STATUS_ACCOUNT_RESTRICTION == status && (long)NTSTATUS.STATUS_ACCOUNT_DISABLED == substatus)
            {
                optionalStatusText = "잠긴 계정입니다.";
                optionalStatusIcon = _CREDENTIAL_PROVIDER_STATUS_ICON.CPSI_WARNING;
            }

            if ((status | 0x10000000) < 0)
            {
                // 패스워드 변경을 해야 할 경우
                if ((status == (long)NTSTATUS.STATUS_PASSWORD_EXPIRED ||
                    status == (long)NTSTATUS.STATUS_PASSWORD_MUST_CHANGE) ||
                    (status == (long)NTSTATUS.STATUS_ACCOUNT_RESTRICTION && substatus == (long)NTSTATUS.STATUS_PASSWORD_EXPIRED))
                    throw new CredentialException(CredentialError.ErrorCode.PasswordExpired);
            }
        }
    }
}
