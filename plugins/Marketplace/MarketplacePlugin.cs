using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Text;
using System.Xml;
using System.Web;
using System.Web.Routing;
using Graffiti.Core;

namespace Graffiti.Marketplace
{
	public class MarketplacePlugin : GraffitiEvent
	{
        internal const string MarketplaceCreatorsRoleName = "Creators";
        internal const string MessagesCategoryName = "news";


		public override void Init(GraffitiApplication ga)
		{
			ga.BeginRequest += new EventHandler(ga_BeginRequest);
			ga.UrlRoutingAdd += new UrlRoutingEventHandler(ga_UrlRoutingAdd);
            ga.AfterNewUser += new UserEventHandler(ga_AfterNewUser);
            ga.AfterUserUpdate += new UserEventHandler(ga_AfterNewUser);
        }

        public override void EventEnabled()
        {
            UrlRouting.Initialize();
            SetupCustomFields();
        }

        #region GraffitiEvent Properties

        public override bool IsEditable
        {
            get { return false; }
        }

        public override string Name
        {
            get { return "Marketplace Plugin"; }
        }

        public override string Description
        {
            get
            {
                return "Adds a Graffiti CMS compatible marketplace to the current Graffiti site. Requires a SQL setup script to be run first.";
            }
        }

        #endregion

        #region Events

        void ga_AfterNewUser(IGraffitiUser user, EventArgs e)
        {
            // If users are added or updated, refresh the list of available creators in the custom dropdown field

            CustomFormSettings cfs = CustomFormSettings.Get();
            if (cfs.Fields == null || cfs.Fields.Count == 0)
            {
                SetupCustomFields();
            }
            else
            {
                CustomField creatorField = cfs.Fields.Find(field => Util.AreEqualIgnoreCase(field.Name, "Creator"));
                if (creatorField != null)
                {
                    UpdateCreatorsFieldOptions(creatorField);
                    cfs.Name = "-1";
                    cfs.Save();
                }
            }


        }

		private void ga_UrlRoutingAdd(System.Web.Routing.RouteCollection routes, EventArgs e)
		{
			routes.Add(new Route("data/{FeedType}", new FeedRouteHandler()));
            routes.Add(new Route("data/{FeedType}/{CategoryName}/{Feed}", new FeedRouteHandler()));
            routes.Add(new Route("data/{FeedType}/{CategoryName}/{Feed}/default.aspx", new FeedRouteHandler()));
            routes.Add(new Route("download/{PostID}", new DownloadRouteHandler()));
        }

		private void ga_BeginRequest(object sender, EventArgs e)
		{

        }

        #endregion

        #region Custom Fields

        private void SetupCustomFields()
        {
            bool saveNeeded = false;
            CustomFormSettings cfs = CustomFormSettings.Get();

            if (!DoesFieldExist(cfs.Fields, "Creator"))
            {
                cfs.Add(CreateCreatorsField("Creator", "Extension Creator"));
                saveNeeded = true;
            }
            if (!DoesFieldExist(cfs.Fields, "Version"))
            {
                cfs.Add(CreateCustomField("Version", "Version", FieldType.TextBox));
                saveNeeded = true;
            }
            if (!DoesFieldExist(cfs.Fields, "FileName"))
            {
                cfs.Add(CreateCustomField("FileName", "Download Url", FieldType.File));
                saveNeeded = true;
            }
            if (!DoesFieldExist(cfs.Fields, "ImageLarge"))
            {
                cfs.Add(CreateCustomField("ImageLarge", "Large Image Url (max 602x200)", FieldType.File));
                saveNeeded = true;
            }
            if (!DoesFieldExist(cfs.Fields, "RequiredMajorVersion"))
            {
                cfs.Add(CreateCustomField("RequiredMajorVersion", "Minimum Required Major Version #", FieldType.TextBox));
                saveNeeded = true;
            }
            if (!DoesFieldExist(cfs.Fields, "RequiredMinorVersion"))
            {
                cfs.Add(CreateCustomField("RequiredMinorVersion", "Minimum Required Minor Version #", FieldType.TextBox));
                saveNeeded = true;
            }
            if (!DoesFieldExist(cfs.Fields, "RequiresManualIntervention"))
            {
                cfs.Add(CreateCustomField("RequiresManualIntervention", "Requires Manual Intervention", FieldType.CheckBox));
                saveNeeded = true;
            }
            if (!DoesFieldExist(cfs.Fields, "Price"))
            {
                cfs.Add(CreateCustomField("Price", "Price", FieldType.TextBox));
                saveNeeded = true;
            }
            if (!DoesFieldExist(cfs.Fields, "BuyUrl"))
            {
                cfs.Add(CreateCustomField("BuyUrl", "BuyUrl", FieldType.TextBox));
                saveNeeded = true;
            }

            if (saveNeeded)
            {
                cfs.Name = "-1";
                cfs.Save();
            }
        }

        private bool DoesFieldExist(List<CustomField> fields, string name)
        {
            if (fields != null)
            {
                foreach (CustomField cf in fields)
                {
                    if (Util.AreEqualIgnoreCase(name, cf.Name))
                        return true;
                }
            }

            return false;
        }

        private CustomField CreateCustomField(string name, string desc, FieldType fieldType)
        {
            CustomField nfield = new CustomField();
            nfield.Name = name;
            nfield.Description = desc;
            nfield.Enabled = true;
            nfield.Id = Guid.NewGuid();
            nfield.FieldType = fieldType;
            return nfield;
        }

        private CustomField CreateCreatorsField(string name, string desc)
        {
            CustomField nfield = new CustomField();
            nfield.Name = name;
            nfield.Description = desc;
            nfield.Enabled = true;
            nfield.Id = Guid.NewGuid();
            nfield.FieldType = FieldType.List;
            UpdateCreatorsFieldOptions(nfield);
            return nfield;
        }

        private void UpdateCreatorsFieldOptions(CustomField field)
        {
            List<ListItemFormElement> listItems = new List<ListItemFormElement>();
            foreach (IGraffitiUser u in GraffitiUsers.GetUsers(MarketplaceCreatorsRoleName))
            {
                listItems.Add(new ListItemFormElement(u.ProperName, u.Name));
            }
            field.ListOptions = listItems;
        }

        #endregion

    }
}
