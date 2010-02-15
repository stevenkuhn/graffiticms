using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Graffiti.Core
{
	public class GraffitiEditor : UserControl, ITextControl, IPostBackDataHandler
	{

		public enum LanguageDir { rtl, ltr };
		protected string _text = string.Empty;
		protected static string _ckEditorJS = "~/__utility/ckeditor/ckeditor.js";
		protected static string _filebrowserUrl = VirtualPathUtility.ToAbsolute("~/graffiti-admin/site-options/utilities/FileSelector.aspx") + "?path=files\\\\media";
		

		public string Text { get { return _text; } set { _text = value; } }

		public static string CkEditorJS { get { return _ckEditorJS; } set { _ckEditorJS = value; } }
		public static string FilebrowserUrl { get { return _filebrowserUrl; } set { _filebrowserUrl = value; } }


		/// <summary>
		/// The URL path for the custom configuration file to be loaded. If not overloaded with inline configurations, it defaults to the "config.js" file present in the root of the CKEditor installation directory.
		/// CKEditor will recursively load custom configuration files defined inside other custom configuration files. 
		/// </summary>
		public string ExternalConfigJs { get { return (string)ViewState["exCfg"]; } set { ViewState["exCfg"] = value; } }

		/// <summary>
		/// A comma delimited set of extra config JS config for options that dont have custom properties on this class.
		/// </summary>
		/// <remarks>
		/// Because plugins can also have their properties set via config options, and this class doesnt implement every config option
		/// as a custom properly, you can add one ore more options via this general purpose property.
		/// <example>
		///     format_h1: {element:'h1', attributes:{'class':'contentH1'}},format_h6: {element:'h6', attributes:{'class':'contentH6'}}
		/// </example>
		/// </remarks>
		public string ExtraConfig { get { return (string)ViewState["extraCfg"]; } set { ViewState["extraCfg"] = value; } }

		/// <summary>
		/// The user interface language localization to use.
		/// </summary>
		public string Language { get { return (string)ViewState["lang"]; } set { ViewState["lang"] = value; } }

		/// <summary>
		/// The writting direction of the language used to write the editor contents. Allowed values are 'ltr' for Left-To-Right language (like English), or 'rtl' for Right-To-Left languages (like Arabic). 
		/// </summary>
		public LanguageDir LanguageDirection { get { return (LanguageDir)GetVstate("ldr", LanguageDir.ltr); } set { ViewState["ldr"] = value; } }

		/// <summary>
		/// Comma delimited list of extra plugins that should be loaded
		/// </summary>
		public string ExtraPlugins { get { return (string)ViewState["exPlug"]; } set { ViewState["exPlug"] = value; } }

		public string EditorSkin { get { return (string)ViewState["skin"]; } set { ViewState["skin"] = value; } }

		/// <summary>
		/// The CSS file to be used to apply style to the contents. It should reflect the CSS used in the final pages where the contents are to be used. 
		/// </summary>
		public string EditorContentCss { get { return (string)ViewState["eccss"]; } set { ViewState["eccss"] = value; } }

		/// <summary>
		/// The base href URL used to resolve relative and absolute URLs in the editor content. 
		/// </summary>
		public string BaseContentUrl { get { return (string)ViewState["bcu"]; } set { ViewState["bcu"] = value; } }

		public bool Resizable { get { return (bool)GetVstate("cRz", true); } set { ViewState["cRz"] = value; } }

		/// <summary>
		/// Whether to enable the "More Colors..." button in the color selectors. 
		/// </summary>
		public bool EnableMoreColors { get { return (bool)GetVstate("emclr", true); } set { ViewState["emclr"] = value; } }

		/// <summary>
		/// Whether to force all pasting operations to insert on plain text into the editor, loosing any formatting information possibly available in the source text. 
		/// </summary>
		public bool ForcePasteAsPlainText { get { return (bool)GetVstate("fcePPlain", false); } set { ViewState["fcePPlain"] = value; } }

		/// <summary>
		/// Whether the "Ignore font face definitions" checkbox is enabled by default in the Paste from Word dialog.
		/// </summary>
		public bool PastFromWordIgnoreFontFace { get { return (bool)GetVstate("pfwordigff", false); } set { ViewState["pfwordigff"] = value; } }

		/// <summary>
		/// Whether to keep structure markup (<h1>, <h2>, etc.) or replace it with elements that create more similar pasting results when pasting content from Microsoft Word into the Paste from Word dialog. 
		/// </summary>
		public bool PasteFromWordKeepsStructure { get { return (bool)GetVstate("pfwordkpstr", false); } set { ViewState["pfwordkpstr"] = value; } }

		/// <summary>
		/// Whether the "Remove styles definitions" checkbox is enabled by default in the Paste from Word dialog. 
		/// </summary>
		public bool PasteFromWordRemoveStyle { get { return (bool)GetVstate("pfwordremst", false); } set { ViewState["pfwordremst"] = value; } }

		public Unit Width { get { return (Unit)GetVstate("w", new Unit()); } set { ViewState["w"] = value; } }
		public Unit Height { get { return (Unit)GetVstate("h", new Unit()); } set { ViewState["h"] = value; } }
		public int MinWidth { get { return (int)GetVstate("miW", -1); } set { ViewState["miW"] = value; } }
		public int MaxWidth { get { return (int)GetVstate("maW", -1); } set { ViewState["maW"] = value; } }
		public int MinHeight { get { return (int)GetVstate("miH", -1); } set { ViewState["miH"] = value; } }
		public int MaxHeight { get { return (int)GetVstate("maH", -1); } set { ViewState["maH"] = value; } }

		public short TabIndex
		{
			get { object o = ViewState["TabIndex"]; return (o == null ? (short)0 : (short)o); }
			set { ViewState["TabIndex"] = value; }
		}

		/// <summary>
		/// The base Z-index for floating dialogs and popups.
		/// </summary>
		public int PopupZIndex { get { return (int)GetVstate("pzi", -1); } set { ViewState["pzi"] = value; } }

		/// <summary>
		/// The color to be used for the background of editor when a dialog is displayed.
		/// This is not the background color of the dialog itself, but the overlay shown around the dialog to cover up
		/// the overall page contents.
		/// </summary>
		public Color DialogCoverBackground { get { return (Color)GetVstate("dbgc", Color.Transparent); } set { ViewState["dbgc"] = value; } }

		/// <summary>
		/// The level of transparency to use for Dialog Cover when showing a popup.
		/// It should be a number within the range [0.0, 1.0]. 
		/// </summary>
		public string DialogCoverTransparency { get { return (string)ViewState["dbgt"]; } set { ViewState["dbgt"] = value; } }

		/// <summary>
		/// Whether the toolbar can be collapsed by the user. If disabled, the collapser button will not be displayed. 
		/// </summary>
		public bool CanCollapseToolbar { get { return (bool)GetVstate("canClpsTb", true); } set { ViewState["canClpsTb"] = value; } }

		/// <summary>
		/// Whether the toolbar must start expanded when the editor is loaded.
		/// </summary>
		public bool ToolbarStartExpanded { get { return (bool)GetVstate("tbsex", true); } set { ViewState["tbsex"] = value; } }

		public string ToolbarSet { get { return (string)ViewState["tb"]; } set { ViewState["tb"] = value; } }

		protected object GetVstate(string key, object defVal)
		{
			object item = ViewState[key];
			return item == null ? defVal : item;
		}

		public static void SetupScripts(Page page)
		{
			page.ClientScript.RegisterClientScriptInclude("GraffitiEditor.CkEditorMain", page.ResolveUrl(CkEditorJS));
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			SetupScripts(Page);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			writer.Write(GenerateHTML());
		}

		public string GenerateHTML()
		{
			return String.Format("<textarea id=\"{0}\" name=\"{1}\"{2}></textarea>\n<script type=\"text/javascript\">var t=\"{4}\";var e = CKEDITOR.instances.{0};if(e != null)CKEDITOR.remove(e);CKEDITOR.replace('{0}'{3}).setData(t);</script>",
				ClientID,
				UniqueID,
				this.TabIndex != 0 ? " tabindex=\"" + this.TabIndex.ToString() + "\"" : "",
				buildConfigOptions(),
				getTextForRender());
		}

		protected virtual string getTextForRender()
		{
			if (Text != null)
				return Text.Replace("\"", "\\\"").Replace("\r\n", "\\r\\n").Replace("\n", "\\n");
			return Text;
		}


		protected virtual string buildConfigOptions()
		{
			string options = innerBuildConfigOptions();
			if (options.Length > 0)
				return ",{" + options + "}";
			return "";
		}

		protected virtual string innerBuildConfigOptions()
		{
			StringBuilder buff = new StringBuilder();
			setupConfigParam(buff, "filebrowserBrowseUrl", FilebrowserUrl, true);
			if (ExternalConfigJs != null)
				setupConfigParam(buff, "customConfig", ExternalConfigJs, true);
			if (Language != null)
				setupConfigParam(buff, "language", Language, true);
			if (LanguageDirection != LanguageDir.ltr)
				setupConfigParam(buff, "contentsLangDirection", "rtl", true);
			if (ExtraPlugins != null)
				setupConfigParam(buff, "extraPlugins", ExtraPlugins, true);
			if (EditorContentCss != null)
				setupConfigParam(buff, "contentsCss", EditorContentCss, true);
			if (ToolbarSet != null)
				setupConfigParam(buff, "toolbar", ToolbarSet, true);
			if (!CanCollapseToolbar)
				setupConfigParam(buff, "toolbarCanCollapse", CanCollapseToolbar, false);
			if (!ToolbarStartExpanded)
				setupConfigParam(buff, "toolbarStartupExpanded", ToolbarStartExpanded, false);
			if (EnableMoreColors)
				setupConfigParam(buff, "colorButton_enableMore", EnableMoreColors, false);
			if (Width.Value != 0)
				setupConfigParam(buff, "width", Width, true);
			if (Height.Value != 0)
				setupConfigParam(buff, "height", Height, true);
			if (!Resizable)
				setupConfigParam(buff, "resize_enabled", Resizable, false);
			if (MinHeight != -1)
				setupConfigParam(buff, "resize_minHeight", MinHeight, false);
			if (MaxHeight != -1)
				setupConfigParam(buff, "resize_maxHeight", MaxHeight, false);
			if (MinWidth != -1)
				setupConfigParam(buff, "resize_minWidth", MinWidth, false);
			if (MaxWidth != -1)
				setupConfigParam(buff, "resize_maxWidth", MaxWidth, false);
			if (PopupZIndex != -1)
				setupConfigParam(buff, "baseFloatZIndex", PopupZIndex, false);
			if (EditorSkin != null)
				setupConfigParam(buff, "skin", EditorSkin, true);
			if (BaseContentUrl != null)
				setupConfigParam(buff, "baseHref", BaseContentUrl, true);
			if (DialogCoverBackground != Color.Transparent)
				setupConfigParam(buff, "dialog_backgroundCoverColor",
					 string.Format("rgb({0},{1},{2})", DialogCoverBackground.R, DialogCoverBackground.G, DialogCoverBackground.B), true);
			if (DialogCoverTransparency != null)
				setupConfigParam(buff, "dialog_backgroundCoverOpacity", DialogCoverTransparency, false);
			if (ForcePasteAsPlainText)
				setupConfigParam(buff, "forcePasteAsPlainText", ForcePasteAsPlainText, false);
			if (!PastFromWordIgnoreFontFace)
				setupConfigParam(buff, "pasteFromWordIgnoreFontFace", PastFromWordIgnoreFontFace, false);
			if (PasteFromWordKeepsStructure)
				setupConfigParam(buff, "pasteFromWordKeepsStructure", PasteFromWordKeepsStructure, false);
			if (PasteFromWordRemoveStyle)
				setupConfigParam(buff, "pasteFromWordRemoveStyle", PasteFromWordRemoveStyle, false);

			if (ExtraConfig != null)
			{
				if (buff.Length > 0)
					buff.Append(",");
				buff.Append(ExtraConfig);
			}

			return buff.ToString();
		}

		protected void setupConfigParam(StringBuilder buff, string name, object val, bool quoteVal)
		{
			if (buff.Length > 0)
				buff.Append(",");
			buff.Append(name).Append(":");
			if (quoteVal)
				buff.Append("'");
			if (val is bool)
				val = val.ToString().ToLower();
			buff.Append(val);
			if (quoteVal)
				buff.Append("'");
		}

		#region IPostBackDataHandler Members

		public bool LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
		{
			String postedValue = postCollection[postDataKey];
			Text = postedValue;

			// Because we are not managing view state for the text property, we cannot tell if the data has changed, so for
			// now we will just always return false.  View state can get heavy with a control like this.
			return false;
		}

		public void RaisePostDataChangedEvent()
		{
			// Never getting called right now as viewstate is not being used for text property.
		}

		#endregion

	}
}
