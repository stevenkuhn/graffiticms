using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telligent.DynamicConfiguration.Components;
using Telligent.Glow.Editor;

namespace Graffiti.Core
{
    public class HtmlPropertyControl : Editor, IPropertyControl
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.ToolbarSet = "Simple";
            this.Width = new Unit(600);

            if (!string.IsNullOrEmpty(this.ConfigurationProperty.Attributes["width"]))
                this.Width = Unit.Parse(this.ConfigurationProperty.Attributes["width"]);

            if (!string.IsNullOrEmpty(this.ConfigurationProperty.Attributes["height"]))
                this.Height = Unit.Parse(this.ConfigurationProperty.Attributes["height"]);
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
            this.Value = value == null ? string.Empty : value.ToString();
        }

        public object GetConfigurationPropertyValue()
        {
            if (string.IsNullOrEmpty(this.Value))
                return string.Empty;
            else
                return this.Value;
        }

        public event ConfigurationPropertyChanged ConfigurationValueChanged
        {
            add { throw new InvalidOperationException("The HtmlPropertyControl does not support the ConfigurationValueChanged event"); }
            remove { throw new InvalidOperationException("The HtmlPropertyControl does not support the ConfigurationValueChanged event"); }
        }

        public Control Control
        {
            get { return this; }
        }

        #endregion
    }
}
