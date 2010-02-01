using System.Collections.Generic;

namespace DataBuddy
{
    public abstract class WHERE
    {
        public abstract string ToSQL(QueryCommand cmd, Table tbl, string letter, bool isFirst);
    }
}