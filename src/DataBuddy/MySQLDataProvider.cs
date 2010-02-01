using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using MySql.Data.MySqlClient;

namespace DataBuddy
{
	public class MySQLDataProvider : DataProvider
	{
	    #region Constructors

		public MySQLDataProvider()
		{
		    _QuoteCharacterPrefix = "`";
		    _QuoteCharacterPostfix = "`";
		    _VariablePrefix = "?";
		}
		#endregion

		#region protected override DbProviderFactory GetFactory()

		protected override DbProviderFactory GetFactory()
        {
            return MySqlClientFactory.Instance;
		}

		#endregion

		#region protected override void PrepareCommandParameters(List<Parameter> queryParamaters, DbCommand command)

		protected override void PrepareCommandParameters(List<Parameter> queryParamaters, DbCommand command)
        {
            if(queryParamaters != null && queryParamaters.Count > 0)
            {
                foreach (Parameter p in queryParamaters)
                {
                    MySqlParameter sqlP = new MySqlParameter(SqlVariable(p.Name), GetDbType(p.DbType));

					bool hasLength = (p.Length > -1);

					if (hasLength)
						sqlP.Size = p.Length;

                    if (p.Value == null)
                        sqlP.Value = DBNull.Value;
                    else
                    {
						if ( hasLength )
						{
							switch( p.DbType )
							{
								case DbType.AnsiString :
								case DbType.AnsiStringFixedLength :
								case DbType.String :
								case DbType.StringFixedLength :
								{
									string s_pValue = Convert.ToString( p.Value ) ?? string.Empty;

									if (s_pValue.Length > p.Length)
										s_pValue = s_pValue.Substring(0, p.Length);

									sqlP.Value = s_pValue;
								}
									break;
								default :
									sqlP.Value = p.Value;
									break;
							}
						}
						else
                            sqlP.Value = p.Value;
                    }

                    command.Parameters.Add(sqlP);
                    
                }
            }
        }

		#endregion

		#region public override QueryCommand CreateQueryCommandFromQuery(Query q)

		// MySQL does not support PERCENT, so we just remove top entirely

		public override QueryCommand CreateQueryCommandFromQuery(Query q)
		{
			if (q.Top == "100 PERCENT")
				q.Top = null;
			
			return base.CreateQueryCommandFromQuery(q);
		}

		#endregion

		#region protected override string SelectSql(string columnList, string tableName, string where, string order, string top)

		protected override string SelectSql(string columnList, string tableName, string where, string order, string top)
		{
			StringBuilder sb = new StringBuilder("SELECT ");

			sb.Append(columnList)
				.Append(" FROM ")
				.Append(QuoteName(tableName));

			if (!string.IsNullOrEmpty(where))
				sb.Append(" WHERE ")
					.Append(where);

			if (!string.IsNullOrEmpty(order))
				sb.Append(order);

			if (!string.IsNullOrEmpty(top))
				sb.Append(" LIMIT ")
					.Append(top);

			return sb.ToString();
		}

		#endregion

		#region protected override string SelectPagedSql(string columnList, string tableName, string where, string order, string pk, int pageIndex, int pageSize)

		protected override string SelectPagedSql(string columnList, string tableName, string where, string order, string pk, int pageIndex, int pageSize)
		{
            StringBuilder sb = new StringBuilder("SELECT ");

            sb.Append(columnList)
                .Append(" FROM ")
                .Append(QuoteName(tableName));

            if (!string.IsNullOrEmpty(where))
                sb.Append(" WHERE ")
                    .Append(where);

            if (!string.IsNullOrEmpty(order))
                sb.Append(order);

            sb.Append(" LIMIT ")
                .Append( pageSize );
            
            if( pageIndex > 1 )
                sb.Append( " OFFSET " )
                    .Append( ( pageIndex - 1 ) * pageSize );

            return sb.ToString();
        }

		#endregion

		#region protected override string GetSelectNextId(string tableName, string column)

		protected override string GetSelectNextId(string tableName, string column)
        {
            return "SELECT LAST_INSERT_ID() as " + QuoteName( "Next_ID" );
        }

		#endregion

        #region public override string GenerateDerivedView(string tableName)

        /// <summary>
        /// This assumes that tableName is already quoted if need be
        /// 
        /// This is presented as a "hack" for a combination of items
        /// MySQL 5.1x does not support updateable tables:
        ///     You can't specify target table 'TableName' for update in FROM clause
        /// But...VistaDB doesn't seem to support derived views within an UPDATE statement
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public override string GenerateDerivedView(string tableName)
        {
            string alias = Guid.NewGuid().ToString("N");
            return string.Format( " ( select {0}.* from {1} as {0} ) ", alias, tableName );
        }

        #endregion

		#region MySqlDbType Helper

        protected static MySqlDbType GetDbType(DbType dbType)
        {
			switch (dbType)
			{
				case DbType.AnsiString:
				case DbType.Guid:
				case DbType.String:
					return MySqlDbType.VarChar;
				case DbType.Byte:
				case DbType.Boolean:
					return MySqlDbType.UByte;
				case DbType.Currency:
				case DbType.Decimal:
					return MySqlDbType.Decimal;
				case DbType.Date:
					return MySqlDbType.Date;
				case DbType.DateTime:
					return MySqlDbType.DateTime;
				case DbType.Double:
					return MySqlDbType.Double;
				case DbType.Int16:
					return MySqlDbType.Int16;
				case DbType.Int32:
					return MySqlDbType.Int32;
				case DbType.Int64:
					return MySqlDbType.Int64;
				case DbType.SByte:
					return MySqlDbType.Byte;
				case DbType.Single:
					return MySqlDbType.Float;
				case DbType.Time:
					return MySqlDbType.Time;
				case DbType.UInt16:
					return MySqlDbType.UInt16;
				case DbType.UInt32:
					return MySqlDbType.UInt32;
				case DbType.UInt64:
					return MySqlDbType.UInt64;
				case DbType.AnsiStringFixedLength:
				case DbType.StringFixedLength:
					return MySqlDbType.String;
			}
			return MySqlDbType.Blob;
		}

        #endregion
    }
}
