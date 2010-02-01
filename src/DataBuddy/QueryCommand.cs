using System.Collections.Generic;
using System.Data;

namespace DataBuddy
{
    public class QueryCommand
    {

        private readonly ParameterCollection _parameters = new ParameterCollection();
        private string _sql;

		/// <summary>
		/// If this override is used, you must set the Sql property as well.
		/// </summary>
        public QueryCommand()
		{}

        public QueryCommand(string sql)
        {
            _sql = sql;
        }

        public string Sql
        {
            get { return _sql; }
            set{ _sql = value;}
        }

        public ParameterCollection Parameters
        {
            get { return _parameters; }
        }

    }
}