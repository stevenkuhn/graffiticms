using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace Graffiti.Core
{
	[Serializable]
	public abstract class Widget : EditableForm
	{
		private Guid _id;

		/// <summary>
		/// Id of the current Widget instance
		/// </summary>
		public Guid Id
		{
			get { return _id; }
			set { _id = value; }
		}

		private string _title = null;

		/// <summary>
		/// The title is usually renderd as an H3. You can change this
		/// by overriding the Render() method.
		/// </summary>
		public virtual string Title
		{
			get { return _title; }
			set { _title = value; }
		}

		private WidgetLocation _location = WidgetLocation.Queue;

		/// <summary>
		/// Where in the layout does this widget belong.
		/// </summary>
		public WidgetLocation Location
		{
			get { return _location; }
			set { _location = value; }
		}


		private int _order = Int16.MaxValue;

		/// <summary>
		/// The sort order of the widgets. This value does not have to be 
		/// unique.
		/// </summary>
		public int Order
		{
			get { return _order; }
			set { _order = value; }
		}


		/// <summary>
		/// Helper method for adding title. Just about every widget will have a title.
		/// </summary>
		protected static TextFormElement AddTitleElement()
		{
			return new TextFormElement("title", "Title", null);
		}

		/// <summary>
		/// This method can be overriden to control what elements are rendered on the edit screen.
		/// </summary>
		protected override FormElementCollection AddFormElements()
		{
			FormElementCollection fec = new FormElementCollection();
			fec.Add(AddTitleElement());
			return fec;
		}

		/// <summary>
		/// The data receieved during a postback.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="nvc">This value will always be the Request.Forms collection</param>
		/// <returns></returns>
		public override StatusType SetValues(HttpContext context, NameValueCollection nvc)
		{
			Title = nvc["title"];
			return StatusType.Success;
		}


		/// <summary>
		/// Renders the _sub_ data of a widget. This method is invoked by Render() which is 
		/// responsible for adding the title. 
		/// </summary>
		/// <returns></returns>
		public abstract string RenderData();



		/// <summary>
		/// Enables widget developers to selectively hide widgets from users at runtime.
		/// </summary>
		/// <returns></returns>
		public virtual bool IsUserValid()
		{
			return true;
		}

		/// <summary>
		/// Renders a widget to the screen.
		/// </summary>
		/// <returns></returns>
		public virtual string Render(string beforeTitle, string afterTitle, string beforeContent, string afterContent)
		{
			return
				 string.Format("{0}{1}{2}\n{3}{4}{5}", beforeTitle, Title, afterTitle, beforeContent, RenderData(),
									afterContent);
		}

		/// <summary>
		/// Enables a common data format to be used for the binding of the custom edit form.
		/// </summary>
		/// <returns></returns>
		protected override NameValueCollection DataAsNameValueCollection()
		{
			NameValueCollection nvc = new NameValueCollection();
			nvc["title"] = Title;
			return nvc;
		}

		internal NameValueCollection GetDefaults()
		{
			NameValueCollection nvc = new NameValueCollection();

			FormElementCollection fec = AddFormElements();
			if (fec != null)
			{
				foreach (FormElement fe in fec)
				{
					CheckFormElement cfe = fe as CheckFormElement;
					if (cfe != null)
					{
						nvc[cfe.Name] = cfe.DefaultValue.ToString();
					}
				}
			}

			return nvc;
		}

	}
}
