using System.Text;

namespace DataBuddy
{
    public class SQL2K5DataProvider : SQLDataProvider
	{
		#region protected override string SelectPagedSql(string columnList, string tableName, string where, string order, string pk, int pageIndex, int pageSize)

		protected override string SelectPagedSql(string columnList, string tableName, string where, string order, string pk, int pageIndex, int pageSize)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("with __PagedTable (__ROW_INDEX, {0}) as (", columnList);
            sb.AppendFormat("SELECT Row_Number() OVER ({0}) as __ROW_INDEX, ",  order ?? ("Order by " + QuoteName(pk) + " DESC") );
            sb.Append(columnList);
            sb.Append(" FROM " + QuoteName(tableName));
            if (!string.IsNullOrEmpty(where))
                sb.Append(" WHERE ");

            sb.Append(where);

            sb.Append(")");

            sb.AppendFormat("SELECT * FROM __PagedTable where __ROW_INDEX > {0} AND __ROW_INDEX <= {1} ORDER BY __ROW_INDEX", (pageIndex - 1) * pageSize,
                            pageIndex * pageSize);

            return sb.ToString();
        }

		#endregion
    }
}