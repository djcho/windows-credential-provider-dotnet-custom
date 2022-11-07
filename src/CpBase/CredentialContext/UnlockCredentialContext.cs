using CredentialProvider.Interop;
using Penta.EeWin.Cp.Base.Field;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static Penta.EeWin.Cp.Base.Constants;

namespace Penta.EeWin.Cp.Base.CredentialContext
{
    public class UnlockCredentialContext : CredentialContext
    {
        public UnlockCredentialContext(in Guid cpClsid, in _CREDENTIAL_PROVIDER_USAGE_SCENARIO cpus)
            : base(cpClsid, cpus, CREDENTIAL_CONTEXT_SCENARIO.CCS_UNLOCK_WORKSTATION)
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
            base.GetSerialization(ref pcpgsr, ref pcpcs, ref optionalStatusText, 
                ref optionalStatusIcon, ref cpConnectResult, fieldController, ref nextCcs);
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
            return CredentialProviderConnectResult.ConnectResultSuccess;
        }
    }
}
