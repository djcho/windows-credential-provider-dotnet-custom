namespace Penta.EeWin.Cp.Base
{
    using System;
    using System.Collections.Generic;
    using System.DirectoryServices.ActiveDirectory;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Windows;
    using System.Windows.Documents;
    using global::CredentialProvider.Interop;
    using Penta.EeWin.Cp.Base;
    using Penta.EeWin.Cp.Base.Exception;
    using Penta.EeWin.Cp.Base.Field;
    using static Penta.EeWin.Cp.Base.Exception.ProviderError;
    using static Penta.EeWin.Cp.Base.PInvoke;

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class Provider : ICredentialProvider, ICredentialProviderSetUserArray
    {
        private ICredentialProviderEvents credentialProviderEvents;
        private _CREDENTIAL_PROVIDER_USAGE_SCENARIO cpus = _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_INVALID;
        private ICredentialProviderUserArray credProviderUserArray = null;
        private List<CredentialPair> credentialPairList = new List<CredentialPair>();
        private bool flagRecreateCredentials = false;
        private ulong upAdviseContext;


        public Provider()
        {
            Log.LogMethodCall();
            Log.LogText("CSharpSampleProvider: Created object");
        }

        public abstract Credential CreateCredential();
        public abstract List<Field.Field> CreateFieldList();

        public CredentialPair CreateCredentialPair()
        {
            Log.LogMethodCall();
            // 1. CCredential * 생성
            Credential credential = CreateCredential();
            if (credential == null)
                throw new ProviderException(ErrorCode.CreateCredentialFailed);

            // 2. FieldList * 생성
            List<Field.Field> fields = CreateFieldList();
            if (fields == null)
                throw new ProviderException(ErrorCode.CreateFieldsFailed);

            // 3. CredentialPair 생성
            CredentialPair credentialPair = new CredentialPair(credential, fields);
            if (credentialPair == null)
                throw new OutOfMemoryException();

            return credentialPair;
        }
        // SetUsageScenario is the provider's cue that it's going to be asked for tiles
        // in a subsequent call.
        public int SetUsageScenario(_CREDENTIAL_PROVIDER_USAGE_SCENARIO cpus, uint dwFlags)
        {
            Log.LogMethodCall();

            int hr = HResultValues.S_OK;
            // Decide which scenarios to support here. Returning E_NOTIMPL simply tells the caller
            // that we're not designed for that scenario.
            switch (cpus)
            {               
                case _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_LOGON:
                case _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_UNLOCK_WORKSTATION:
                case _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_CHANGE_PASSWORD:
                    // The reason why we need _fRecreateEnumeratedCredentials is because ICredentialProviderSetUserArray::SetUserArray() is called after ICredentialProvider::SetUsageScenario(),
                    // while we need the ICredentialProviderUserArray during enumeration in ICredentialProvider::GetCredentialCount()
                    this.cpus = cpus;
                    flagRecreateCredentials = true;
                    hr = HResultValues.S_OK;
                    break;
                case _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_CREDUI:
                    hr = HResultValues.E_NOTIMPL;
                    break;                    
                default:
                    hr = HResultValues.E_INVALIDARG;
                    break;
            }

            return hr;
        }

        // SetSerialization takes the kind of buffer that you would normally return to LogonUI for
        // an authentication attempt.  It's the opposite of ICredentialProviderCredential::GetSerialization.
        // GetSerialization is implement by a credential and serializes that credential.  Instead,
        // SetSerialization takes the serialization and uses it to create a tile.
        //
        // SetSerialization is called for two main scenarios.  The first scenario is in the credui case
        // where it is prepopulating a tile with credentials that the user chose to store in the OS.
        // The second situation is in a remote logon case where the remote client may wish to
        // prepopulate a tile with a username, or in some cases, completely populate the tile and
        // use it to logon without showing any UI.
        //
        // If you wish to see an example of SetSerialization, please see either the SampleCredentialProvider
        // sample or the SampleCredUICredentialProvider sample.  [The logonUI team says, "The original sample that
        // this was built on top of didn't have SetSerialization.  And when we decided SetSerialization was
        // important enough to have in the sample, it ended up being a non-trivial amount of work to integrate
        // it into the main sample.  We felt it was more important to get these samples out to you quickly than to
        // hold them in order to do the work to integrate the SetSerialization changes from SampleCredentialProvider
        // into this sample.]
        public int SetSerialization(ref _CREDENTIAL_PROVIDER_CREDENTIAL_SERIALIZATION pcpcs)
        {
            Log.LogMethodCall();
            return HResultValues.E_NOTIMPL;
        }

        // Called by LogonUI to give you a callback.  Providers often use the callback if they
        // some event would cause them to need to change the set of tiles that they enumerated.
        public int Advise(ICredentialProviderEvents pcpe, ulong upAdviseContext)
        {
            Log.LogMethodCall();

            if(this.credentialProviderEvents != null)
            {
                var intPtr = Marshal.GetIUnknownForObject(this.credentialProviderEvents);
                Marshal.Release(intPtr);
            }

            this.credentialProviderEvents = pcpe;
            this.upAdviseContext = upAdviseContext;
            {
                var intPtr = Marshal.GetIUnknownForObject(pcpe);
                Marshal.AddRef(intPtr);
            }

            return HResultValues.S_OK;
        }

        // Called by LogonUI when the ICredentialProviderEvents callback is no longer valid.
        public int UnAdvise()
        {
            Log.LogMethodCall();

            if (this.credentialProviderEvents != null)
            {
                var intPtr = Marshal.GetIUnknownForObject(this.credentialProviderEvents);
                Marshal.Release(intPtr);
            }

            return HResultValues.S_OK;
        }

        // Called by LogonUI to determine the number of fields in your tiles.  This
        // does mean that all your tiles must have the same number of fields.
        // This number must include both visible and invisible fields. If you want a tile
        // to have different fields from the other tiles you enumerate for a given usage
        // scenario you must include them all in this count and then hide/show them as desired
        // using the field descriptors.
        public int GetFieldDescriptorCount(out uint pdwCount)
        {
            Log.LogMethodCall();

            // FieldList에 속한 Field의 개수를 전달하는 것이 목적이므로, credentialPairList의 index를 0으로 고정해도 무방하다.
            List<Field.Field> fieldList = this.credentialPairList[0].Item2;
            pdwCount = (uint)fieldList.Count;
            return HResultValues.S_OK;
        }

        // Gets the field descriptor for a particular field.
        public int GetFieldDescriptorAt(uint dwIndex, [Out] IntPtr ppcpfd) /* _CREDENTIAL_PROVIDER_FIELD_DESCRIPTOR** */
        {
            Log.LogMethodCall();

            int hr = HResultValues.E_INVALIDARG;

            // FieldList에 속한 Field들의 guidFieldType를 전달하는 것이 목적이므로, credentialPairList의 index를 0으로 고정해도 무방하다.
            List<Field.Field> fields = this.credentialPairList[0].Item2;

            if ((dwIndex < fields.Count) && (ppcpfd != IntPtr.Zero))
            {
                _CREDENTIAL_PROVIDER_FIELD_DESCRIPTOR cpfd = new _CREDENTIAL_PROVIDER_FIELD_DESCRIPTOR();
                Field.Field field = fields.ElementAt((int)dwIndex);

                cpfd.dwFieldID = field.FieldId;
                cpfd.cpft = field.FieldType;
                cpfd.pszLabel = field.Label;
                cpfd.guidFieldType = field.GuidFieldType;
             
                IntPtr structBuffer = Marshal.AllocHGlobal(Marshal.SizeOf(cpfd));
                Marshal.StructureToPtr(cpfd, structBuffer, false);
                Marshal.StructureToPtr(structBuffer, ppcpfd, false);
                return HResultValues.S_OK;
            }

            return hr;
        }

        // Sets pdwCount to the number of tiles that we wish to show at this time.
        // Sets pdwDefault to the index of the tile which should be used as the default.
        // The default tile is the tile which will be shown in the zoomed view by default. If
        // more than one provider specifies a default the last used cred prov gets to pick
        // the default. If *pbAutoLogonWithDefault is TRUE, LogonUI will immediately call
        // GetSerialization on the credential you've specified as the default and will submit
        // that credential for authentication without showing any further UI.
        public int GetCredentialCount(out uint pdwCount, out uint pdwDefault, out int pbAutoLogonWithDefault)
        {
            Log.LogMethodCall();

            if (flagRecreateCredentials)
            {
                flagRecreateCredentials = false;
                ReleaseEnumeratedCredentialPairs();
                CreateEnumeratedCredentialPairs();
            }

            pdwCount = (uint)this.credentialPairList.Count;
            pdwDefault = (uint)Constants.CredentialProviderNoDefault;
            pbAutoLogonWithDefault = Convert.ToInt32(false);

            return HResultValues.S_OK;
        }

        // Returns the credential at the index specified by dwIndex. This function is called by logonUI to enumerate
        // the tiles.
        public int GetCredentialAt(uint dwIndex, out ICredentialProviderCredential ppcpc)
        {
            Log.LogMethodCall();

            ppcpc = null;
            if (dwIndex < this.credentialPairList.Count)
                ppcpc = this.credentialPairList.ElementAt((int)dwIndex).Item1;

            return HResultValues.S_OK;
        }

        // This function will be called by LogonUI after SetUsageScenario succeeds.
        // Sets the User Array with the list of users to be enumerated on the logon screen.
        public int SetUserArray(ICredentialProviderUserArray users)
        {
            Log.LogMethodCall();

            if (credProviderUserArray != null)
            {
                var intPtr = Marshal.GetIUnknownForObject(this.credProviderUserArray);
                Marshal.Release(intPtr);
            }
            credProviderUserArray = users;
            {
                var intPtr = Marshal.GetIUnknownForObject(this.credProviderUserArray);
                Marshal.AddRef(intPtr);
            }

            return HResultValues.S_OK;
        }
        void CreateEnumeratedCredentialPairs()
        {
            Log.LogMethodCall();
            switch (cpus)
            {
                case _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_LOGON:
                case _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_UNLOCK_WORKSTATION:
                case _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_CHANGE_PASSWORD:
                    {
                        EnumerateCredentialPairs();
                        break;
                    }
                default:
                    break;
            }
        }

        void ReleaseEnumeratedCredentialPairs()
        {
            Log.LogMethodCall();
            if (this.credentialPairList == null || 
                this.credentialPairList.Count == 0)
                return;

            this.credentialPairList.Clear();
        }

        void EnumerateCredentialPairs()
        {
            Log.LogMethodCall();

            try
            {
                int hr = HResultValues.E_UNEXPECTED;
                if (credProviderUserArray == null)
                    return;

                uint dwUserCount;
                credProviderUserArray.GetCount(out dwUserCount);

                if (dwUserCount > 0) // 일반 사용자의 수에 맞춰 Credential 생성
                {
                    for (uint i = 0; i < dwUserCount; i++)
                    {
                        ICredentialProviderUser pCredUser;
                        hr = this.credProviderUserArray.GetAt(i, out pCredUser);
                        if (hr < 0)
                            continue;

                        EnumerateOnePair(pCredUser);
                    }
                }

                if ((dwUserCount == 0) || SysInfo.SysInfo.IsInDomain() == true) // 기타 사용자를 위한 여분의 Credential 생성
                    EnumerateOnePair(null);
            }
            catch(System.Exception ex)
            {
                Log.LogText(ex.Message);
            }
        }

        void EnumerateOnePair(ICredentialProviderUser pCredUser)
        {
            try
            {
                // 1. typedef std::pair<CCredential *, FieldList *> CredentialPair 생성 후 밀어넣기
                CredentialPair credentialPair = CreateCredentialPair();
                if (credentialPair == null)
                    throw new ProviderException(ErrorCode.EnumerateCredentialFailed);

                // 2. initialize
                credentialPair.Item1.Initialize(this.cpus, credentialPair.Item2, pCredUser);
                this.credentialPairList.Add(credentialPair);
            }
            catch(System.Exception ex)
            {
                Log.LogText(ex.Message);
                throw new ProviderException(ErrorCode.InitializeCredentialFailed, ex);
            }
        }
    }
}
