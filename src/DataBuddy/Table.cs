using System.Collections.Generic;

namespace DataBuddy
{
    public class Table
    {
        private string _name;
        private string _typeName;
        private List<Column> _columns = new List<Column>();

        public Table(string name, string typeName)
        {
            _name = name;
            _typeName = typeName;
            
        }

        private bool  _isReadOnly;

        public bool  IsReadOnly
        {
            get { return _isReadOnly; }
            set { _isReadOnly = value; }
        }
	

        public string TableName
        {
            get { return _name; }
        }

        public string TypeName
        {
            get { return _typeName; }
        }

        public List<Column> Columns
        {
            get { return _columns; }
        }

        private string  _primaryKeyName;

        public string  PrimaryKey
        {
            get { return _primaryKeyName; }
            set { _primaryKeyName = value; }
        }

        public bool HasPrimaryKey
        {
            get { return !string.IsNullOrEmpty(PrimaryKey); }
        }
	
        
    }
}