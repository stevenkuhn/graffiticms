using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Text;

namespace DataBuddy
{
    public class SQLiteProvider : DataProvider
    {
        static SQLiteProvider()
        {
            _factory = DbProviderFactories.GetFactory("System.Data.SQLite");
        }

		public override string QuoteName(string objectName)
		{
			return "[" + objectName + "]";
		}

        protected override string GetSelectNextId()
        {
            return " SELECT LAST_INSERT_ROWID() as newID";
        }

        public override T GetValue<T>(Column column, IDataReader reader)
        {
            object obj = reader[column.Name];
            if (obj == null || obj == DBNull.Value)
            {
                return default(T);
            }

            if (column.DbType == DbType.Int32)
            {
                obj =  Convert.ToInt32(obj);
            }
            else if(column.DbType == DbType.Double)
            {
                obj = Convert.ToDouble(obj);
            }
            else if(column.DbType == DbType.Guid) //Guids are treated as strings
            {
                obj = new Guid(obj.ToString());
            }
            else if(column.DbType == DbType.DateTime) //Dates are treated as strings
            {
                obj = DateTime.Parse(obj.ToString());
            }

            return (T) obj;
        }

        protected override void PrepareCommandParameters(List<Parameter> queryParamaters, DbCommand command)
        {
            if (queryParamaters != null && queryParamaters.Count > 0)
            {
                foreach (Parameter p in queryParamaters)
                {
                    SQLiteParameter parameter = new SQLiteParameter();
                    parameter.ParameterName = p.Name;
                    parameter.DbType = parameter.DbType;

                    if (p.Value == null)
                        parameter.Value = DBNull.Value;
                    else if(p.DbType == DbType.Guid) //Guids are not supported
                    {
                        parameter.DbType = DbType.String;
                        parameter.Value = p.Value.ToString();
                    }
                    else if(p.DbType == DbType.DateTime) //Dates are not really supported
                    {
                        parameter.Value = ((DateTime) p.Value).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    else
                        parameter.Value = p.Value;

                    command.Parameters.Add(parameter);
                }
            }
        }

        protected override string SelectPagedSql(string columnList, string tableName, string where, string order, string pk, int pageIndex, int pageSize)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT {0}", columnList);
            sb.Append(" FROM " + tableName);
            if (!string.IsNullOrEmpty(where))
                sb.Append(" WHERE ");

            sb.Append(where);

            if (!string.IsNullOrEmpty(order))
                sb.Append(" " + order);

            sb.AppendFormat(" LIMIT {0},{1}", pageSize*(pageIndex-1), pageSize);

            return sb.ToString();
        }

        protected override string SelectSql(string columnList, string tableName, string where, string order, string top)
        {
            StringBuilder sb = new StringBuilder("SELECT ");



            sb.Append(columnList);
            sb.AppendFormat(" FROM {0} ", QuoteName( tableName ) );

            if (!string.IsNullOrEmpty(where))
                sb.AppendFormat(" WHERE {0}", where);

            if (!string.IsNullOrEmpty(order))
            {
                sb.Append(order);
            }

            if (!string.IsNullOrEmpty(top))
            {
                if (top != "100 PERCENT")
                    sb.AppendFormat(" LIMIT {0} ", top);
            }

            return sb.ToString();
        }
    }
}