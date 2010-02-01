/*

 * Copyright (C) 2003-2007 Frederico Caldeira Knabben
 *
 * == BEGIN LICENSE ==
 *
 * Licensed under the terms of any of the following licenses at your
 * choice:
 *
 *  - GNU General Public License Version 2 or later (the "GPL")
 *    http://www.gnu.org/licenses/gpl.html
 *
 *  - GNU Lesser General Public License Version 2.1 or later (the "LGPL")
 *    http://www.gnu.org/licenses/lgpl.html
 *
 *  - Mozilla Public License Version 1.1 or later (the "MPL")
 *    http://www.mozilla.org/MPL/MPL-1.1.html
 *
 * == END LICENSE ==
 *
 * Editor configuration settings.
 *
 * Follow this link for more information:
 * http://wiki.teeditor.net/Developer%27s_Guide/Configuration/Configurations_Settings
 */

TEConfig.CustomConfigurationsPath = '' ;

TEConfig.EditorAreaCSS = TEConfig.BasePath + 'css/te_editorarea.css' ;
TEConfig.EditorAreaStyles = '' ;
TEConfig.ToolbarComboPreviewCSS = '' ;

TEConfig.DocType = '' ;

TEConfig.BaseHref = '' ;

TEConfig.FullPage = false ;

// The following option determines whether the "Show Blocks" feature is enabled or not at startup.
TEConfig.StartupShowBlocks = false ;

TEConfig.Debug = false ;
TEConfig.AllowQueryStringDebug = true ;

TEConfig.SkinPath = TEConfig.BasePath + 'skins/default/' ;
TEConfig.PreloadImages = [ TEConfig.SkinPath + 'images/toolbar.start.gif', TEConfig.SkinPath + 'images/toolbar.buttonarrow.gif' ] ;

TEConfig.PluginsPath = TEConfig.BasePath + 'plugins/' ;

// TEConfig.Plugins.Add( 'autogrow' ) ;
// TEConfig.Plugins.Add( 'dragresizetable' );
// TEConfig.AutoGrowMax = 500 ;

// TEConfig.ProtectedSource.Add( /<%[\s\S]*?%>/g ) ;	// ASP style server side code <%...%>
// TEConfig.ProtectedSource.Add( /<\?[\s\S]*?\?>/g ) ;	// PHP style server side code
// TEConfig.ProtectedSource.Add( /(<asp:[^\>]+>[\s|\S]*?<\/asp:[^\>]+>)|(<asp:[^\>]+\/>)/gi ) ;	// ASP.Net style tags <asp:control>

TEConfig.AutoDetectLanguage	= true ;
TEConfig.DefaultLanguage		= 'en' ;
TEConfig.ContentLangDirection	= 'ltr' ;

TEConfig.ProcessHTMLEntities	= true ;
TEConfig.IncludeLatinEntities	= true ;
TEConfig.IncludeGreekEntities	= true ;

TEConfig.ProcessNumericEntities = false ;

TEConfig.AdditionalNumericEntities = ''  ;		// Single Quote: "'"

TEConfig.FillEmptyBlocks	= true ;

TEConfig.FormatSource		= true ;
TEConfig.FormatOutput		= true ;
TEConfig.FormatIndentator	= '    ' ;

TEConfig.GeckoUseSPAN	= false ;
TEConfig.StartupFocus	= false ;
TEConfig.ForcePasteAsPlainText	= false ;
TEConfig.AutoDetectPasteFromWord = true ;	// IE only.
TEConfig.ShowDropDialog = true ;
TEConfig.ForceSimpleAmpersand	= false ;
TEConfig.TabSpaces		= 0 ;
TEConfig.ShowBorders	= true ;
TEConfig.SourcePopup	= false ;
TEConfig.ToolbarStartExpanded	= true ;
TEConfig.ToolbarCanCollapse	= true ;
TEConfig.IgnoreEmptyParagraphValue = true ;
TEConfig.PreserveSessionOnFileBrowser = false ;
TEConfig.FloatingPanelsZIndex = 10000 ;
TEConfig.HtmlEncodeOutput = false ;

TEConfig.TemplateReplaceAll = true ;
TEConfig.TemplateReplaceCheckbox = true ;

TEConfig.ToolbarLocation = 'In' ;

TEConfig.ToolbarSets["Default"] = [
	['Source','DocProps','-','Save','NewPage','Preview','-','Templates'],
	['Cut','Copy','Paste','PasteText','PasteWord','-','Print','SpellCheck'],
	['Undo','Redo','-','Find','Replace','-','SelectAll','RemoveFormat'],
	['Form','Checkbox','Radio','TextField','Textarea','Select','Button','ImageButton','HiddenField'],
	'/',
	['Bold','Italic','Underline','StrikeThrough','-','Subscript','Superscript'],
	['OrderedList','UnorderedList','-','Outdent','Indent','Blockquote'],
	['JustifyLeft','JustifyCenter','JustifyRight','JustifyFull'],
	['Link','Unlink','Anchor'],
	['Image','Flash','Table','Rule','Smiley','SpecialChar','PageBreak'],
	'/',
	['Style','FontFormat','FontName','FontSize'],
	['TextColor','BGColor'],
	['FitWindow','ShowBlocks','-','About']		// No comma for the last row.
] ;

TEConfig.ToolbarSets["Basic"] = [
	['Bold','Italic','-','OrderedList','UnorderedList','-','Link','Unlink','-','About']
] ;

TEConfig.ToolbarSets["Simple"] = [
	['Bold','Italic','Underline','StrikeThrough', '-', 'OrderedList','UnorderedList','-','Outdent','Indent','-', 'Link', 'Unlink', '-', 'Image', '-', 'SpellCheck', '-', 'Templates', '-', 'Source','-','FitWindow']
] ;

TEConfig.EnterMode = 'p' ;			// p | div | br
TEConfig.ShiftEnterMode = 'br' ;	// p | div | br

TEConfig.Keystrokes = [
	[ CTRL + 65 /*A*/, true ],
	[ CTRL + 67 /*C*/, true ],
	[ CTRL + 70 /*F*/, true ],
	[ CTRL + 83 /*S*/, true ],
	[ CTRL + 88 /*X*/, true ],
	[ CTRL + 86 /*V*/, 'Paste' ],
	[ SHIFT + 45 /*INS*/, 'Paste' ],
	[ CTRL + 88 /*X*/, 'Cut' ],
	[ SHIFT + 46 /*DEL*/, 'Cut' ],
	[ CTRL + 90 /*Z*/, 'Undo' ],
	[ CTRL + 89 /*Y*/, 'Redo' ],
	[ CTRL + SHIFT + 90 /*Z*/, 'Redo' ],
	[ CTRL + 76 /*L*/, 'Link' ],
	[ CTRL + 66 /*B*/, 'Bold' ],
	[ CTRL + 73 /*I*/, 'Italic' ],
	[ CTRL + 85 /*U*/, 'Underline' ],
	[ CTRL + SHIFT + 83 /*S*/, 'Save' ],
	[ CTRL + ALT + 13 /*ENTER*/, 'FitWindow' ],
	[ CTRL + 9 /*TAB*/, 'Source' ]
] ;

TEConfig.ContextMenu = ['Generic','Link','Anchor','Image','Flash','Select','Textarea','Checkbox','Radio','TextField','HiddenField','ImageButton','Button','BulletedList','NumberedList','Table','Form'] ;
TEConfig.BrowserContextMenuOnCtrl = true ;

TEConfig.EnableMoreFontColors = true ;
TEConfig.FontColors = '000000,993300,333300,003300,003366,000080,333399,333333,800000,FF6600,808000,808080,008080,0000FF,666699,808080,FF0000,FF9900,99CC00,339966,33CCCC,3366FF,800080,999999,FF00FF,FFCC00,FFFF00,00FF00,00FFFF,00CCFF,993366,C0C0C0,FF99CC,FFCC99,FFFF99,CCFFCC,CCFFFF,99CCFF,CC99FF,FFFFFF' ;

TEConfig.FontFormats	= 'p;h1;h2;h3;h4;h5;h6;pre;address;div' ;
TEConfig.FontNames		= 'Arial;Comic Sans MS;Courier New;Tahoma;Times New Roman;Verdana' ;
TEConfig.FontSizes		= 'smaller;larger;xx-small;x-small;small;medium;large;x-large;xx-large' ;

TEConfig.StylesXmlPath		= TEConfig.EditorPath + 'testyles.xml' ;
TEConfig.TemplatesXmlPath	= TEConfig.EditorPath + 'tetemplates.xml' ;

TEConfig.SpellChecker			= 'ieSpell' ;	// 'ieSpell' | 'SpellerPages'
TEConfig.IeSpellDownloadUrl	= 'http://www.iespell.com/download.php' ;
TEConfig.SpellerPagesServerScript = 'server-scripts/spellchecker.php' ;	// Available extension: .php .cfm .pl
TEConfig.FirefoxSpellChecker	= true ;

TEConfig.MaxUndoLevels = 15 ;

TEConfig.DisableObjectResizing = false ;
TEConfig.DisableFFTableHandles = true ;

TEConfig.LinkDlgHideTarget		= false ;
TEConfig.LinkDlgHideAdvanced	= false ;

TEConfig.ImageDlgHideLink		= false ;
TEConfig.ImageDlgHideAdvanced	= false ;

TEConfig.FlashDlgHideAdvanced	= false ;

TEConfig.ProtectedTags = '' ;

// This will be applied to the body element of the editor
TEConfig.BodyId = '' ;
TEConfig.BodyClass = '' ;

TEConfig.DefaultStyleLabel = '' ;
TEConfig.DefaultFontFormatLabel = '' ;
TEConfig.DefaultFontLabel = '' ;
TEConfig.DefaultFontSizeLabel = '' ;

TEConfig.DefaultLinkTarget = '' ;

// The option switches between trying to keep the html structure or do the changes so the content looks like it was in Word
TEConfig.CleanWordKeepsStructure = false ;

// Only inline elements are valid.
TEConfig.RemoveFormatTags = 'b,big,code,del,dfn,em,font,i,ins,kbd,q,samp,small,span,strike,strong,sub,sup,tt,u,var' ;

TEConfig.CustomStyles = 
{
	'Red Title'	: { Element : 'h3', Styles : { 'color' : 'Red' } }
};

// Do not add, rename or remove styles here. Only apply definition changes.
TEConfig.CoreStyles = 
{
	// Basic Inline Styles.
	'Bold'			: { Element : 'strong', Overrides : 'b' },
	'Italic'		: { Element : 'em', Overrides : 'i' },
	'Underline'		: { Element : 'u' },
	'StrikeThrough'	: { Element : 'strike' },
	'Subscript'		: { Element : 'sub' },
	'Superscript'	: { Element : 'sup' },
	
	// Basic Block Styles (Font Format Combo).
	'p'				: { Element : 'p' },
	'div'			: { Element : 'div' },
	'pre'			: { Element : 'pre' },
	'address'		: { Element : 'address' },
	'h1'			: { Element : 'h1' },
	'h2'			: { Element : 'h2' },
	'h3'			: { Element : 'h3' },
	'h4'			: { Element : 'h4' },
	'h5'			: { Element : 'h5' },
	'h6'			: { Element : 'h6' },
	
	// Other formatting features.
	'FontFace' : 
	{ 
		Element		: 'span', 
		Styles		: { 'font-family' : '#("Font")' }, 
		Overrides	: [ { Element : 'font', Attributes : { 'face' : null } } ]
	},
	
	'Size' :
	{ 
		Element		: 'span', 
		Styles		: { 'font-size' : '#("Size","fontSize")' }, 
		Overrides	: [ { Element : 'font', Attributes : { 'size' : null } } ]
	},
	
	'Color' :
	{ 
		Element		: 'span', 
		Styles		: { 'color' : '#("Color","color")' }, 
		Overrides	: [ { Element : 'font', Attributes : { 'color' : null } } ]
	},
	
	'BackColor'		: { Element : 'span', Styles : { 'background-color' : '#("Color","color")' } }
};

// The distance of an indentation step.
TEConfig.IndentLength = 40 ;
TEConfig.IndentUnit = 'px' ;

// Alternatively, TEeditor allows the use of CSS classes for block indentation.
// This overrides the IndentLength/IndentUnit settings.
TEConfig.IndentClasses = [] ;

// [ Left, Center, Right, Justified ]
TEConfig.JustifyClasses = [] ;

// The following value defines which File Browser connector and Quick Upload
// "uploader" to use. It is valid for the default implementaion and it is here
// just to make this configuration file cleaner.
// It is not possible to change this value using an external file or even
// inline when creating the editor instance. In that cases you must set the
// values of LinkBrowserURL, ImageBrowserURL and so on.
// Custom implementations should just ignore it.
var _FileBrowserLanguage	= 'aspx' ;	// asp | aspx | cfm | lasso | perl | php | py
var _QuickUploadLanguage	= 'aspx' ;	// asp | aspx | cfm | lasso | perl | php | py

// Don't care about the following two lines. It just calculates the correct connector
// extension to use for the default File Browser (Perl uses "cgi").
var _FileBrowserExtension = _FileBrowserLanguage == 'perl' ? 'cgi' : _FileBrowserLanguage ;
var _QuickUploadExtension = _QuickUploadLanguage == 'perl' ? 'cgi' : _QuickUploadLanguage ;

TEConfig.LinkBrowser = true ;
TEConfig.LinkBrowserURL = TEConfig.BasePath + '../../../graffiti-admin/site-options/utilities/FileSelector.aspx?path=files\\media';
TEConfig.LinkBrowserWindowWidth	= TEConfig.ScreenWidth * 0.7 ;		// 70%
TEConfig.LinkBrowserWindowHeight	= TEConfig.ScreenHeight * 0.7 ;	// 70%

TEConfig.ImageBrowser = true ;
TEConfig.ImageBrowserURL = TEConfig.BasePath + '../../../graffiti-admin/site-options/utilities/FileSelector.aspx?path=files\\media';
TEConfig.ImageBrowserWindowWidth  = TEConfig.ScreenWidth * 0.7 ;	// 70% ;
TEConfig.ImageBrowserWindowHeight = TEConfig.ScreenHeight * 0.7 ;	// 70% ;

TEConfig.FlashBrowser = false ;
TEConfig.FlashBrowserURL = TEConfig.BasePath + '../../../graffiti-admin/site-options/utilities/FileSelector.aspx?path=files\\media';
TEConfig.FlashBrowserWindowWidth  = TEConfig.ScreenWidth * 0.7 ;	//70% ;
TEConfig.FlashBrowserWindowHeight = TEConfig.ScreenHeight * 0.7 ;	//70% ;

TEConfig.LinkUpload = false;
TEConfig.LinkUploadURL = TEConfig.BasePath + 'filemanager/connectors/' + _QuickUploadLanguage + '/upload.' + _QuickUploadExtension ;
TEConfig.LinkUploadAllowedExtensions	= ".(7z|aiff|asf|avi|bmp|csv|doc|fla|flv|gif|gz|gzip|jpeg|jpg|mid|mov|mp3|mp4|mpc|mpeg|mpg|ods|odt|pdf|png|ppt|pxd|qt|ram|rar|rm|rmi|rmvb|rtf|sdc|sitd|swf|sxc|sxw|tar|tgz|tif|tiff|txt|vsd|wav|wma|wmv|xls|xml|zip)$" ;			// empty for all
TEConfig.LinkUploadDeniedExtensions	= "" ;	// empty for no one

TEConfig.ImageUpload = false;
TEConfig.ImageUploadURL = TEConfig.BasePath + 'filemanager/connectors/' + _QuickUploadLanguage + '/upload.' + _QuickUploadExtension + '?Type=Image' ;
TEConfig.ImageUploadAllowedExtensions	= ".(jpg|gif|jpeg|png|bmp)$" ;		// empty for all
TEConfig.ImageUploadDeniedExtensions	= "" ;							// empty for no one

TEConfig.FlashUpload = false;
TEConfig.FlashUploadURL = TEConfig.BasePath + 'filemanager/connectors/' + _QuickUploadLanguage + '/upload.' + _QuickUploadExtension + '?Type=Flash' ;
TEConfig.FlashUploadAllowedExtensions	= ".(swf|flv)$" ;		// empty for all
TEConfig.FlashUploadDeniedExtensions	= "" ;					// empty for no one

TEConfig.SmileyPath	= TEConfig.BasePath + 'images/smiley/msn/' ;
TEConfig.SmileyImages	= ['regular_smile.gif','sad_smile.gif','wink_smile.gif','teeth_smile.gif','confused_smile.gif','tounge_smile.gif','embaressed_smile.gif','omg_smile.gif','whatchutalkingabout_smile.gif','angry_smile.gif','angel_smile.gif','shades_smile.gif','devil_smile.gif','cry_smile.gif','lightbulb.gif','thumbs_down.gif','thumbs_up.gif','heart.gif','broken_heart.gif','kiss.gif','envelope.gif'] ;
TEConfig.SmileyColumns = 8 ;
TEConfig.SmileyWindowWidth		= 320 ;
TEConfig.SmileyWindowHeight	= 240 ;

