using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Graffiti.Core
{
	/// <summary>
	/// Defines a single custom form. This object is stored in the ObjectStore
	/// </summary>
	[Serializable]
	public class CustomFormSettings
	{
		public static readonly string DefaultCustomFormName = "Default::Custom::Form";
		public string Name;
		public int CategoryId;
		public int FormCategoryId;
		public bool IsNew = true;

		public List<CustomField> Fields;

		public void Add(CustomField field)
		{
			if (field.Id == Guid.Empty)
				field.Id = Guid.NewGuid();

			if (Fields == null)
				Fields = new List<CustomField>();

			Fields.Add(field);
		}

		public bool HasFields
		{
			get { return Fields != null && Fields.Count > 0; }
		}

		/// <summary>
		/// Returns a named custom form. The default post form is named "post".
		/// </summary>
		public static CustomFormSettings Get(int categoryId)
		{
			return Get(categoryId, true);
		}

		public static CustomFormSettings Get(int categoryId, bool processFormInheritance)
		{
			if (categoryId > 0)
				return Get(new CategoryController().GetCachedCategory(categoryId, false), processFormInheritance);
			else
				return Get();
		}

		public static CustomFormSettings Get(Category category)
		{
			return Get(category, true);
		}

		public static CustomFormSettings Get(Category category, bool processFormInheritance)
		{
			if (category == null)
				return Get();

			CustomFormSettings cfs = ObjectManager.Get<CustomFormSettings>("CustomFormSettings-" + category.Id);
			cfs.FormCategoryId = category.Id;

			if (processFormInheritance)
			{
				CustomFormSettings combinedCfs = new CustomFormSettings();
				combinedCfs.Name = category.Name;
				combinedCfs.FormCategoryId = category.Id;
				combinedCfs.IsNew = cfs.IsNew;
				combinedCfs.Fields = new List<CustomField>();

				if (cfs.Fields != null)
				{
					foreach (CustomField field in cfs.Fields)
					{
						combinedCfs.Fields.Add(field);
					}
				}

				CustomFormSettings cfs2;
				if (category.ParentId > 0)
				{
					cfs2 = ObjectManager.Get<CustomFormSettings>("CustomFormSettings-" + category.ParentId);

					if (cfs2.Fields != null)
					{
						foreach (CustomField field in cfs2.Fields)
						{
							combinedCfs.Fields.Add(field);
						}
					}

					if (combinedCfs.IsNew)
					{
						combinedCfs.FormCategoryId = category.ParentId;
						combinedCfs.IsNew = false;
					}
				}

				cfs2 = Get();
				if (cfs2.Fields != null)
				{
					foreach (CustomField field in cfs2.Fields)
					{
						combinedCfs.Fields.Add(field);
					}
				}

				if (combinedCfs.IsNew)
				{
					combinedCfs.FormCategoryId = -1;
					combinedCfs.IsNew = false;
				}

				cfs = combinedCfs;
			}

			cfs.CategoryId = category.Id;

			return cfs;
		}

		public static CustomFormSettings Get()
		{
			CustomFormSettings cfs = ObjectManager.Get<CustomFormSettings>(DefaultCustomFormName);
			cfs.FormCategoryId = -1;
			cfs.CategoryId = -1;

			return cfs;
		}

		public void Save()
		{
			if (string.IsNullOrEmpty(Name))
				throw new Exception("The name of the custom form cannot be null");

			this.IsNew = false;
			if (CategoryId == -1)
				ObjectManager.Save(this, DefaultCustomFormName);
			else
				ObjectManager.Save(this, "CustomFormSettings-" + CategoryId.ToString());
		}

		public string GetHtmlForm(NameValueCollection customFieldValues, bool isNew)
		{
			NameValueCollection fieldValues = new NameValueCollection();
			foreach (CustomField cf in this.Fields)
			{
				if (customFieldValues[cf.Name] != null)
					fieldValues[cf.Id.ToString()] = customFieldValues[cf.Name];
				else
					fieldValues[cf.Id.ToString()] = customFieldValues[cf.Id.ToString()];
			}

			StringBuilder sb = new StringBuilder();
			foreach (CustomField cf in this.Fields)
			{
				switch (cf.FieldType)
				{
					case FieldType.TextBox:
						new TextFormElement(cf.Id.ToString(), cf.Name, cf.Description).Write(sb, fieldValues);
						break;
		
					case FieldType.TextArea:
						new TextAreaFormElement(cf.Id.ToString(), cf.Name, cf.Description, cf.Rows).Write(sb, fieldValues);
						break;

					case FieldType.CheckBox:
						new CheckFormElement(cf.Id.ToString(), cf.Name, cf.Description, cf.Checked, isNew).Write(sb, fieldValues);
						break;

					case FieldType.File:
						new FileFormElement(cf.Id.ToString(), cf.Name, cf.Description).Write(sb, fieldValues);
						break;

					case FieldType.WYSIWYG:
						new WYWIWYGFormElement(cf.Id.ToString(), cf.Name, cf.Description).Write(sb, fieldValues);
						break;

					case FieldType.DateTime:
						new DateTimeFormElement(cf.Id.ToString(), cf.Name, cf.Description).Write(sb, fieldValues);
						break;

					case FieldType.List:
						if (cf.ListOptions != null && cf.ListOptions.Count > 0)
						{
							ListFormElement lfe = new ListFormElement(cf.Id.ToString(), cf.Name, cf.Description);
							foreach (ListItemFormElement life in cf.ListOptions)
								lfe.Add(life);

							lfe.Write(sb, fieldValues);
						}
						break;
				}
			}

			return sb.ToString();
		}
	}



}
