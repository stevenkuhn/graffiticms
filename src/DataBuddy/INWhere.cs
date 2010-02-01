using System.Text;

namespace DataBuddy
{
    public class INWhere : WHERE
    {
        private int[] int_Ids = null;
        private string[] string_Keys = null;
        private bool _useOr = false;
        private Column _column = null;

        public override string ToSQL(QueryCommand cmd, Table tbl, string letter, bool isFirst)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(isFirst ? "" : (_useOr ? " OR " : " AND "))
                .Append( DataService.Provider.QuoteName( tbl.TableName ) )
				.Append( "." )
				.Append( DataService.Provider.QuoteName( _column.Name ) )
				.Append( " IN (" );

            if(int_Ids != null && int_Ids.Length > 0)
            {
                sb.Append( int_Ids[0] );
                
                for(int i = 1; i < int_Ids.Length; i++)
                {
                    sb.Append( ", " )
                        .Append( int_Ids[i] );
                }
            }
            else if(string_Keys != null && string_Keys.Length > 0)
            {
                sb.Append( DataService.Provider.SqlVariable( _column.Name + "_" + letter + "_0" ) );
                cmd.Parameters.Add( _column.Name + "_" + letter + "_0", string_Keys[0], _column.DbType );
                
                for(int i = 1; i<string_Keys.Length; i++)
                {
                    sb.Append( ", " )
                        .Append( DataService.Provider.SqlVariable( _column.Name + "_" + letter + "_" + i ) );

                    cmd.Parameters.Add( _column.Name + "_" + letter + "_" + i, string_Keys[i], _column.DbType );
                }
            }

            sb.Append(")");

            return sb.ToString();
        }

        public INWhere(Column column, bool useOr, params int[] ints)
        {
            int_Ids = ints;
            _column = column;
            _useOr = useOr;
        }

        public INWhere(Column column, bool useOr, params string[] strings)
        {
            string_Keys = strings;
            _column = column;
            _useOr = useOr;
        }
    }
}