using CredentialProvider.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penta.EeWin.Cp.Base.Field
{
    public class SubmitButtonField : Field
    {
        private uint attachedFieldId;
        public uint AttachedFieldId { get { return attachedFieldId; } set { attachedFieldId = value; } }

        public SubmitButtonField(
            in uint fieldId,
            in string label, 
            in _CREDENTIAL_PROVIDER_FIELD_STATE fieldState, 
            in _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE fieldInteractiveState,
            in uint attachedFieldId)
            : base(fieldId, _CREDENTIAL_PROVIDER_FIELD_TYPE.CPFT_SUBMIT_BUTTON, label, fieldState, fieldInteractiveState, Guid.Empty)
        {
            this.attachedFieldId = fieldId;
        }
    }
}
