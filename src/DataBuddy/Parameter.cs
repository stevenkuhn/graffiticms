using System;
using System.Collections.Generic;
using System.Data;

namespace DataBuddy
{
    public class ParameterCollection : List<Parameter>
	{
		#region public Parameter Add(...) overloads

		public Parameter Add(string name, object value)
        {
            DbType dbType = DbType.Object;

            if (value is Int16 || value is Int32 || value is Int64)
                dbType = DbType.Int32;
            else if (value is String)
                dbType = DbType.String;
            else if (value is DateTime)
                dbType = DbType.DateTime;
            else if (value is Boolean)
                dbType = DbType.Boolean;
            else if (value is Guid)
                dbType = DbType.Guid;

            return Add(name, value,dbType);
        }

		public Parameter Add(string name, object value, DbType dbType)
        {
            Parameter p = new Parameter();
            p.Name = name;
            p.Value = value;
            p.DbType = dbType;
            return Add(p);
        }

		public new Parameter Add(Parameter parameter)
		{
			base.Add(parameter);
			
			return parameter;
		}

        #endregion
    }

    public class Parameter
    {
        #region Constructors

        public Parameter()
        {}
        
        /// <summary>
        /// Made to mimic the ParameterCollection.Add method, which already exists...
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
		/// <param name="dbType"></param>
        public Parameter(string name, object value, DbType dbType)
        {
            this._Name = name;
            this._value = value;
            this._dbType = dbType;
        }

        #endregion

        #region public string Name

        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        #endregion

        #region public DbType DbType

        private DbType _dbType = System.Data.DbType.Object;

        public DbType DbType
        {
            get { return _dbType; }
            set { _dbType = value; }
        }

        #endregion

		#region public int Length

		private int _Length = -1;

		public int Length
		{
			get { return _Length; }
			set { _Length = value; }
		}

		#endregion

		#region public object Value

        private object _value;

        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

	    #endregion
    }
}