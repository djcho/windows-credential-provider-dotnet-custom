using CredentialProvider.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penta.EeWin.Cp.Base.Field
{
    public class Field
    {
        private uint fieldId;

        private string label;
        private string text;

        private Guid guidFieldType;
        private _CREDENTIAL_PROVIDER_FIELD_TYPE fieldType;
        private _CREDENTIAL_PROVIDER_FIELD_STATE fieldState;
        private _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE fieldInteractiveState;

        public uint FieldId { get { return fieldId; } set { this.fieldId = value; } }
        public string Label { get { return label; } set { this.label = value; } }
        public string Text { get { return text; } set { this.text = value; } }
        public _CREDENTIAL_PROVIDER_FIELD_TYPE FieldType { get { return fieldType; } set { this.fieldType = value; } }
        public _CREDENTIAL_PROVIDER_FIELD_STATE FieldState { get { return fieldState; } set { this.fieldState = value; } }
        public _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE FieldInteractiveState { get { return fieldInteractiveState; } set { this.fieldInteractiveState = value; } }
        public Guid GuidFieldType { get { return guidFieldType; } set { this.guidFieldType = value; } }
        
        public Field(in uint fieldId, in _CREDENTIAL_PROVIDER_FIELD_TYPE fieldType, in string label, in _CREDENTIAL_PROVIDER_FIELD_STATE fieldState, in _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE fieldInteractiveState, in Guid guidFieldType)
        {
            this.fieldId = fieldId;
            this.fieldType = fieldType;
            this.fieldState = fieldState;
            this.label = label;
            this.fieldInteractiveState = fieldInteractiveState;
            this.guidFieldType = guidFieldType;
        }
    }
}
