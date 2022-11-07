using CredentialProvider.Interop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace Penta.EeWin.Cp.Base.Field
{
    public class FieldController
    {
        //Members
        private ICredentialProviderCredential credential;
        private ICredentialProviderCredentialEvents2 credentialEvent;
        private readonly List<Field> fields = new List<Field>();
        
        //Properties
        public List<Field> Fields { get { return fields; } }

        //Methods
        public FieldController(in ICredentialProviderCredential credential,
            in List<Field> fields, 
            in ICredentialProviderCredentialEvents2 credentialEvent)
        {
            Log.LogMethodCall();
            this.credential = credential;
            this.fields = fields;
            this.credentialEvent = credentialEvent;
        }

        public void AddField(in Field field)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            if (field.FieldType == _CREDENTIAL_PROVIDER_FIELD_TYPE.CPFT_EDIT_TEXT ||
                field.FieldType == _CREDENTIAL_PROVIDER_FIELD_TYPE.CPFT_PASSWORD_TEXT)
                field.Text = string.Empty;
            else
                field.Text = field.Label;

            fields.Add(field);
        }

        public _CREDENTIAL_PROVIDER_FIELD_STATE GetFieldState(in uint fieldId)
        {
            if (fieldId >= this.fields.Count)
                throw new ArgumentOutOfRangeException();

            return this.fields.ElementAt((int)fieldId).FieldState;
        }

        public _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE GetFieldInteractiveState(in uint fieldId)
        {
            if (fieldId >= this.fields.Count)
                throw new ArgumentOutOfRangeException();

            return this.fields.ElementAt((int)fieldId).FieldInteractiveState;
        }

        public void SetFieldInteractiveState(in uint fieldId, _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE fieldInteractiveState)
        {
            if (fieldId >= this.fields.Count)
                throw new ArgumentOutOfRangeException();

            this.fields.ElementAt((int)fieldId).FieldInteractiveState = fieldInteractiveState;
        }

        public int GetSubmitButtonValue(in uint fieldId)
        {
            if (fieldId >= this.fields.Count)
                throw new ArgumentOutOfRangeException();

            if (this.fields.ElementAt((int)fieldId).FieldType != _CREDENTIAL_PROVIDER_FIELD_TYPE.CPFT_SUBMIT_BUTTON)
                throw new InstanceNotFoundException();

            SubmitButtonField submitButtonField = this.fields.ElementAt((int)fieldId) as SubmitButtonField;
            if (submitButtonField == null)
                throw new InvalidCastException();

            return (int)submitButtonField.AttachedFieldId;
        }

        public void SetSubmitButtonValue(in uint fieldId)
        {
            if (fieldId >= this.fields.Count)
                throw new ArgumentOutOfRangeException();

            Field field = fields.Find(f => f.FieldType == _CREDENTIAL_PROVIDER_FIELD_TYPE.CPFT_SUBMIT_BUTTON);
            if (field == null)
                throw new InvalidOperationException("Field not found");

            SubmitButtonField submitButtonField = field as SubmitButtonField;
            if (submitButtonField == null)
                throw new InvalidCastException();

            submitButtonField.AttachedFieldId = fieldId;
        }

        public void BeginFieldUpdated()
        {
            if (this.credentialEvent == null)
            {
                Log.LogText("credentialEvent is null");
                return;
            }

            this.credentialEvent.BeginFieldUpdates();
        }

        public void EndFieldUpdated()
        {
            if (this.credentialEvent == null)
            {
                Log.LogText("credentialEvent is null");
                return;
            }

            this.credentialEvent.EndFieldUpdates();
        }

        public void FocusField(in uint fieldId)
        {
            if (fieldId >= this.fields.Count)
                throw new ArgumentOutOfRangeException();

            _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE fieldInteractiveState = this.fields.ElementAt((int)fieldId).FieldInteractiveState;
            if (fieldInteractiveState == _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE.CPFIS_DISABLED)
                throw new InvalidOperationException("field is disabled");

            foreach(Field field in this.fields)
            {
                if(field.FieldInteractiveState == _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE.CPFIS_FOCUSED)
                {
                    field.FieldInteractiveState = _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE.CPFIS_NONE;
                    if (this.credentialEvent != null)
                        this.credentialEvent.SetFieldInteractiveState(this.credential, field.FieldId, _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE.CPFIS_NONE);
                }
            }

            this.fields.ElementAt((int)fieldId).FieldInteractiveState = _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE.CPFIS_FOCUSED;
            if (this.credentialEvent != null)
                credentialEvent.SetFieldInteractiveState(this.credential, fieldId, _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE.CPFIS_FOCUSED);
        }

        public void EnableField(in uint fieldId, in bool enabled)
        {
            if (fieldId >= this.fields.Count)
                throw new ArgumentOutOfRangeException();

            _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE cpfis;
            if (enabled)
                cpfis = _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE.CPFIS_NONE;
            else
                cpfis = _CREDENTIAL_PROVIDER_FIELD_INTERACTIVE_STATE.CPFIS_DISABLED;

            this.fields.ElementAt((int)fieldId).FieldInteractiveState = cpfis;
            if (this.credentialEvent != null)
                this.credentialEvent.SetFieldInteractiveState(this.credential, fieldId, cpfis);
        }

        public void ReadOnlyField(in uint fieldId, in bool enabled)
        {
            // readonly 속성은 deprecated 되었다.
            this.EnableField(fieldId, enabled);
        }

        public void ShowField(in uint fieldId, in bool show)
        {
            if (fieldId >= this.fields.Count)
                throw new ArgumentOutOfRangeException();

            _CREDENTIAL_PROVIDER_FIELD_STATE cpfs;
            if (show)
                cpfs = _CREDENTIAL_PROVIDER_FIELD_STATE.CPFS_DISPLAY_IN_SELECTED_TILE;
            else
                cpfs = _CREDENTIAL_PROVIDER_FIELD_STATE.CPFS_HIDDEN;

            this.fields.ElementAt((int)fieldId).FieldState = cpfs;
            if(this.credentialEvent != null)
                this.credentialEvent.SetFieldState(this.credential, fieldId, cpfs);
        }


        public string GetfieldString(in uint fieldId)
        {
            if (fieldId > this.fields.Count)
                throw new ArgumentOutOfRangeException();

            return this.fields.ElementAt((int)fieldId).Text;

        }
        public void SetFieldString(in uint fieldId, in string fieldString)
        {
            if (fieldId >= this.fields.Count)
                throw new ArgumentOutOfRangeException();

            this.fields.ElementAt((int)fieldId).Text = fieldString;
            
            if(this.credentialEvent != null)
            {
                this.credentialEvent.SetFieldString(this.credential, fieldId, fieldString);
            }
        }

        public Bitmap GetBitmap(in uint fieldId)
        {
            if (fieldId >= this.fields.Count)
                throw new ArgumentOutOfRangeException();

            if (this.fields.ElementAt((int)fieldId).FieldType != _CREDENTIAL_PROVIDER_FIELD_TYPE.CPFT_TILE_IMAGE)
                throw new InvalidOperationException();

            TileImageField tileImageField = this.fields.ElementAt((int)fieldId) as TileImageField;
            if (tileImageField == null)
                throw new InvalidCastException();

            return tileImageField.BitmapImage;
        }

        public int GetComboBoxValueCount(in uint fieldId)
        {
            if (fieldId >= this.fields.Count)
                throw new ArgumentOutOfRangeException();

            if (this.fields.ElementAt((int)fieldId).FieldType != _CREDENTIAL_PROVIDER_FIELD_TYPE.CPFT_COMBOBOX)
                throw new InvalidOperationException();

            ComboBoxField comboBoxField = this.fields.ElementAt((int)fieldId) as ComboBoxField;
            if (comboBoxField == null)
                throw new InvalidCastException();

            return comboBoxField.Items.Count;
        }

        public string GetComboBoxValueAt(in uint fieldId, in uint itemId)
        {
            if (fieldId >= this.fields.Count)
                throw new ArgumentOutOfRangeException();

            if (this.fields.ElementAt((int)fieldId).FieldType != _CREDENTIAL_PROVIDER_FIELD_TYPE.CPFT_COMBOBOX)
                throw new InvalidOperationException();

            ComboBoxField comboBoxField = this.fields.ElementAt((int)fieldId) as ComboBoxField;
            if (comboBoxField == null)
                throw new InvalidCastException();

            if(itemId >= comboBoxField.Items.Count)
                throw new ArgumentOutOfRangeException();

            return comboBoxField.Items.ElementAt((int)itemId);
        }

        public void SetComboBoxSelectedValue(in uint fieldId, in uint itemId)
        {
            if (fieldId >= this.fields.Count)
                throw new ArgumentOutOfRangeException();

            if(this.fields.ElementAt((int)fieldId).FieldType != _CREDENTIAL_PROVIDER_FIELD_TYPE.CPFT_COMBOBOX)
                throw new InvalidOperationException();

            ComboBoxField comboBoxField = this.fields.ElementAt((int)fieldId) as ComboBoxField;
            if (comboBoxField == null)
                throw new InvalidCastException();

            if (itemId >= comboBoxField.Items.Count)
                throw new ArgumentOutOfRangeException();

            comboBoxField.SelectedIndex = itemId;
            if (this.credentialEvent != null)
                this.credentialEvent.SetFieldComboBoxSelectedItem(this.credential, fieldId, itemId);
        }

        public void DeleteComboBoxItem(in uint fieldId, in uint itemId)
        {
            if (fieldId >= this.fields.Count)
                throw new ArgumentOutOfRangeException();

            if(this.fields.ElementAt((int)fieldId).FieldType != _CREDENTIAL_PROVIDER_FIELD_TYPE.CPFT_COMBOBOX)
                throw new InvalidOperationException();

            ComboBoxField comboBoxField = this.fields.ElementAt((int)fieldId) as ComboBoxField;
            if (comboBoxField == null)
                throw new InvalidCastException();

            if (itemId >= comboBoxField.Items.Count)
                throw new ArgumentOutOfRangeException();

            comboBoxField.Items.RemoveAt((int)itemId);
            if (this.credentialEvent != null)
                this.credentialEvent.DeleteFieldComboBoxItem(this.credential, fieldId, itemId);
        }

        public void AppendComboBoxItem(in uint fieldId, in string itemText)
        {
            if (fieldId >= this.fields.Count)
                throw new ArgumentOutOfRangeException();

            if (this.fields.ElementAt((int)fieldId).FieldType != _CREDENTIAL_PROVIDER_FIELD_TYPE.CPFT_COMBOBOX)
                throw new InvalidOperationException();

            ComboBoxField comboBoxField = this.fields.ElementAt((int)fieldId) as ComboBoxField;
            if (comboBoxField == null)
                throw new InvalidCastException();

            comboBoxField.Items.Append(itemText);
            if (this.credentialEvent != null)
                this.credentialEvent.AppendFieldComboBoxItem(this.credential, fieldId, itemText);
        }

        public bool GetCheckBoxValue(in uint fieldId)
        {
            if (fieldId >= this.fields.Count)
                throw new ArgumentOutOfRangeException();

            if (this.fields.ElementAt((int)fieldId).FieldType != _CREDENTIAL_PROVIDER_FIELD_TYPE.CPFT_CHECKBOX)
                throw new InvalidOperationException();

            CheckboxField checkboxField = this.fields.ElementAt((int)fieldId) as CheckboxField;
            if (checkboxField == null)
                throw new InvalidCastException();

            return checkboxField.IsChecked;
        }

        public void SetCheckBoxValue(in uint fieldId, in bool isChecked, in string text)
        {
            if (fieldId >= this.fields.Count)
                throw new ArgumentOutOfRangeException();

            if (this.fields.ElementAt((int)fieldId).FieldType != _CREDENTIAL_PROVIDER_FIELD_TYPE.CPFT_CHECKBOX)
                throw new InvalidOperationException();

            CheckboxField checkboxField = this.fields.ElementAt((int)fieldId) as CheckboxField;
            if (checkboxField == null)
                throw new InvalidCastException();

            checkboxField.IsChecked = isChecked;
            if(this.credentialEvent != null)
            {
                this.credentialEvent.SetFieldCheckbox(this.credential, fieldId, Convert.ToInt32(isChecked), text);
            }
        }

        public Window GetCredentialHwnd()
        {
            IntPtr hwnd = default;
            if (this.credentialEvent != null)
            {
                this.credentialEvent.OnCreatingWindow(out hwnd);
            }

            HwndSource hwndSource = HwndSource.FromHwnd(hwnd);

            return hwndSource.RootVisual as Window;
        }
    }
}
