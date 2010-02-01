using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Text;

namespace DataBuddy
{
    public class MSAccessProvider: DataProvider
	{
		#region public override string ConnectionString

		private string _connectionString = null;

		public override string ConnectionString
		{
			get
			{
				if (_connectionString == null)
				{
					_connectionString = base.NormalizePath(base.ConnectionString);
				}
				return _connectionString;
			}
		}

		#endregion

		#region protected override DbProviderFactory GetFactory()

		protected override DbProviderFactory GetFactory()
        {
            return OleDbFactory.Instance;
        }

		#endregion

		#region public override int Insert(Table table, List<Parameter> parameters)

		public override int Insert(Table table, List<Parameter> parameters)
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
                    sb.AppendFormat(" {0} {1}"
						, isFirst ? string.Empty : ", "
						, QuoteName(column.Name)
						);
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

            sb.Append(")");
            command.Sql = sb.ToString();
            using(DbConnection conn = GetConnection())
            {
                conn.Open();
                DbCommand dbCommand = GetCommand(command, conn);
                dbCommand.ExecuteNonQuery();
                DbCommand c2 = GetFactory().CreateCommand();
                c2.CommandText = GetSelectNextId(table.TableName,table.PrimaryKey);
                c2.Connection = conn;
                object obj = c2.ExecuteScalar();
                conn.Close();
                return Int32.Parse(obj.ToString());
            }
        }

		#endregion

		#region private static OleDbType GetOleDbType(DbType dbType)

		private static OleDbType GetOleDbType(DbType dbType)
        {
            switch (dbType)
            {
                case DbType.AnsiString:
                    return OleDbType.VarBinary;
                case DbType.AnsiStringFixedLength:
                    return OleDbType.Char;
                case DbType.Binary:
                    return OleDbType.VarBinary;
                case DbType.Boolean:
                    return OleDbType.Boolean;
                case DbType.Byte:
                    return OleDbType.Single;
                case DbType.Currency:
                    return OleDbType.Currency;
                case DbType.Date:
                    return OleDbType.Date;
                case DbType.DateTime:
                    return OleDbType.Date;
                    //return OleDbType.Variant; //Hack?
                case DbType.Decimal:
                    return OleDbType.Decimal;
                case DbType.Double:
                    return OleDbType.Double;
                case DbType.Guid:
                    return OleDbType.Guid;
                case DbType.Int16:
                    return OleDbType.Integer;
                case DbType.Int32:
                    return OleDbType.Integer;
                case DbType.Int64:
                    return OleDbType.BigInt;
                case DbType.Object:
                    return OleDbType.Variant;
                case DbType.SByte:
                    return OleDbType.Single;
                case DbType.Single:
                    return OleDbType.Single;
                case DbType.String:
                    return OleDbType.VarChar;
                case DbType.StringFixedLength:
                    return OleDbType.WChar;
                case DbType.Time:
                    return OleDbType.DBTime;
                case DbType.UInt16:
                    return OleDbType.UnsignedSmallInt;
                case DbType.UInt32:
                    return OleDbType.UnsignedInt;
                case DbType.UInt64:
                    return OleDbType.UnsignedBigInt;
                case DbType.VarNumeric:
                    return OleDbType.Numeric;

                default:
                    {
                        return OleDbType.Variant;
                    }
            }
        }

		#endregion

		#region protected override void PrepareCommandParameters(List<Parameter> queryParamaters, DbCommand command)

		protected override void PrepareCommandParameters(List<Parameter> queryParamaters, DbCommand command)
        {
            if (queryParamaters != null && queryParamaters.Count > 0)
            {
                foreach (Parameter p in queryParamaters)
                {
                    OleDbParameter oleDbParamater = new OleDbParameter(SqlVariable(p.Name), GetOleDbType(p.DbType));

					if (p.Length > -1)
						oleDbParamater.Size = p.Length;

                    if (p.Value == null)
                    {
                        oleDbParamater.Value = DBNull.Value;
                    }
                    else if(p.DbType == DbType.DateTime)
                    {
                        oleDbParamater.Value = GetDateTimeWithOutMilliSeconds((DateTime)p.Value);
                    }
                    else
                        oleDbParamater.Value = p.Value;

                    command.Parameters.Add(oleDbParamater);
                }
            }
        }

		#endregion

        #region private static DateTime GetDateTimeWithOutMilliSeconds(DateTime dt)

		private static DateTime GetDateTimeWithOutMilliSeconds(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
        }

		#endregion

		#region public override QueryCommand CreateQueryCommandFromQuery(Query q)

		public override QueryCommand CreateQueryCommandFromQuery(Query q)
        {
            if(!string.IsNullOrEmpty(q.Table.PrimaryKey))
            {
                q.Orders.Add(QuoteName(q.Table.PrimaryKey) + " DESC");
            }
            return base.CreateQueryCommandFromQuery(q);
        }

		#endregion

		#region ### COMMENTED ###

        //protected override string SelectSql(string columnList, string tableName, string where, string order, string top)
        //{
        //    StringBuilder sb = new StringBuilder("SELECT ");
        //    if (!string.IsNullOrEmpty(top))
        //        sb.AppendFormat("TOP {0} * FROM ( SELECT ", top);

        //    sb.Append(columnList);
        //    sb.AppendFormat(" FROM {0} ", QuoteName(tableName));

        //    if (!string.IsNullOrEmpty(where))
        //        sb.AppendFormat(" WHERE {0}", where);

        //    if (!string.IsNullOrEmpty(order))
        //    {
        //        sb.Append(order);
        //    }

        //    if (!string.IsNullOrEmpty(top))
        //        sb.Append(")");

        //    return sb.ToString();
        //}

		#endregion
    }
}