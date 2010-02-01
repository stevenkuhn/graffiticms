using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;

namespace DataBuddy
{
    public abstract class DataProvider
    {
        #region private variables

        private static readonly char[] letter_list = "abcdefghijklmnopqrstuvxwzy".ToCharArray();

        #endregion

        #region protected variables

        protected string _QuoteCharacterPrefix = "[";
        protected string _QuoteCharacterPostfix = "]";
        protected string _VariablePrefix = "@";
        protected string _SqlCountFunctionFormatString = "count({0})";

        #endregion

        #region Insert
        public virtual int Insert(Table table, List<Parameter> parameters)
        {
            if (table.IsReadOnly)
                throw new Exception("Readonly tables (views) cannot recieve inserts");

            StringBuilder sb = new StringBuilder();
			QueryCommand command = new QueryCommand();

            sb.AppendFormat("INSERT INTO {0} (", QuoteName(table.TableName));

            bool isFirst = true;
            foreach (Column column in table.Columns)
            {
                if (column.Name != table.PrimaryKey)
                {
                    sb.AppendFormat(" {0} {1}", isFirst ? string.Empty : ", ", QuoteName(column.Name));
                    isFirst = false;
                }
            }

            sb.Append(") VALUES (");
            isFirst = true;

            foreach (Column column in table.Columns)
            {
                if (column.Name != table.PrimaryKey)
                {
                    sb.AppendFormat(" {0} {1}", isFirst ? string.Empty : ", ", SqlVariable(column.Name));
                    command.Parameters.Add( parameters.Find( delegate(Parameter p) { return (p.Name == column.Name); } ) );
                    isFirst = false;
                }
            }

            sb.AppendFormat("); {0}", GetSelectNextId(table.TableName,table.PrimaryKey));
            command.Sql = sb.ToString();
            object obj = ExecuteScalar(command);

            return Int32.Parse(obj.ToString());
        }

        protected virtual string GetSelectNextId(string tableName, string column)
        {
            return "SELECT @@IDENTITY as " + QuoteName("Next_ID") + ";";
        }

        #endregion

        #region Update

        public virtual void Update(Table table, List<Parameter>  parameters)
        {
            if (table.IsReadOnly)
                throw new Exception("Readonly tables (views) cannot be updated");

            StringBuilder sb = new StringBuilder();
			QueryCommand command = new QueryCommand();

			sb.AppendFormat("UPDATE {0} SET ", QuoteName(table.TableName));

            bool isFirst = true;
            foreach(Column column in table.Columns)
            {
                if (column.Name != table.PrimaryKey)
                {
                    sb.AppendFormat(" {0} {1} = {2} ", isFirst ? string.Empty : ", ", QuoteName(column.Name), SqlVariable(column.Name));
                    command.Parameters.Add( parameters.Find( delegate(Parameter p) { return (p.Name == column.Name); } ) );
                    isFirst = false;
                }
            }

            sb.AppendFormat(" WHERE {0} = {1}", QuoteName(table.PrimaryKey), SqlVariable(table.PrimaryKey));
            command.Parameters.Add( parameters.Find( delegate(Parameter p) { return (p.Name == table.PrimaryKey); } ) );

            command.Sql = sb.ToString();

            ExecuteNonQuery(command);

        }
        #endregion

        #region Delete/Destroy

        public virtual int Destroy(Table table, Column column, object value)
        {
            QueryCommand command = new QueryCommand(string.Format(
				"DELETE FROM {0} WHERE {1} = {2}"
				, QuoteName(table.TableName)
				, QuoteName(column.Name)
				, SqlVariable(column.Name)
				));

            command.Parameters.Add( column.Name, value, column.DbType );
            return ExecuteNonQuery(command);
        }

        public virtual int Delete(Table table, Column column, object value)
        {
            QueryCommand command = new QueryCommand(string.Format(
				"UPDATE {0} SET IsDeleted = 1 WHERE {1} = {2}"
				, QuoteName(table.TableName)
				, QuoteName(column.Name)
				, SqlVariable(column.Name)
				));

            command.Parameters.Add( column.Name, value, column.DbType );
            return ExecuteNonQuery(command);
        }

        #endregion

        #region Commands
        public virtual  IDataReader ExecuteReader(QueryCommand cmd)
        {
			Guid marker = DbTrace.GetMarker();
			DbCommand command = GetCommand(marker, cmd, GetConnection());
            command.Connection.Open();
			DbTrace.Write(marker, "Connection Open");
			IDataReader dr = command.ExecuteReader(CommandBehavior.CloseConnection);
			DbTrace.Write(marker, "IDataReader returned");
			return dr;
        }

        protected virtual DbConnection GetConnection()
        {
            DbConnection conn = GetFactory().CreateConnection();
            conn.ConnectionString = ConnectionString;
            return conn;
        }

        protected virtual DbCommand GetCommand(QueryCommand qryCommand, DbConnection conn)
        {
            return GetCommand(DbTrace.GetMarker(), qryCommand, conn);
        }

		protected virtual DbCommand GetCommand(Guid marker, QueryCommand qryCommand, DbConnection conn)
		{
			DbCommand command = GetFactory().CreateCommand();
			command.CommandText = qryCommand.Sql;
			PrepareCommandParameters(qryCommand.Parameters, command);
			command.Connection = conn;
			DbTrace.Write(marker, command);
			return command;
		}

		public virtual int ExecuteNonQuery(QueryCommand cmd)
        {
            using (DbConnection conn = GetConnection())
            {
				Guid marker = DbTrace.GetMarker();
				DbCommand command = GetCommand(marker, cmd, conn);
                conn.Open();
                DbTrace.Write(marker, "Connection Open");
                int rowCount = command.ExecuteNonQuery();
				DbTrace.Write(marker, "ExecuteNonQuery completed");
				conn.Close();
                return rowCount;
            }
            
        }

        public virtual  object ExecuteScalar(QueryCommand cmd)
        {
            using (DbConnection conn = GetConnection())
            {
				Guid marker = DbTrace.GetMarker();
				DbCommand command = GetCommand(cmd, conn);
                conn.Open();
				DbTrace.Write(marker, "Connection Open");
				object obj = command.ExecuteScalar();
				DbTrace.Write(marker, "ExecuteScalar completed");
				conn.Close();
                return obj;
            }
        }
        #endregion

        #region Factory

        protected abstract DbProviderFactory GetFactory();

        #endregion

		#region protected virtual ITrace DbTrace

		protected ITrace _Trace;

		/// <summary>
		/// Use
		/// <code>
		///		<add key="DataBuddy::EnableTracing" value="true"/>
		/// </code>
		/// to enable the tracing
		/// </summary>
		public virtual ITrace DbTrace
		{
			get { return TraceManager.Instance; }
		}

		#endregion

		#region protected string NormalizePath(string connectionString)

		/// <summary>
		/// Changes path separators in connection strings.  This does not parse the
		/// connection string, it merely replaces one path separator with another.
		/// </summary>
		/// <param name="connectionString"></param>
		/// <returns></returns>
		protected string NormalizePath(string connectionString)
		{
			char windowsPathSeparator = '\\';
			char unixPathSeparator = '/';

			return ( Environment.OSVersion.Platform != PlatformID.Unix )
				// If it is not unix, replace / with \
				? connectionString.Replace( unixPathSeparator, windowsPathSeparator )
				// If it is unix, replace \ with /
				: connectionString.Replace( windowsPathSeparator, unixPathSeparator );
		}

		#endregion

        #region public virtual string QuoteName(string objectName)

        public virtual string QuoteName(string objectName)
        {
            if (objectName == null) objectName = string.Empty;
            return _QuoteCharacterPrefix
                + objectName.Replace(
                    _QuoteCharacterPostfix
                    , _QuoteCharacterPostfix + _QuoteCharacterPostfix
                    )
                + _QuoteCharacterPostfix;
        }

        #endregion

        #region public virtual string SqlVariable(string objectName)

        public virtual string SqlVariable(string objectName)
        {
            if (objectName == null) objectName = string.Empty;
            return _VariablePrefix + objectName;
        }

        #endregion

        #region public virtual string SqlCountFunction(...) overloads

        public virtual string SqlCountFunction()
        {
            return SqlCountFunction( "*" );
        }

        public virtual string SqlCountFunction(string expression)
        {
            return string.Format( _SqlCountFunctionFormatString, expression );
        }

        #endregion

        #region public virtual string GenerateDerivedView(string tableName)

        /// <summary>
        /// This is presented as a "hack" for a combination of items
        /// MySQL 5.1x does not support updateable tables:
        ///     You can't specify target table 'graffiti_Categories' for update in FROM clause
        /// But...VistaDB doesn't seem to support derived views within an UPDATE statement
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public virtual string GenerateDerivedView(string tableName)
        {
            return tableName;
        }

        #endregion

        #region protected virtual void PrepareCommandParameters(List<Parameter> queryParamaters, DbCommand command)

        protected virtual void PrepareCommandParameters(List<Parameter> queryParamaters, DbCommand command)
        {
            if (queryParamaters != null && queryParamaters.Count > 0)
            {
                foreach (Parameter p in queryParamaters)
                {
                    DbParameter dbParameter = GetFactory().CreateParameter();
                    dbParameter.DbType = p.DbType;
                    dbParameter.ParameterName = SqlVariable(p.Name);

					if (p.Length > -1)
						dbParameter.Size = p.Length;

                    if (p.Value == null)
						dbParameter.Value = DBNull.Value;
                    else
                        dbParameter.Value = p.Value;

                    command.Parameters.Add(dbParameter);
                }
            }
        }

        #endregion

        #region public virtual string ConnectionString

        public virtual string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["DataBuddy::ConnectionString"]].ConnectionString; }
        }

        #endregion

        #region public virtual T GetValue<T>(Column column, IDataReader reader)

        public virtual T GetValue<T>(Column column, IDataReader reader)
        {
            object obj = reader[column.Name];

            if (obj == null || obj == DBNull.Value)
            {
                return default(T);
            }
            else
            {
                if (column.DbType == DbType.Guid && !(obj is Guid))
                {
                    // if we expect a guid, the database platform might not handle this specific data type
                    obj = new Guid((string)obj);
                }

				// this is included as a hack for MySQL - COUNT(*) returns an Int64 always
				// most of Graffiti assumes that COUNT(*) returns an Int32
				if (obj is Int64 && column.DbType != DbType.Int64 && column.DbType != DbType.UInt64)
				{
					if (default(T) is Int32)
						obj = Convert.ToInt32(obj);
				}

                return (T)obj;
            }
        }

        #endregion

        #region public virtual QueryCommand CreateQueryCommandFromQuery(Query q)

        public virtual QueryCommand CreateQueryCommandFromQuery(Query q)
        {
            StringBuilder columnList = new StringBuilder();
            QueryCommand command = new QueryCommand("");
            bool isFirst = true;
            foreach (Column column in q.Table.Columns)
            {
                columnList.AppendFormat("{1} {0}", QuoteName(column.Name), isFirst ? string.Empty : ",");
                isFirst = false;
            }

            string whereStatement = null;
            if (q.Wheres.Count > 0)
            {
                StringBuilder _whereStatement = new StringBuilder();
                int position = 1;
                foreach (WHERE where in q.Wheres)
                {
                    _whereStatement.Append(where.ToSQL(command, q.Table, GetNextLetter(position), position == 1));
                    position++;
                }
                whereStatement = _whereStatement.ToString();
            }

            string orderStatement = null;

            if (q.Orders.Count > 0)
            {
                isFirst = true;
                foreach (string order in q.Orders)
                {
                    if (isFirst)
                        orderStatement = " Order By ";

                    orderStatement += (isFirst ? " " : " , ") + order;
                    isFirst = false;
                }
            }

            if (q.PageIndex == 0 || q.PageSize == 0)
            {
                command.Sql = SelectSql(columnList.ToString(),q.Table.TableName,whereStatement,orderStatement, q.Top);
            }
            else if (q.PageIndex == 1)
            {
                command.Sql = SelectSql(columnList.ToString(), q.Table.TableName, whereStatement, orderStatement, q.PageSize.ToString());
            }
            else
            {
                command.Sql =
                    SelectPagedSql(columnList.ToString(), q.Table.TableName, whereStatement, orderStatement,
                                   q.Table.PrimaryKey, q.PageIndex, q.PageSize);
            }

            return command;
        }

        #endregion

        #region protected virtual string SelectSql(string columnList, string tableName, string where, string order, string top)

        protected virtual string SelectSql(string columnList, string tableName, string where, string order, string top)
        {
            StringBuilder sb = new StringBuilder("SELECT ");
            if (!string.IsNullOrEmpty(top))
                sb.AppendFormat("TOP {0} ", top);

            sb.Append(columnList);
            sb.AppendFormat(" FROM {0} ", QuoteName(tableName));

            if (!string.IsNullOrEmpty(where))
                sb.AppendFormat(" WHERE {0}", where);

            if(!string.IsNullOrEmpty(order))
            {
                sb.Append(order);
            }

            return sb.ToString();
        }

        #endregion

        #region protected virtual string SelectPagedSql(string columnList, string tableName, string where, string order, string pk, int pageIndex, int pageSize)

        protected virtual string SelectPagedSql(string columnList, string tableName, string where, string order, string pk, int pageIndex, int pageSize)
        {
            StringBuilder sb = new StringBuilder("SELECT ");


            sb.Append(columnList);
            sb.AppendFormat(" FROM {0} ", QuoteName(tableName));

            sb.Append(" WHERE ");

            if (!string.IsNullOrEmpty(where))
                sb.AppendFormat(" ({0}) AND {1} in ", where, QuoteName(pk));
            else
                sb.AppendFormat(" {0} in ", QuoteName(pk));

            sb.AppendFormat(" (SELECT TOP {0} {1} FROM {2} WHERE ", pageSize, QuoteName(pk), QuoteName(tableName));

            if (!string.IsNullOrEmpty(where))
                sb.AppendFormat(" ({0}) AND {1} NOT IN (SELECT TOP {2} {1} FROM {3} WHERE ({0}) {4} "
					, where
					, QuoteName(pk)
					, ((pageIndex -1) * pageSize)
					, QuoteName(tableName)
					, order
					);
            else
                sb.AppendFormat(" {0} NOT IN (SELECT TOP {1} {0} FROM {2} {3} "
					, QuoteName(pk)
					, ((pageIndex - 1) * pageSize)
					, QuoteName(tableName)
					, order
					);

            sb.AppendFormat(") {0}) ", order);

            if (!string.IsNullOrEmpty(order))
            {
                sb.Append(order);
            }

            return sb.ToString();
        }

        #endregion

        #region protected static string GetNextLetter(int position)

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

        #endregion
    }
}