using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telligent.DynamicConfiguration.Components;
using Telligent.DynamicConfiguration.Controls;

namespace Graffiti.Core
{
    public class FilePropertyControl : Control, IPropertyControl
    {
        TextBox _textBox = null;
        Button _browseButton = null;
        CustomValidator _validator = null;

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            _textBox = new TextBox();
            _textBox.ID = "Text";
            _textBox.Columns = 50;
            this.Controls.Add(_textBox);

            _browseButton = new Button();
            _browseButton.Text = "Browse";
            this.Controls.Add(_browseButton);

            ConfigurationForm form = ConfigurationControlUtility.Instance().GetCurrentConfigurationForm(this);
            if (form != null)
            {
                _validator = new CustomValidator();
                _validator.ID = "Validator";
                _validator.ControlToValidate = _textBox.ID;
                _validator.ErrorMessage = form.InvalidUrlErrorMessage;
                this.Controls.Add(_validator);

                _validator.ServerValidate += new ServerValidateEventHandler(DefaultUrlControl_ServerValidate);
            }

            _textBox.TextChanged += new EventHandler(DefaultUrlControl_TextChanged);
        }

        void DefaultUrlControl_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (string.IsNullOrEmpty(args.Value))
                args.IsValid = true;
            else
            {
                string value = args.Value;
                if (value.StartsWith("~|"))
                    value = value.Substring(2);

                args.IsValid = Uri.IsWellFormedUriString(value, UriKind.RelativeOrAbsolute);
            }
        }

        void DefaultUrlControl_TextChanged(object sender, EventArgs e)
        {
            OnConfigurationValueChanged(GetConfigurationPropertyValue());
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            EnsureChildControls();
        }

        protected override void OnPreRender(EventArgs e)
        {
            FileBrowser.RegisterJavaScript(this.Page);

            _browseButton.OnClientClick = "OpenFileBrowser(new Function('url', 'document.getElementById(\\'" + _textBox.ClientID + "\\').value = url;')); return false;";

            base.OnPreRender(e);
        }

        #region IPropertyControl Members

        private Property _configurationProperty = null;
        public Property ConfigurationProperty
        {
            get { return _configurationProperty; }
            set { _configurationProperty = value; }
        }

        private ConfigurationDataBase _configurationData = null;
        public ConfigurationDataBase ConfigurationData
        {
            get { return _configurationData; }
            set { _configurationData = value; }
        }

        public void SetConfigurationPropertyValue(object value)
        {
            EnsureChildControls();
            _textBox.Text = value == null ? string.Empty : value.ToString();
        }

        public object GetConfigurationPropertyValue()
        {
            EnsureChildControls();
            return string.IsNullOrEmpty(_textBox.Text) ? null : new Uri(_textBox.Text, UriKind.RelativeOrAbsolute);
        }

        public event ConfigurationPropertyChanged ConfigurationValueChanged
        {
            add
            {
                EnsureChildControls();
                base.Events.AddHandler(EventConfigurationValueChanged, value);
                _textBox.AutoPostBack = true;
            }
            remove { base.Events.RemoveHandler(EventConfigurationValueChanged, value); }
        }
        private static readonly object EventConfigurationValueChanged = new object();

        protected virtual void OnConfigurationValueChanged(object value)
        {
            ConfigurationPropertyChanged handler = (ConfigurationPropertyChanged)base.Events[EventConfigurationValueChanged];
            if (handler != null)
                handler(this, value);
        }

        public Control Control
        {
            get { return this; }
        }

        #endregion
    }
}
