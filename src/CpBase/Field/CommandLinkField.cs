using CredentialProvider.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penta.EeWin.Cp.Base.Field
{
    public class CommandLinkField : Field
    {
        public CommandLinkField(in uint fieldId, in string label, in _CREDENTIAL_PROVIDER_FIELD_STATE fieldState, in _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE fieldInteractiveState)
            : base(fieldId, _CREDENTIAL_PROVIDER_FIELD_TYPE.CPFT_COMMAND_LINK, label, fieldState, fieldInteractiveState, Guid.Empty)
        {
        }
    }
}
