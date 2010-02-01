using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DataBuddy
{
    /// <summary>
    /// Used to filter a list of results
    /// </summary>
    public class Query
    {
        /// <summary>
        /// Queries are built off of a single table. 
        /// </summary>
        /// <param name="tbl"></param>
        public Query(Table tbl)
        {
            _Table = tbl;
        }

        #region Properties

        private string _top = "100 PERCENT";

        /// <summary>
        /// Defaults to 100 PERCENT
        /// </summary>
        public string  Top
        {
            get { return _top; }
            set { _top = value; }
        }

        private int _PageSize = 10;

        /// <summary>
        /// How many records to return per page. Default is 10.
        /// </summary>
        public int PageSize
        {
            get { return _PageSize; }
            set { _PageSize = value; }
        }

        private int _PageIndex;

        /// <summary>
        /// 1 based index. If PageIndex = 0, paging will be disabled.
        /// </summary>
        public int PageIndex
        {
            get { return _PageIndex; }
            set { _PageIndex = value; }
        }
	
	

        private Table _Table;

        /// <summary>
        /// Current table used for the query
        /// </summary>
        public Table Table { get { return _Table; } }

        #endregion

        #region Where
        private List<WHERE> _wheres = new List<WHERE>();

        public List<WHERE> Wheres { get { return _wheres; } }

        /// <summary>
        /// Add your own custom WHERE filters
        /// </summary>
        /// <param name="where"></param>
        public void AddWhere(WHERE where)
        {
            _wheres.Add(where);
        }

        /// <summary>
        /// "Ands" Column = value
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public void AndWhere(Column column, object  value)
        {
            AndWhere(column,value,Comparison.Equals);
        }

        /// <summary>
        /// Ands Column comp value
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <param name="comp"></param>
        public void AndWhere(Column column, object value, Comparison comp)
        {
            _wheres.Add(new ANDORWhere(column,value,comp,false));
        }

        /// <summary>
        /// Ors Column = value
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public void OrWhere(Column column, object value)
        {
            OrWhere(column, value, Comparison.Equals);
        }

        /// <summary>
        /// Ors Column comp value
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <param name="comp"></param>
        public void OrWhere(Column column, object value, Comparison comp)
        {
            _wheres.Add(new ANDORWhere(column, value, comp, true));
        }

        /// <summary>
        /// Ands an IN clause
        /// </summary>
        /// <param name="column"></param>
        /// <param name="strings"></param>
        public void AndInWhere(Column column, params string[] strings)
        {
            _wheres.Add(new INWhere(column,false,strings));
        }

        /// <summary>
        /// Ors an IN clause
        /// </summary>
        /// <param name="column"></param>
        /// <param name="ints"></param>
        public void AndInWhere(Column column, params int[] ints)
        {
            _wheres.Add(new INWhere(column, false, ints));
        }

        #endregion

        #region RecordCount

        /// <summary>
        /// Returns the number of records matching the current query.
        /// </summary>
        /// <returns></returns>
        public int GetRecordCount()
        {
            QueryCommand command = new QueryCommand("");
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT ")
                .Append( DataService.Provider.SqlCountFunction() )
                .Append(" as RecordCount FROM ")
				.Append( DataService.Provider.QuoteName( _Table.TableName ) );

            if (_wheres.Count > 0)
            {
                sb.Append(" WHERE ");

                int position = 1;
                foreach (WHERE where in _wheres)
                {
                    sb.Append(where.ToSQL(command, _Table, GetNextLetter(position), position == 1));
                    position++;
                }
            }

            command.Sql = sb.ToString();
            object obj = DataService.ExecuteScalar(command);
            int count = 0;
            if (obj == null || obj is DBNull)
                return count;

            if ( obj is Int32 )
                count = (int) obj;
            else if ( obj is Int64 )
                // this may cause errors in the future...if we begin to support Int64 keys
                count = (long) obj > Convert.ToInt64(Int32.MaxValue) ? Int32.MaxValue : Convert.ToInt32( (long) obj );
            else if ( obj is Int16 )
                count = Convert.ToInt32( (short) obj );
            
            return count;
        }

        #endregion

        #region Select

        /// <summary>
        /// Returns an IDataReader based on the query results. 
        /// </summary>
        /// <returns></returns>
        public IDataReader ExecuteReader()
        {
            QueryCommand command = DataService.CreateQueryCommandFromQuery(this);
            return DataService.ExecuteReader(command);
        }
     

        #endregion

        #region Orders

        /// <summary>
        /// Ads an Order by Asc
        /// </summary>
        /// <param name="column"></param>
        public void OrderByAsc(Column column)
        {
            OrderBy(column, "ASC");
        }

        /// <summary>
        /// Ads an Order by DESC
        /// </summary>
        /// <param name="column"></param>
        public void OrderByDesc(Column column)
        {
			OrderBy(column, "DESC");
		}

		private void OrderBy(Column column, string direction)
		{
			_orders.Add(string.Format("{0}.{1} {2}"
				, DataService.Provider.QuoteName( _Table.TableName )
				, DataService.Provider.QuoteName( column.Name )
				, direction
				));
		}

        private List<string> _orders = new List<string>();

        public List<string> Orders { get { return _orders; } }

        #endregion

        #region Helpers

        protected static string GetNextLetter(int position)
        {
            if (position <= 26)
                return Convert.ToString(letter_list[position - 1]);
            else
            {
                int offset = position % 26;
                if (offset == 0)
                    offset = 26;
                int iterations = (position / 26) + ((position % 26) > 0 ? 1 : 0);
                string letter = string.Empty;
                for (int i = 0; i < iterations; i++)
                    letter += letter_list[offset - 1];

                return letter;
            }
        }

        private static readonly char[] letter_list = "abcdefghijklmnopqrstuvxwzy".ToCharArray();

        #endregion
    }
}
