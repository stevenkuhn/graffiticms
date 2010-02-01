using System.Collections.Generic;

namespace DataBuddy
{
    public class WhereSet : WHERE
    {
        public override string ToSQL(QueryCommand cmd, Table tbl, string letter, bool isFirst)
        {
            if (_wheres.Count > 1)
            {
                string result = isFirst ? "" : (_useOr ? " OR " : " AND ");
                result += "(";

                int position = 1;
                foreach (WHERE where in _wheres)
                {
                    //result += (position == 1) ? " " : (_orChildren ? " OR " : " AND ");
                    result += where.ToSQL(cmd, tbl, letter + "_" + position, position == 1);
                    position++;
                }

                result += ")";

                return result;
            }
            else if (_wheres.Count == 1)
                return _wheres[0].ToSQL(cmd, tbl, letter, false);

            return string.Empty;
        }

        private List<WHERE> _wheres = new List<WHERE>();

        public void AddWhere(WHERE where)
        {
            _wheres.Add(where);
        }


        public void AndWhere(Column column, object value)
        {
            AndWhere(column, value, Comparison.Equals);
        }

        public void AndWhere(Column column, object value, Comparison comp)
        {
            _wheres.Add(new ANDORWhere(column, value, comp, false));
        }

        public void OrWhere(Column column, object value)
        {
            OrWhere(column, value, Comparison.Equals);
        }

        public void OrWhere(Column column, object value, Comparison comp)
        {
            _wheres.Add(new ANDORWhere(column, value, comp, true));
        }

        private bool _useOr = false;
        private bool _orChildren = false;

        public WhereSet()
            : this(false, false)
        {
        }

        public WhereSet(bool useOr, bool orChildren)
        {
            _useOr = useOr;
            _orChildren = orChildren;
        }

    }
}