using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Graffiti.Core;

namespace Graffiti.Web.graffiti_admin.presentation.themes
{
    public partial class ConfigureTheme : AdminControlPanelPage
    {
        protected Telligent.DynamicConfiguration.Controls.ConfigurationForm ConfigurationForm;
        protected Button Save;
        protected LinkButton RestoreDefaults;
        protected PlaceHolder ConfigWrapper, NoConfigWrapper;

        protected void Page_Load(object sender, EventArgs e)
        {
            LiHyperLink.SetNameToCompare(Context, "presentation");
            this.DataBind();
        }

        public override void DataBind()
        {
            ThemeConfigurationData themeConfig = ObjectManager.Get<ThemeConfigurationData>(ThemeConfigurationData.GetKey(Request.QueryString[Graffiti.Core.API.QueryStringKey.Theme]));
            themeConfig.Theme = Request.QueryString[Graffiti.Core.API.QueryStringKey.Theme];

            ConfigurationForm.ConfigurationData = themeConfig;

            bool hasGroups = false;
            if (themeConfig.PropertyGroups != null)
            {
                foreach (Telligent.DynamicConfiguration.Components.PropertyGroup group in themeConfig.PropertyGroups)
                {
                    if (group.Visible)
                    {
                        hasGroups = true;
                        break;
                    }
                }
            }

            Save.Visible = hasGroups;
            ConfigWrapper.Visible = hasGroups;
            NoConfigWrapper.Visible = !hasGroups;

            if (!hasGroups)
                SetMessage(StatusType.Warning, "The selected theme does not require any additional configuration.");

            base.DataBind();
        }

        protected void Save_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                ConfigurationForm.Commit();

                ConfigurationForm.ReloadControlValuesFromConfigurationData();

                SetMessage(StatusType.Success, "Your changes have been successfully saved.");
            }
            else
                SetMessage(StatusType.Error, "Please check your configuration.  There was an issue while saving.");
        }

        protected void RestoreDefaults_Click(object sender, EventArgs e)
        {
            ThemeConfigurationData data = ConfigurationForm.ConfigurationData as ThemeConfigurationData;
            if (data != null)
            {
                foreach (Telligent.DynamicConfiguration.Components.PropertyGroup group in data.PropertyGroups)
                {
                    foreach (Telligent.DynamicConfiguration.Components.Property property in group.GetAllProperties())
                    {
                        data.ClearValue(property);
                    }
                }

                ConfigurationForm.ReloadControlValuesFromConfigurationData();

                ConfigurationForm.Commit();

                SetMessage(StatusType.Success, "The default values have been restored and saved.");
            }
        }

        protected void SetMessage(StatusType type, string message)
        {
            SetMessageRecursive(type, message, this);
        }

        protected void SetMessageRecursive(StatusType type, string message, Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is StatusMessage)
                {
                    StatusMessage sm = (StatusMessage)c;
                    sm.Text = message;
                    sm.Type = type;
                }
                else if (c.Controls.Count > 0)
                    SetMessageRecursive(type, message, c);
            }
        }
    }
}
