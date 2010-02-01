using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace DataBuddy
{
    public class SQLDataProvider : DataProvider
	{
		#region protected override DbProviderFactory GetFactory()

		protected override DbProviderFactory GetFactory()
        {
            return SqlClientFactory.Instance;
        }

		#endregion

		#region protected override void PrepareCommandParameters(List<Parameter> queryParamaters, DbCommand command)

        protected override void PrepareCommandParameters(List<Parameter> queryParamaters, DbCommand command)
        {
            if(queryParamaters != null && queryParamaters.Count > 0)
            {
                foreach (Parameter p in queryParamaters)
                {
                    SqlParameter sqlP = new SqlParameter(SqlVariable(p.Name), GetDbType(p.DbType));

					if (p.Length > -1)
						sqlP.Size = p.Length;

                    if (p.Value == null)
                        sqlP.Value = DBNull.Value;
                    else
                        sqlP.Value = p.Value;
                    
                    command.Parameters.Add(sqlP);
                    
                }
            }
		}

		#endregion

		#region protected override string GetSelectNextId(string tableName, string column)

		protected override string GetSelectNextId(string tableName, string column)
        {
            return "SELECT SCOPE_IDENTITY() as " + QuoteName("Next_ID");
		}

		#endregion

		#region protected static SqlDbType GetDbType(DbType dbType)

		protected static SqlDbType GetDbType(DbType dbType)
        {
            switch (dbType)
            {
                case DbType.AnsiString:
                    return SqlDbType.VarChar;
                case DbType.AnsiStringFixedLength:
                    return SqlDbType.Char;
                case DbType.Binary:
                    return SqlDbType.VarBinary;
                case DbType.Boolean:
                    return SqlDbType.Bit;
                case DbType.Byte:
                    return SqlDbType.TinyInt;
                case DbType.Currency:
                    return SqlDbType.Money;
                case DbType.Date:
                    return SqlDbType.DateTime;
                case DbType.DateTime:
                    return SqlDbType.DateTime;
                case DbType.Decimal:
                    return SqlDbType.Decimal;
                case DbType.Double:
                    return SqlDbType.Float;
                case DbType.Guid:
                    return SqlDbType.UniqueIdentifier;
                case DbType.Int16:
                    return SqlDbType.Int;
                case DbType.Int32:
                    return SqlDbType.Int;
                case DbType.Int64:
                    return SqlDbType.BigInt;
                case DbType.Object:
                    return SqlDbType.Variant;
                case DbType.SByte:
                    return SqlDbType.TinyInt;
                case DbType.Single:
                    return SqlDbType.Real;
                case DbType.String:
                    return SqlDbType.NVarChar;
                case DbType.StringFixedLength:
                    return SqlDbType.NChar;
                case DbType.Time:
                    return SqlDbType.DateTime;
                case DbType.UInt16:
                    return SqlDbType.Int;
                case DbType.UInt32:
                    return SqlDbType.Int;
                case DbType.UInt64:
                    return SqlDbType.BigInt;
                case DbType.VarNumeric:
                    return SqlDbType.Decimal;

                default:
                    {
                        return SqlDbType.VarChar;
                    }
            }
        }

        #endregion
    }
}