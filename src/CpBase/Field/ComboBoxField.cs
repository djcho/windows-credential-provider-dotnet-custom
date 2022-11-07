using CredentialProvider.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penta.EeWin.Cp.Base.Field
{
    public class ComboBoxField : Field
    {
        private uint selectedIndex;
        private List<string> items = new List<string>();

        public List<string> Items { get { return this.items; } }
        public uint SelectedIndex { get { return this.selectedIndex; } set { this.selectedIndex = value;  } }

        public ComboBoxField(
            in uint fieldId,
            in string label, 
            in _CREDENTIAL_PROVIDER_FIELD_STATE fieldState, 
            in _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE fieldInteractiveState)
            : base(fieldId, _CREDENTIAL_PROVIDER_FIELD_TYPE.CPFT_COMBOBOX, label, fieldState, fieldInteractiveState, Guid.Empty)
        {
        }

        public ComboBoxField(
            in uint fieldId,
            in string label,
            in _CREDENTIAL_PROVIDER_FIELD_STATE fieldState,            
            in _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE fieldInteractiveState,
            in List<string> items,
            in uint selectedIndex)
            : base(fieldId, _CREDENTIAL_PROVIDER_FIELD_TYPE.CPFT_COMBOBOX, label, fieldState, fieldInteractiveState, Guid.Empty)
        {
            this.items = items;
            this.selectedIndex = selectedIndex;
        }
    }
}
