using System;
using System.Data;

namespace DataBuddy
{
    public class Column
    {
        public Column()
        {
        }

        public Column(string name, DbType dtType, Type type, string alias, bool allowNull, bool isPK)
        {
            _name = name;
            _dbType = dtType;
            _type = type;
            _alias = alias;
            _isPrimaryKey = isPK;
            _allowNull = allowNull;
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private DbType _dbType;

        public DbType DbType
        {
            get { return _dbType; }
            set { _dbType = value; }
        }

        private Type _type;

        public Type Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private string _alias;

        public string Alias
        {
            get
            {
                if (string.IsNullOrEmpty(_alias))
                    return Name;

                return _alias;
            }
            set
            {
                _alias = value;
            }
        }


        private bool _isPrimaryKey;

        public bool IsPrimaryKey
        {
            get { return _isPrimaryKey; }
            set { _isPrimaryKey = value; }
        }

        private bool _allowNull;

        public bool AllowNull
        {
            get { return _allowNull; }
            set { _allowNull = value; }
        }

		private int _Length = -1;

		public int Length
		{
			get { return _Length; }
			set { _Length = value; }
		}
	}
}