using System.Collections.Generic;

namespace DataBuddy
{
    public class Schema : List<Table>
    {
        private string  _nameSpace;

        public string  NameSpace
        {
            get { return _nameSpace; }
            set { _nameSpace = value; }
        }
	
    }
}