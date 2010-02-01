using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using DataBuddy;

namespace Graffiti.Core
{
    public partial class TagWeight
    {
        public string VirtualUrl
        {
            get { return "~/tags/" + Name.ToLower() + "/"; }
        }

        public string Url
        {
            get { return VirtualPathUtility.ToAbsolute(VirtualUrl); }
        }


        private string _fontSize;

        public string FontSize
        {
            get { return _fontSize; }
            set { _fontSize = value; }
        }

        private int _fontFactor;

        public int FontFactor
        {
            get { return _fontFactor; }
            set { _fontFactor = value; }
        }
	
        public int Count
        {
            get { return Weight; }
        }
	
    }
}
