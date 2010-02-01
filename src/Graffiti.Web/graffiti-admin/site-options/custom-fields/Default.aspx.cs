using System;
using System.Collections.Generic;
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

public partial class graffiti_admin_site_options_forms_Default : AdminControlPanelPage
{

    private int _category = -1;

    protected void Page_Load(object sender, EventArgs e)
    {
        LiHyperLink.SetNameToCompare(Context,"settings");

        _category = int.Parse(Request.QueryString["category"] ?? "-1");

        if (Request.QueryString["id"] != null)
        {
            if (Request.QueryString["new"] != null)
            {
                if (!IsPostBack)
                {
                    Message.Text = "Your field was successfully added.";
                    Message.Type = StatusType.Success;
                }
            }

            if (!IsPostBack)
            {
                Cancel_Edit.NavigateUrl = "?category=" + _category.ToString();

                Master.FindControl("SideBarRegion").Visible = false;

                CustomFormSettings csf = CustomFormSettings.Get(_category, false);

                CustomField cf = null;
                Guid g = new Guid(Request.QueryString["id"]);
                foreach (CustomField cfx in csf.Fields)
                {
                    if (cfx.Id == g)
                    {
                        cf = cfx;
                        break;
                    }
                }

                if (cf != null)
                {
                    FormViews.SetActiveView(EditFieldView);

                    ExistingName.Text = cf.Name;
                    ExistingDescription.Text = cf.Description;

                    if (cf.FieldType == FieldType.CheckBox)
                    {
                        CheckboxRegion.Visible = true;
                        ExistingCheckBox.Checked = cf.Checked;
                    }
                    else if (cf.FieldType == FieldType.List)
                    {
                        ListRegion.Visible = true;
                        if (cf.ListOptions != null && cf.ListOptions.Count > 0)
                        {
                            foreach (ListItemFormElement li in cf.ListOptions)
                            {
                                ExistingListOptions.Text += (li.Text + "\n");
                            }
                        }
                    }
                }

            }
        }
        else
        {
            if (!IsPostBack)
            {
                Master.FindControl("SideBarRegion").Visible = true;

                if (_category != -1)
                {
                    Category c = new Category(_category);
                    lblCategory.Text = c.Name;
                }
                else
                    lblCategory.Text = "Global";

                LoadCategories();

                LoadCustomFields();
            }
        }
    }

    private void LoadCustomFields()
    {
        CustomFormSettings csf = CustomFormSettings.Get(_category, false);

        if (csf.Fields != null && csf.Fields.Count > 0)
        {
            CustomFieldList.Visible = true;

            ExistingFields.DataSource = csf.Fields;
            ExistingFields.DataBind();
        }
        else
        {
            CustomFieldList.Visible = false;
        }

        if (Request.QueryString["upd"] != null)
        {
            CustomField temp = csf.Fields.Find(
                                    delegate(CustomField cf)
                                    {
                                        return cf.Id == new Guid(Request.QueryString["upd"]);
                                    });

            Message.Text = "The custom field <strong>" + temp.Name + "</strong> was updated";
            Message.Type = StatusType.Success;
        }
    }

    private void LoadCategories()
    {
        CategoryCollection categories = new CategoryController().GetTopLevelCachedCategories();

        rptCategories.DataSource = categories;
        rptCategories.DataBind();
    }

    protected void NewFieldBTN_Click(object sender, EventArgs e)
    {
        if(!string.IsNullOrEmpty(FieldName.Text))
        {
            CustomFormSettings cfs = CustomFormSettings.Get(_category, false);
            if(cfs.Fields != null && cfs.Fields.Count > 0)
            {
                foreach(CustomField cf in cfs.Fields)
                {
                    if(Util.AreEqualIgnoreCase(FieldName.Text.Trim(),cf.Name))
                    {
                        Message.Text = "Field name already exists!";
                        Message.Type = StatusType.Error;
                        return;
                    }
                }
            }

            CustomField nfield = new CustomField();
            nfield.Name = FieldName.Text;
            nfield.Enabled = false;
            nfield.Id = Guid.NewGuid();
            nfield.FieldType = (FieldType) Enum.Parse(typeof (FieldType), TypesOfField.SelectedValue);

            cfs.Name = _category.ToString();

            cfs.Add(nfield);

            cfs.Save();

            Response.Redirect("?new=true&id=" + nfield.Id + "&category=" + _category.ToString());

        }
    }

    protected void UpdateFieldBTN_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(ExistingName.Text))
        {
            CustomFormSettings cfs = CustomFormSettings.Get(_category, false);

            CustomField cf = null;
            Guid g = new Guid(Request.QueryString["id"]);
            foreach (CustomField cfx in cfs.Fields)
            {
                if (cfx.Id == g)
                {
                    cf = cfx;
                    break;
                }
            }

            if (cf == null)
                throw new Exception("Custom Field does not exist to be edited.");

            if (cfs.Fields != null && cfs.Fields.Count > 0)
            {
                foreach (CustomField cfx in cfs.Fields)
                {
                    if (Util.AreEqualIgnoreCase(FieldName.Text.Trim(), cf.Name) && cfx.Id != cf.Id)
                    {
                        Message.Text = "Field Name already exists!";
                        Message.Type = StatusType.Error;
                        return;
                    }
                }
            }


            cf.Name = ExistingName.Text;
            cf.Description = ExistingDescription.Text;
            if (cf.FieldType == FieldType.CheckBox)
                cf.Checked = ExistingCheckBox.Checked;
            else if(cf.FieldType == FieldType.List)
            {
                string[] lines =
                    ExistingListOptions.Text.Split(new char[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);

                cf.ListOptions = new List<ListItemFormElement>();
                foreach(string line in lines)
                    cf.ListOptions.Add(new ListItemFormElement(line.Trim(),line.Trim()));

            }
            cf.Enabled = true;

            cfs.Save();

            Response.Redirect("~/graffiti-admin/site-options/custom-fields/?upd=" + cf.Id + "&category=" + _category.ToString());
        }
    }

    protected string IsSelectedCategory(string id)
    {
        if (id == (Request.QueryString["category"] ?? "-1"))
            return " class=\"selected\"";
        else
            return " class=\"notselected\"";
    }

    protected void lbDelete_Command(object sender, CommandEventArgs args)
    {
        string id = args.CommandArgument.ToString();

        CustomFormSettings cfs = CustomFormSettings.Get(_category, false);

        CustomField temp = cfs.Fields.Find(delegate(CustomField cf)
                                            {
                                                return cf.Id == new Guid(id);

                                            });

        if (temp != null)
            cfs.Fields.Remove(temp);

        cfs.Save();

        LoadCustomFields();
        Message.Text = "The custom field <b>" + temp.Name + "</b> was deleted.";
        Message.Type = StatusType.Success;
    }
}
