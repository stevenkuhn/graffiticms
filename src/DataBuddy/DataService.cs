using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text;
using System.Xml;
using DataBuddy;


namespace DataBuddy
{
    public static class DataService
    {
        private static DataProvider _dp = null;

        public static Type DataProviderType
        {
            get { return _dp.GetType(); }
        }

        public static DataProvider Provider
        {
            get { return _dp; }
        }

        static DataService()
        {
            string typeName = ConfigurationManager.AppSettings["DataBuddy::Provider"] ??
                              "DataBuddy.SQLDataProvider, DataBuddy";

            Type type = Type.GetType(typeName);

            if (type == null)
                throw new Exception("Could not initialized DataBuddy, Unknown Type: " + typeName);

            _dp = Activator.CreateInstance(type) as DataProvider;

            if (_dp == null)
                throw new Exception("Could not initialize DataBuddy, Provider Type Unknown: " + typeName);
        }

        public static int Destroy(Table table, Column column, object value)
        {
            return _dp.Destroy(table, column, value);
        }

        public static int Delete(Table table, Column column, object value)
        {
            return _dp.Delete(table, column, value);
        }

        public static int Insert(Table table, List<Parameter> parameters)
        {
          return  _dp.Insert(table, parameters);
        }

        public static void Update(Table table, List<Parameter>  parameters)
        {
            _dp.Update(table,parameters);
        }

        public static IDataReader ExecuteReader(QueryCommand cmd)
        {
            return _dp.ExecuteReader(cmd);
        }

        public static int ExecuteNonQuery(QueryCommand cmd)
        {
            return _dp.ExecuteNonQuery(cmd);
        }

        public static object ExecuteScalar(QueryCommand cmd)
        {
            return _dp.ExecuteScalar(cmd); 
        }

        public static QueryCommand CreateQueryCommandFromQuery(Query q)
        {
            return _dp.CreateQueryCommandFromQuery(q);
        }

        public static string GetComparisonOperator(Comparison comp)
        {
            string sOut = " = ";
            switch (comp)
            {
                case Comparison.Blank:
                    sOut = " ";
                    break;
                case Comparison.GreaterThan:
                    sOut = " > ";
                    break;
                case Comparison.GreaterOrEquals:
                    sOut = " >= ";
                    break;
                case Comparison.LessThan:
                    sOut = " < ";
                    break;
                case Comparison.LessOrEquals:
                    sOut = " <= ";
                    break;
                case Comparison.Like:
                    sOut = " LIKE ";
                    break;
                case Comparison.NotEquals:
                    sOut = " <> ";
                    break;
                case Comparison.NotLike:
                    sOut = " NOT LIKE ";
                    break;
                case Comparison.Is:
                    sOut = " IS ";
                    break;
                case Comparison.IsNot:
                    sOut = " IS NOT ";
                    break;
            }
            return sOut;
        }

        public static T GetValue<T>(Column column, IDataReader reader)
        {
            return _dp.GetValue<T>(column, reader);
        }
    }

    public enum Comparison
    {
        Blank,
        GreaterThan,
        GreaterOrEquals,
        LessThan,
        LessOrEquals,
        Like,
        NotEquals,
        NotLike,
        Is,
        IsNot, 
        Equals

    }
}
