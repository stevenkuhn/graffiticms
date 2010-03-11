using System;
using System.Collections.Generic;

namespace Graffiti.Core
{
	[Serializable]
	public class CustomField
	{
		private Guid _id;
		public Guid Id
		{
			get { return _id; }
			set { _id = value; }
		}


		private FieldType _fieldType = FieldType.TextBox;
		public FieldType FieldType
		{
			get { return _fieldType; }
			set { _fieldType = value; }
		}

		private string _name;
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		private string _description;
		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}


		private int _rows = 5;
		/// <summary>
		/// The number of rows to render for a text area type field
		/// </summary>
		public int Rows
		{
			get { return _rows; }
			set { _rows = value; }
		}

		private List<ListItemFormElement> _fields = new List<ListItemFormElement>();
		public List<ListItemFormElement> ListOptions
		{
			get { return _fields; }
			set { _fields = value; }
		}


		private bool _enabled;
		public bool Enabled
		{
			get { return _enabled; }
			set { _enabled = value; }
		}

		private bool _checked;
		/// <summary>
		/// Determines whether a CheckBox type field is checked by default
		/// </summary>
		public bool Checked
		{
			get { return _checked; }
			set { _checked = value; }
		}

	}

	public enum FieldType
	{
		CheckBox,
		List,
		TextBox,
		TextArea,
		File,
		WYSIWYG,
		DateTime
	}
}