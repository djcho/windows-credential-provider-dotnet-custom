using CredentialProvider.Interop;
using Penta.EeWin.Cp.Base;
using Penta.EeWin.Cp.Base.CredentialContext;
using Penta.EeWin.Cp.Base.Field;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static Penta.EeWin.Cp.IdPwCp.Constants;

namespace Penta.EeWin.Cp.IdPwCp
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class IdPwCredential : Credential
    {
        private Guid classId = Guid.Parse(Constants.IdPwCpGuid);
        public override int Initialize(_CREDENTIAL_PROVIDER_USAGE_SCENARIO cpus, List<Field> fields, ICredentialProviderUser pcpUser)
        {
            Log.LogMethodCall();
#if DEBUG
            MessageBox.Show("IdPwCredential Initialize()");
#endif
            int hr = base.Initialize(cpus, fields, pcpUser);
            if (hr < 0)
                return hr;
            
            // Initialize fieldcontroller
            this.fieldController = new FieldController(this, fields, this.credProvCredentialEvents);
            this.credentialContext.Initialize(this.fieldController, pcpUser);

            return HResultValues.S_OK;
        }
        public override int Connect(IQueryContinueWithStatus pqcws)
        {
            CREDENTIAL_CONTEXT_SCENARIO nextCcs = CREDENTIAL_CONTEXT_SCENARIO.CCS_NONE;

            this.cpConnectResult = this.credentialContext.Connect(ref pqcws, this.fieldController, ref nextCcs);
            base.UpdateCredentialContext(nextCcs);

            return HResultValues.E_FAIL;
        }

        public override Base.CredentialContext.CredentialContext CreateCredentialContext(
            in _CREDENTIAL_PROVIDER_USAGE_SCENARIO cpus,
            in CREDENTIAL_CONTEXT_SCENARIO ccs)
        {
            Log.LogMethodCall();
            switch (ccs)
            {
                default:
                case CREDENTIAL_CONTEXT_SCENARIO.CCS_LOGON:
                    return new CredentialContext.IdPwLogonCredentialContext(this.classId, this.cpus);
                case CREDENTIAL_CONTEXT_SCENARIO.CCS_UNLOCK_WORKSTATION:
                    return new CredentialContext.IdPwUnlockCredentialContext(this.classId, this.cpus);
                case CREDENTIAL_CONTEXT_SCENARIO.CCS_CHANGE_PASSWORD:
                    return new CredentialContext.IdPwPasswordChangeCredentialContext(this.classId, this.cpus);
            }
        }

        public override int GetSerialization(
            out _CREDENTIAL_PROVIDER_GET_SERIALIZATION_RESPONSE pcpgsr, 
            out _CREDENTIAL_PROVIDER_CREDENTIAL_SERIALIZATION pcpcs,
            out string ppszOptionalStatusText, 
            out _CREDENTIAL_PROVIDER_STATUS_ICON pcpsiOptionalStatusIcon)
        {
            pcpgsr = _CREDENTIAL_PROVIDER_GET_SERIALIZATION_RESPONSE.CPGSR_NO_CREDENTIAL_NOT_FINISHED;
            pcpcs = new _CREDENTIAL_PROVIDER_CREDENTIAL_SERIALIZATION();
            ppszOptionalStatusText = null;
            pcpsiOptionalStatusIcon = _CREDENTIAL_PROVIDER_STATUS_ICON.CPSI_NONE;

            try
            {
                CREDENTIAL_CONTEXT_SCENARIO nextCcs = CREDENTIAL_CONTEXT_SCENARIO.CCS_NONE;
                this.credentialContext.GetSerialization(ref pcpgsr, ref pcpcs, ref ppszOptionalStatusText,
                    ref pcpsiOptionalStatusIcon, ref this.cpConnectResult, this.fieldController, ref nextCcs);

                base.UpdateCredentialContext(nextCcs);
            }
            catch(Exception ex)
            {
                Log.LogText(ex.Message);
                return HResultValues.E_FAIL;
            }
            return HResultValues.S_OK;
        }

        public override int ReportResult(
            int ntsStatus,
            int ntsSubstatus,
            out string ppszOptionalStatusText,
            out _CREDENTIAL_PROVIDER_STATUS_ICON pcpsiOptionalStatusIcon)
        {
            ppszOptionalStatusText = default;
            pcpsiOptionalStatusIcon = _CREDENTIAL_PROVIDER_STATUS_ICON.CPSI_NONE;
            try
            {
                CREDENTIAL_CONTEXT_SCENARIO nextCcs = CREDENTIAL_CONTEXT_SCENARIO.CCS_NONE;
                this.credentialContext.ReportResult(ntsStatus, ntsSubstatus, ref ppszOptionalStatusText,
                    ref pcpsiOptionalStatusIcon, fieldController, ref nextCcs);

                base.UpdateCredentialContext(nextCcs);
            }
            catch (Exception ex)
            {
                Log.LogText(ex.Message);
                return HResultValues.E_FAIL;
            }
            return HResultValues.S_OK;
        }

        public override int CommandLinkClicked(uint dwFieldID)
        {
            if (dwFieldID >= this.fieldController.Fields.Count)
                return HResultValues.E_INVALIDARG;

            return HResultValues.S_OK;
        }

        public override int GetComboBoxValueCount(uint dwFieldID, out uint pcItems, out uint pdwSelectedItem)
        {
            if (dwFieldID >= this.fieldController.Fields.Count)
            {
                pcItems = 0;
                pdwSelectedItem = 0;
                return HResultValues.E_INVALIDARG;
            }

            return base.GetComboBoxValueCount(dwFieldID, out pcItems, out pdwSelectedItem);
        }

        public override int GetComboBoxValueAt(uint dwFieldID, uint dwItem, out string ppszItem)
        {
            if (dwFieldID >= this.fieldController.Fields.Count)
            {
                ppszItem = null;
                return HResultValues.E_INVALIDARG;
            }

            return base.GetComboBoxValueAt(dwFieldID, dwItem, out ppszItem);
        }

        public override int SetComboBoxSelectedValue(uint dwFieldID, uint dwSelectedItem)
        {
            if (dwFieldID >= this.fieldController.Fields.Count)
                return HResultValues.E_INVALIDARG;

            IntPtr hwnd = IntPtr.Zero;

            if (this.credProvCredentialEvents != null)
                this.credProvCredentialEvents.OnCreatingWindow(out hwnd);

            if(dwFieldID == (uint)CpFiledId.EtcComboBox)
            {
                string menuTitle = this.fieldController.GetComboBoxValueAt((uint)CpFiledId.EtcComboBox, dwSelectedItem);
                if(menuTitle == "")
                {

                }
            }

            return base.SetComboBoxSelectedValue(dwFieldID, dwSelectedItem);
        }
    }
}
