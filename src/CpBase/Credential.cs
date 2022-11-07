using CredentialProvider.Interop;
using Penta.EeWin.Cp.Base.CredentialContext;
using Penta.EeWin.Cp.Base.Field;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace Penta.EeWin.Cp.Base
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class Credential : ICredentialProviderCredential2, ICredentialProviderCredentialWithFieldOptions, IConnectableCredentialProviderCredential
    {
        protected _CREDENTIAL_PROVIDER_USAGE_SCENARIO cpus;
        protected ICredentialProviderUser cpUser;
        protected ICredentialProviderCredentialEvents2 credProvCredentialEvents;
        protected CredentialContext.CredentialContext credentialContext;
        protected FieldController fieldController = default;
        protected Constants.CredentialProviderConnectResult cpConnectResult = Constants.CredentialProviderConnectResult.ConnectResultSuccess;

        public abstract CredentialContext.CredentialContext CreateCredentialContext(
            in _CREDENTIAL_PROVIDER_USAGE_SCENARIO cpus, 
            in CREDENTIAL_CONTEXT_SCENARIO ccs);

        // Initializes one credential with the field information passed in.
        // Set the value of the SFI_LARGE_TEXT field to pwzUsername.
        public virtual int Initialize(_CREDENTIAL_PROVIDER_USAGE_SCENARIO cpus,
            List<Field.Field> fields,
            ICredentialProviderUser pcpUser)
        {
            Log.LogMethodCall();
#if DEBUG
            MessageBox.Show("initialize braek point");
#endif

            if (pcpUser != null)
            {
                this.cpUser = pcpUser;
                IntPtr inPtr = Marshal.GetIUnknownForObject(this.cpUser);
                Marshal.AddRef(inPtr);
            }

            CREDENTIAL_CONTEXT_SCENARIO ccs = CREDENTIAL_CONTEXT_SCENARIO.CCS_NONE;
            if (cpus == _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_CHANGE_PASSWORD)
                ccs = CREDENTIAL_CONTEXT_SCENARIO.CCS_CHANGE_PASSWORD;
            else if (cpus == _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_LOGON)
            {
                if (isLockedUser(pcpUser))
                    ccs = CREDENTIAL_CONTEXT_SCENARIO.CCS_UNLOCK_WORKSTATION;
                else
                    ccs = CREDENTIAL_CONTEXT_SCENARIO.CCS_LOGON;
            }

            this.cpus= cpus;
            this.credentialContext = CreateCredentialContext(cpus, ccs);
            
            //SysInfo.Keyboard.SetNumlockOn(); <<-- 터지는 버그 있음

            return HResultValues.S_OK;
        }

        bool isLockedUser(in ICredentialProviderUser cpUser)
        {
            if (cpUser == null)
                return false;

            string logonStatus;
            int result = cpUser.GetStringValue(PInvoke.PKEY_Identity_LogonStatusString, out logonStatus);
            if (result != HResultValues.S_OK || logonStatus == string.Empty)
                return false;

            return Helpers.ParseLogonStatus(logonStatus);
        }

        // LogonUI calls this in order to give us a callback in case we need to notify it of anything.
        public int Advise(ICredentialProviderCredentialEvents pcpce)
        {
            Log.LogMethodCall();
            if (this.credProvCredentialEvents != null)
            {
                var intPtr = Marshal.GetIUnknownForObject(this.credProvCredentialEvents);
                Marshal.Release(intPtr);
            }

            if (pcpce != null)
            {
                credProvCredentialEvents = (ICredentialProviderCredentialEvents2)pcpce;
                var intPtr = Marshal.GetIUnknownForObject(pcpce);
                Marshal.AddRef(intPtr);
            }
            return HResultValues.S_OK;
        }

        // LogonUI calls this to tell us to release the callback.
        public int UnAdvise()
        {
            Log.LogMethodCall();
            if (credProvCredentialEvents != null)
            {
                var intPtr = Marshal.GetIUnknownForObject(credProvCredentialEvents);
                Marshal.Release(intPtr);
            }
            credProvCredentialEvents = null;
            return HResultValues.S_OK;
        }

        // LogonUI calls this function when our tile is selected (zoomed)
        // If you simply want fields to show/hide based on the selected state,
        // there's no need to do anything here - you can set that up in the
        // field definitions. But if you want to do something
        // more complicated, like change the contents of a field when the tile is
        // selected, you would do it here.
        public int SetSelected(out int pbAutoLogon)
        {
            Log.LogMethodCall();
            pbAutoLogon = 0;
            return HResultValues.S_OK;
        }

        // Similarly to SetSelected, LogonUI calls this when your tile was selected
        // and now no longer is. The most common thing to do here (which we do below)
        // is to clear out the password field.
        public int SetDeselected()
        {
            Log.LogMethodCall();            
            return HResultValues.S_OK;
        }

        // Get info for a particular field of a tile. Called by logonUI to get information
        // to display the tile.
        public int GetFieldState(uint dwFieldID, out _CREDENTIAL_PROVIDER_FIELD_STATE pcpfs, out _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE pcpfis)
        {
            Log.LogMethodCall();

            pcpfs = default;
            pcpfis = default;

            // Validate our parameters.
            if (dwFieldID >= this.fieldController.Fields.Count)
                return HResultValues.E_INVALIDARG;

            pcpfs = this.fieldController.GetFieldState(dwFieldID);
            pcpfis = this.fieldController.GetFieldInteractiveState(dwFieldID);

            return HResultValues.S_OK;
        }

        // Sets ppwsz to the string value of the field at the index dwFieldID
        public int GetStringValue(uint dwFieldID, out string ppsz)
        {
            Log.LogMethodCall();
            ppsz = null;

            if (dwFieldID >= this.fieldController.Fields.Count)
                return HResultValues.E_INVALIDARG;

            // Make a copy of the string and return that. The caller
            // is responsible for freeing it.
            ppsz = this.fieldController.Fields.ElementAt((int)dwFieldID).Text;

            return HResultValues.S_OK;
        }

        // Get the image to show in the user tile
        public int GetBitmapValue(uint dwFieldID, out IntPtr phbmp)
        {
            Log.LogMethodCall();
            phbmp = IntPtr.Zero;

            Bitmap bitmap = this.fieldController.GetBitmap(dwFieldID);
            if (bitmap == null)
                return HResultValues.E_INVALIDARG;
         
            phbmp = bitmap.GetHbitmap();

            return HResultValues.S_OK;
        }

        // Returns whether a checkbox is checked or not as well as its label.
        public int GetCheckboxValue(uint dwFieldID, out int pbChecked, out string ppszLabel)
        {
            pbChecked = default;
            ppszLabel = default;

            return HResultValues.S_OK;
        }

        // Sets pdwAdjacentTo to the index of the field the submit button should be
        // adjacent to. We recommend that the submit button is placed next to the last
        // field which the user is required to enter information in. Optional fields
        // should be below the submit button.
        public int GetSubmitButtonValue(uint dwFieldID, out uint pdwAdjacentTo)
        {
            Log.LogMethodCall();

            pdwAdjacentTo = (uint)this.fieldController.GetSubmitButtonValue(dwFieldID);

            if (pdwAdjacentTo < 0)
                return HResultValues.E_INVALIDARG;

            return HResultValues.S_OK;
        }

        // Returns the number of items to be included in the combobox (pcItems), as well as the
        // currently selected item (pdwSelectedItem).
        public virtual int GetComboBoxValueCount(uint dwFieldID, out uint pcItems, out uint pdwSelectedItem)
        {
            pcItems = (uint)this.fieldController.GetComboBoxValueCount(dwFieldID);
            pdwSelectedItem = 0;
            return HResultValues.S_OK;
        }

        // Called iteratively to fill the combobox with the string (ppwszItem) at index dwItem.
        public virtual int GetComboBoxValueAt(uint dwFieldID, uint dwItem, out string ppszItem)
        {
            ppszItem = this.fieldController.GetComboBoxValueAt(dwFieldID, dwItem);
            return HResultValues.S_OK;
        }

        // Sets the value of a field which can accept a string as a value.
        // This is called on each keystroke when a user types into an edit field
        public int SetStringValue(uint dwFieldID, string psz)
        {
            this.fieldController.SetFieldString(dwFieldID, psz);
            return HResultValues.S_OK;
        }

        // Sets whether the specified checkbox is checked or not.
        public int SetCheckboxValue(uint dwFieldID, int bChecked)
        {
            return HResultValues.S_OK;
        }

        // Called when the user changes the selected item in the combobox.
        public virtual int SetComboBoxSelectedValue(uint dwFieldID, uint dwSelectedItem)
        {
            this.fieldController.SetComboBoxSelectedValue(dwFieldID, dwSelectedItem);
            return HResultValues.S_OK;
        }

        public virtual int CommandLinkClicked(uint dwFieldID)
        {
            return HResultValues.S_OK;
        }

        // Collect the username and password into a serialized credential for the correct usage scenario
        // (logon/unlock is what's demonstrated in this sample).  LogonUI then passes these credentials
        // back to the system to log on.
        public abstract int GetSerialization(
            out _CREDENTIAL_PROVIDER_GET_SERIALIZATION_RESPONSE pcpgsr,
            out _CREDENTIAL_PROVIDER_CREDENTIAL_SERIALIZATION pcpcs,
            out string ppszOptionalStatusText,
            out _CREDENTIAL_PROVIDER_STATUS_ICON pcpsiOptionalStatusIcon);

        // ReportResult is completely optional.  Its purpose is to allow a credential to customize the string
        // and the icon displayed in the case of a logon failure.  For example, we have chosen to
        // customize the error shown in the case of bad username/password and in the case of the account
        // being disabled.
        public abstract int ReportResult(
            int ntsStatus, 
            int ntsSubstatus, 
            out string ppszOptionalStatusText,
            out _CREDENTIAL_PROVIDER_STATUS_ICON pcpsiOptionalStatusIcon);

        // Gets the SID of the user corresponding to the credential.
        public int GetUserSid(out string sid)
        {
            Log.LogMethodCall();

            sid = this.credentialContext.UserSid;
            // Return S_FALSE with a null SID in ppszSid for the
            // credential to be associated with an empty user tile.
            return HResultValues.S_OK; ;
        }

        // GetFieldOptions to enable the password reveal button and touch keyboard auto-invoke in the password field.
        public int GetFieldOptions(uint fieldID, out CREDENTIAL_PROVIDER_CREDENTIAL_FIELD_OPTIONS options)
        {
            Log.LogMethodCall();
            options = CREDENTIAL_PROVIDER_CREDENTIAL_FIELD_OPTIONS.CPCFO_NONE;
            return HResultValues.S_OK;
        }

        //Implimente IConnectableCredentialProviderCredential Interface 
        public abstract int Connect(IQueryContinueWithStatus pqcws);
        public int Disconnect()
        {
            return HResultValues.S_OK;
        }

        public void SetCredentialContext(CredentialContext.CredentialContext credentialContext)
        {
            this.credentialContext = credentialContext;
        }
        public void UpdateCredentialContext(in CREDENTIAL_CONTEXT_SCENARIO ccs)
        {
            if (this.credentialContext == null || ccs == CREDENTIAL_CONTEXT_SCENARIO.CCS_NONE)
                return;

            CREDENTIAL_CONTEXT_SCENARIO nextCcs = ccs;
            if (nextCcs != this.credentialContext.Ccs)
            {                
                if (this.cpus == _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_CHANGE_PASSWORD) // CPUS_CHANGE_PASSWORD 시나리오는 CCS_CHANGE_PASSWORD 만 유효하도록 고정
                    nextCcs = CREDENTIAL_CONTEXT_SCENARIO.CCS_CHANGE_PASSWORD;
                else if (this.cpus == _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_LOGON || this.cpus == _CREDENTIAL_PROVIDER_USAGE_SCENARIO.CPUS_UNLOCK_WORKSTATION)
                    if (nextCcs == CREDENTIAL_CONTEXT_SCENARIO.CCS_LOGON && this.isLockedUser(this.cpUser)) //CCS_LOGON 이라도 화면 잠금 상태의 사용자라면 강제로 CCS_UNLOCK_WORKSTATION 으로 진행
                        nextCcs = CREDENTIAL_CONTEXT_SCENARIO.CCS_UNLOCK_WORKSTATION;

                this.credentialContext = CreateCredentialContext(this.cpus, nextCcs);
                if (this.credentialContext == null)
                    return;

                this.credentialContext.Initialize(this.fieldController, this.cpUser);
            }
        }
    }
}
