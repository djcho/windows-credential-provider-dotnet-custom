using CredentialProvider.Interop;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Penta.EeWin.Cp.Base
{
    public static class PInvoke
    {
        // From http://www.pinvoke.net/default.aspx/secur32/LsaLogonUser.html
        [StructLayout(LayoutKind.Sequential)]
        public struct LUID
        {
            public uint LowPart;
            public uint HighPart;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct UNICODE_STRING
        {
            public UInt16 Length;
            public UInt16 MaximumLength;
            public IntPtr Buffer;

            public UNICODE_STRING(string s)
            {
                Length = (ushort)(s.Length * 2);
                MaximumLength = (ushort)(Length);
                Buffer = Marshal.StringToHGlobalUni(s);
            }

            public void Dispose()
            {
                Marshal.FreeHGlobal(Buffer);
                Buffer = IntPtr.Zero;
            }

            public override string ToString()
            {
                return Marshal.PtrToStringUni(Buffer);
            }
        }

        public enum KERB_LOGON_SUBMIT_TYPE
        {
            KerbInteractiveLogon = 2,
            KerbSmartCardLogon = 6,
            KerbWorkstationUnlockLogon = 7,
            KerbSmartCardUnlockLogon = 8,
            KerbProxyLogon = 9,
            KerbTicketLogon = 10,
            KerbTicketUnlockLogon = 11,
            KerbS4ULogon = 12,
            KerbCertificateLogon = 13,
            KerbCertificateS4ULogon = 14,
            KerbCertificateUnlockLogon = 15,
            KerbNoElevationLogon = 83,
            KerbLuidLogon = 84,
        }
        public enum CRED_PROTECTION_TYPE
        {
            CredUnprotected = 0,
            CredUserProtection,
            CredTrustedProtection
        }

        public enum WINERROR
        {
            ERROR_INSUFFICIENT_BUFFER = 122,
        }

        public enum KERB_PROTOCOL_MESSAGE_TYPE
        {
            KerbDebugRequestMessage = 0,
            KerbQueryTicketCacheMessage,
            KerbChangeMachinePasswordMessage,
            KerbVerifyPacMessage,
            KerbRetrieveTicketMessage,
            KerbUpdateAddressesMessage,
            KerbPurgeTicketCacheMessage,
            KerbChangePasswordMessage,
            KerbRetrieveEncodedTicketMessage,
            KerbDecryptDataMessage,
            KerbAddBindingCacheEntryMessage,
            KerbSetPasswordMessage,
            KerbSetPasswordExMessage,
            KerbVerifyCredentialsMessage,
            KerbQueryTicketCacheExMessage,
            KerbPurgeTicketCacheExMessage,
            KerbRefreshSmartcardCredentialsMessage,
            KerbAddExtraCredentialsMessage,
            KerbQuerySupplementalCredentialsMessage,
            KerbTransferCredentialsMessage,
            KerbQueryTicketCacheEx2Message,
            KerbSubmitTicketMessage,
            KerbAddExtraCredentialsExMessage,
            KerbQueryKdcProxyCacheMessage,
            KerbPurgeKdcProxyCacheMessage,
            KerbQueryTicketCacheEx3Message,
            KerbCleanupMachinePkinitCredsMessage,
            KerbAddBindingCacheEntryExMessage,
            KerbQueryBindingCacheMessage,
            KerbPurgeBindingCacheMessage,
            KerbPinKdcMessage,
            KerbUnpinAllKdcsMessage,
            KerbQueryDomainExtendedPoliciesMessage,
            KerbQueryS4U2ProxyCacheMessage,
            KerbRetrieveKeyTabMessage,
            KerbRefreshPolicyMessage,
            KerbPrintCloudKerberosDebugMessage,
        }

        public struct KERB_INTERACTIVE_LOGON
        {
            public KERB_LOGON_SUBMIT_TYPE MessageType;
            public UNICODE_STRING LogonDomainName;
            public UNICODE_STRING UserName;
            public UNICODE_STRING Password;
        }

        public struct KERB_CHANGEPASSWORD_REQUEST
        {
            public KERB_PROTOCOL_MESSAGE_TYPE MessageType;
            public UNICODE_STRING DomainName;
            public UNICODE_STRING AccountName;
            public UNICODE_STRING OldPassword;
            public UNICODE_STRING NewPassword;
            public byte Impersonating;
        }       

        [StructLayout(LayoutKind.Sequential)]
        public struct KERB_INTERACTIVE_UNLOCK_LOGON
        {
            public KERB_INTERACTIVE_LOGON Logon;
            public LUID LogonId;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LSA_STRING
        {
            public UInt16 Length;
            public UInt16 MaximumLength;
            public /*PCHAR*/ IntPtr Buffer;
        }

        public class LsaStringWrapper : IDisposable
        {
            public LSA_STRING _string;

            public LsaStringWrapper(string value)
            {
                _string = new LSA_STRING();
                _string.Length = (ushort)value.Length;
                _string.MaximumLength = (ushort)value.Length;
                _string.Buffer = Marshal.StringToHGlobalAnsi(value);
            }

            ~LsaStringWrapper()
            {
                Dispose(false);
            }

            private void Dispose(bool disposing)
            {
                if (_string.Buffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(_string.Buffer);
                    _string.Buffer = IntPtr.Zero;
                }
                if (disposing)
                    GC.SuppressFinalize(this);
            }

#region IDisposable Members

            public void Dispose()
            {
                Dispose(true);
            }

#endregion
        }

        public enum NTSTATUS : long
        {
            STATUS_LOGON_FAILURE = 0xC000006DL,
            STATUS_SUCCESS = 0x00000000L,
            STATUS_ACCOUNT_RESTRICTION = 0xC000006EL,
            STATUS_ACCOUNT_DISABLED = 0xC0000072L,
            STATUS_PASSWORD_EXPIRED = 0xC0000071L,
            STATUS_PASSWORD_MUST_CHANGE = 0xC0000224L,
        };

        public static readonly _tagpropertykey PKEY_Identity_QualifiedUserName = new _tagpropertykey
        {
            fmtid = Guid.Parse("DA520E51-F4E9-4739-AC82-02E0A95C9030"),
            pid = 100
        };

        public static readonly _tagpropertykey PKEY_Identity_UserName = new _tagpropertykey
        {
            fmtid = Guid.Parse("C4322503-78CA-49C6-9ACC-A68E2AFD7B6B"),
            pid = 100
        };

        public static readonly _tagpropertykey PKEY_Identity_DisplayName = new _tagpropertykey
        {
            fmtid = Guid.Parse("7D683FC9-D155-45A8-BB1F-89D19BCB792F"),
            pid = 100
        };
        public static readonly _tagpropertykey PKEY_Identity_LogonStatusString = new _tagpropertykey
        {
            fmtid = Guid.Parse("F18DEDF3-337F-42C0-9E03-CEE08708A8C3"),
            pid = 100
        };

        public static readonly Guid CPFG_CREDENTIAL_PROVIDER_LOGO = Guid.Parse("2d837775-f6cd-464e-a745-482fd0b47493");
        public static readonly Guid CPFG_CREDENTIAL_PROVIDER_LABEL = Guid.Parse("286BBFF3-BAD4-438F-B007-79B7267C3D48");
        public static readonly string Identity_LocalUserProvider = "A198529B-730F-4089-B646-A12557F5665E";

        [DllImport("credui.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CredPackAuthenticationBuffer(
            int dwFlags,
            string pszUserName,
            string pszPassword,
            IntPtr pPackedCredentials,
            ref int pcbPackedCredentials);

        [DllImport("Advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CredProtect(
            bool fAsSelf,
            string szCredentials,
            int cchCredentials,
            StringBuilder szProtectedCredentials,
            ref uint pcchMaxChars,
            out CRED_PROTECTION_TYPE ProtectionType);

        [DllImport("Advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CredIsProtected(
            string szProtectedCredentials,
            out CRED_PROTECTION_TYPE pProtectionType);

        [DllImport("secur32.dll", SetLastError = false)]
        public static extern uint LsaConnectUntrusted([Out] out IntPtr lsaHandle);

        [DllImport("secur32.dll", SetLastError = false)]
        public static extern uint LsaLookupAuthenticationPackage([In] IntPtr lsaHandle, [In] ref LSA_STRING packageName, [Out] out UInt32 authenticationPackage);

        [DllImport("secur32.dll", SetLastError = false)]
        public static extern uint LsaDeregisterLogonProcess([In] IntPtr lsaHandle);

        public enum MessageBoxCheckFlags : uint
        {
            MB_OK = 0x00000000,
            MB_OKCANCEL = 0x00000001,
            MB_YESNO = 0x00000004,
            MB_ICONHAND = 0x00000010,
            MB_ICONQUESTION = 0x00000020,
            MB_ICONEXCLAMATION = 0x00000030,
            MB_ICONINFORMATION = 0x00000040
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int MessageBox(IntPtr hWnd, string lpText, string lpCaption, uint uType);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void CopyMemory(IntPtr destination, IntPtr source, int length);
    }
}
