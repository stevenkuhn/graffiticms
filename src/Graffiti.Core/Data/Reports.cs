using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using DataBuddy;

namespace Graffiti.Core
{
    public static class Reports
    {
        public static IDictionary<DateTime, int> ViewsByDateReport(DateTime min, DateTime max)
        {
			DataProvider dp = DataService.Provider;
            QueryCommand cmd = new QueryCommand(@"
                select " +
                    dp.SqlYearFunction("ps.DateViewed") + " as dvYear, " +
                    dp.SqlMonthFunction("ps.DateViewed") + " as dvMonth, " +
                    dp.SqlDayFunction("ps.DateViewed") + " as dvDay, " + 
                    dp.SqlCountFunction("ps.Id") + @" as IdCount
                from
                    graffiti_Post_Statistics AS ps
                left outer join
                    graffiti_Posts AS p on p.Id = ps.PostId
                where
                    ps.DateViewed >= " + dp.SqlVariable("MinDate") + @" and ps.DateViewed < " + dp.SqlVariable("MaxDate") + @"
                    and p.CategoryId in " + RolePermissionManager.GetInClauseForReadPermissions(GraffitiUsers.Current) + @"
                group by " +
                    dp.SqlYearFunction("ps.DateViewed") + ", " +
                    dp.SqlMonthFunction("ps.DateViewed") + ", " +
                    dp.SqlDayFunction("ps.DateViewed")
            );  

			Parameter pDateViewed = PostStatistic.FindParameter("DateViewed");
			cmd.Parameters.Add("MinDate", min, pDateViewed.DbType);
			cmd.Parameters.Add("MaxDate", max.AddDays(1), pDateViewed.DbType);

            return GetDateDictionary(cmd);
        }

        public static ReportData ViewsByDateSingle(DateTime date)
        {
			// top 10
			DataProvider dp = DataService.Provider;
			QueryCommand cmd = new QueryCommand(@"
                select Title, Id, IdCount FROM ( SELECT 
        	        max(p.Title) as Title, p.Id
        	        , " + dp.SqlCountFunction("ps.Id") + @" as IdCount
                from
                    graffiti_Post_Statistics AS ps
                left outer join
                    graffiti_Posts AS p on p.Id = ps.PostId
                where
                    ps.DateViewed >= " + dp.SqlVariable("MinDate") + @" and ps.DateViewed < " + dp.SqlVariable("MaxDate") + @"
                    and p.CategoryId in " + RolePermissionManager.GetInClauseForReadPermissions(GraffitiUsers.Current) + @"
                group by
                    p.Id) as dv
                order by
                    IdCount desc
                ");

			Parameter pDateViewed = PostStatistic.FindParameter("DateViewed");
            cmd.Parameters.Add("MinDate", date, pDateViewed.DbType);
            cmd.Parameters.Add("MaxDate", date.AddDays(1), pDateViewed.DbType);

            return GetPostDictionary(cmd, 10);
        }

        public static ReportData GetViewsByPost(DateTime min, DateTime max)
        {
			// top 10
			DataProvider dp = DataService.Provider;
			QueryCommand cmd = new QueryCommand(@"
                select Title, Id, IdCount FROM ( SELECT
	                max(p.Title) as Title, p.Id, " + dp.SqlCountFunction("p.Id") + @" as IdCount
                from
                    graffiti_Post_Statistics AS ps
                left outer join
                    graffiti_Posts AS p on p.Id = ps.PostId
                where
                    ps.DateViewed >= " + dp.SqlVariable("MinDate") + @" and ps.DateViewed < " + dp.SqlVariable("MaxDate") + @"
                    and p.CategoryId in " + RolePermissionManager.GetInClauseForReadPermissions(GraffitiUsers.Current) + @"
                group by
                    p.Id) as dv
                order by
                    IdCount desc
                ");

			Parameter pDateViewed = PostStatistic.FindParameter("DateViewed");
            cmd.Parameters.Add("MinDate", min, pDateViewed.DbType);
            cmd.Parameters.Add("MaxDate", max.AddDays(1), pDateViewed.DbType);

            return GetPostDictionary(cmd, 10);
        }

        public static IDictionary<DateTime, int> ViewsByPostSingle(int postId, DateTime min, DateTime max)
        {
			DataProvider dp = DataService.Provider;
			QueryCommand cmd = new QueryCommand(@"
                select " +
                    dp.SqlYearFunction("ps.DateViewed") + " as dvYear, " +
                    dp.SqlMonthFunction("ps.DateViewed") + " as dvMonth, " +
                    dp.SqlDayFunction("ps.DateViewed") + " as dvDay, " + 
                    dp.SqlCountFunction("ps.Id") + @" as IdCount
                from
                    graffiti_Post_Statistics AS ps
                where
                    ps.DateViewed >= " + dp.SqlVariable("MinDate") + @" and ps.DateViewed < " + dp.SqlVariable("MaxDate") + @"
                    and ps.PostId = " + dp.SqlVariable("PostId") + @"
                group by " +
                    dp.SqlYearFunction("ps.DateViewed") + ", " +
                    dp.SqlMonthFunction("ps.DateViewed") + ", " +
                    dp.SqlDayFunction("ps.DateViewed")
                );

			List<Parameter> parameters = PostStatistic.GenerateParameters();
			Parameter pDateViewed = PostStatistic.FindParameter(parameters, "DateViewed");
            cmd.Parameters.Add("MinDate", min, pDateViewed.DbType);
            cmd.Parameters.Add("MaxDate", max.AddDays(1), pDateViewed.DbType);
            cmd.Parameters.Add(PostStatistic.FindParameter(parameters, "PostId")).Value = postId;

            return GetDateDictionary(cmd);
        }

        public static int ViewsByPostSingleCount(int postId, DateTime min, DateTime max)
        {
			DataProvider dp = DataService.Provider;
			QueryCommand cmd = new QueryCommand(@"
                select
                    " + dp.SqlCountFunction("ps.Id") + @" as IdCount
                from
                    graffiti_Post_Statistics AS ps
                where
                    ps.DateViewed >= " + dp.SqlVariable("MinDate") + @" and ps.DateViewed < " + dp.SqlVariable("MaxDate") + @"
                    and ps.PostId = " + dp.SqlVariable("PostId")
                );

			List<Parameter> parameters = PostStatistic.GenerateParameters();
			Parameter pDateViewed = PostStatistic.FindParameter(parameters, "DateViewed");
			cmd.Parameters.Add("MinDate", min, pDateViewed.DbType);
            cmd.Parameters.Add("MaxDate", max.AddDays(1), pDateViewed.DbType);
            cmd.Parameters.Add(PostStatistic.FindParameter(parameters, "PostId")).Value = postId;

            return (int)DataService.ExecuteScalar(cmd);
        }

        public static ReportData CommentsByDateSingle(DateTime date)
        {
			// top 10
			DataProvider dp = DataService.Provider;
			QueryCommand cmd = new QueryCommand(@"
                select Title, Id, IdCount FROM ( SELECT
                    max(p.Title) as Title, p.Id
                    , " + dp.SqlCountFunction("c.Id") + @" as IdCount
                from
                    graffiti_Comments AS c
                left outer join
                    graffiti_Posts AS p on p.Id = c.PostId
                where
                    c.Published >= " + dp.SqlVariable("MinDate") + @" and c.Published < " + dp.SqlVariable("MaxDate") + @"
                    and p.CategoryId in " + RolePermissionManager.GetInClauseForReadPermissions(GraffitiUsers.Current) + @"
                    and c.IsDeleted = 0
                group by
                    p.Id ) as dv
                order by
                    IdCount desc
                ");

			Parameter pPublished = Comment.FindParameter("Published");
			cmd.Parameters.Add("MinDate", date, pPublished.DbType);
            cmd.Parameters.Add("MaxDate", date.AddDays(1), pPublished.DbType);

            return GetPostDictionary(cmd, 10);
        }

        public static ReportData CommentsByPost(DateTime min, DateTime max)
        {
			// top 10
			DataProvider dp = DataService.Provider;
			QueryCommand cmd = new QueryCommand(@"
                SELECT Title, Id, IdCount from (
            	   SELECT max(p.Title) as Title, p.Id, " + dp.SqlCountFunction("c.Id") + @" as IdCount
                from
                    graffiti_Comments AS c
                left outer join
                    graffiti_Posts AS p on p.Id = c.PostId
                where
                    c.Published >= " + dp.SqlVariable("MinDate") + @" and c.Published < " + dp.SqlVariable("MaxDate") + @"
                    and p.CategoryId in " + RolePermissionManager.GetInClauseForReadPermissions(GraffitiUsers.Current) + @"
                    and c.IsDeleted = 0
                group by
                    p.Id) as dv
                order by
                    IdCount desc
                ");

			Parameter pPublished = Comment.FindParameter("Published");
			cmd.Parameters.Add("MinDate", min, pPublished.DbType);
			cmd.Parameters.Add("MaxDate", max.AddDays(1), pPublished.DbType);

            return GetPostDictionary(cmd, 10);
        }

        public static IDictionary<DateTime, int> CommentsByPostSingle(int postId, DateTime min, DateTime max)
        {
			DataProvider dp = DataService.Provider;
			QueryCommand cmd = new QueryCommand(@"
                select " +
                    dp.SqlYearFunction("c.Published") + " as dvYear, " +
                    dp.SqlMonthFunction("c.Published") + " as dvMonth, " +
                    dp.SqlDayFunction("c.Published") + " as dvDay, " + 
	                dp.SqlCountFunction("c.Id") + @" as IdCount
                from
                    graffiti_Comments AS c
                where
                    c.Published >= " + dp.SqlVariable("MinDate") + @" and c.Published < " + dp.SqlVariable("MaxDate") + @"
	                and c.PostId = " + dp.SqlVariable("PostId") + @"
                    and c.IsDeleted = 0
                group by " +
                    dp.SqlYearFunction("c.Published") + ", " +
                    dp.SqlMonthFunction("c.Published") + ", " +
                    dp.SqlDayFunction("c.Published")
                );

			List<Parameter> parameters = Comment.GenerateParameters();
			Parameter pPublished = Comment.FindParameter(parameters, "Published");
			cmd.Parameters.Add("MinDate", min, pPublished.DbType);
			cmd.Parameters.Add("MaxDate", max.AddDays(1), pPublished.DbType);
            cmd.Parameters.Add(Comment.FindParameter(parameters, "PostId")).Value = postId;

            return GetDateDictionary(cmd);
        }

        public static int CommentsByPostSingleCount(int postId, DateTime min, DateTime max)
        {
			DataProvider dp = DataService.Provider;
			QueryCommand cmd = new QueryCommand(@"
                select
                    " + dp.SqlCountFunction("c.Id") + @" as IdCount
                from
                    graffiti_Comments c
                inner join
                    graffiti_Posts p on p.Id = c.PostId
                where
                    c.Published >= " + dp.SqlVariable("MinDate") + @" and c.Published < " + dp.SqlVariable("MaxDate") + @" and c.PostId = " + dp.SqlVariable("PostId") + @"
                    and c.IsDeleted = 0"
                );

			List<Parameter> parameters = Comment.GenerateParameters();
			Parameter pPublished = Comment.FindParameter(parameters, "Published");
			cmd.Parameters.Add("MinDate", min, pPublished.DbType);
			cmd.Parameters.Add("MaxDate", max.AddDays(1), pPublished.DbType);
			cmd.Parameters.Add(Comment.FindParameter(parameters, "PostId")).Value = postId;

            return (int)DataService.ExecuteScalar(cmd);
        }

        public static IDictionary<DateTime, int> CommentsByDate(DateTime min, DateTime max)
        {
			DataProvider dp = DataService.Provider;
			QueryCommand cmd = new QueryCommand(@"
                select " +
                    dp.SqlYearFunction("c.Published") + " as dvYear, " +
                    dp.SqlMonthFunction("c.Published") + " as dvMonth, " +
                    dp.SqlDayFunction("c.Published") + " as dvDay, " + 
                    dp.SqlCountFunction("c.Id") + @" as IdCount
                from
                    graffiti_Comments AS c
                left outer join
                    graffiti_Posts AS p on p.Id = c.PostId
                where
                    c.Published >= " + dp.SqlVariable("MinDate") + @" and c.Published < " + dp.SqlVariable("MaxDate") + @"
                    and p.CategoryId in " + RolePermissionManager.GetInClauseForReadPermissions(GraffitiUsers.Current) + @"
                    and c.IsDeleted = 0
                group by " +
                    dp.SqlYearFunction("c.Published") + ", " +
                    dp.SqlMonthFunction("c.Published") + ", " +
                    dp.SqlDayFunction("c.Published")
                );

			Parameter pPublished = Comment.FindParameter("Published");
			cmd.Parameters.Add("MinDate", min, pPublished.DbType);
			cmd.Parameters.Add("MaxDate", max.AddDays(1), pPublished.DbType);

            return GetDateDictionary(cmd);
        }

        public static ReportData MostPopularPosts()
        {
			// top 5
			DataProvider dp = DataService.Provider;
			QueryCommand cmd = new QueryCommand(@"
                SELECT Title, Id, IdCount FROM ( SELECT
	                p.Title, p.Id, " + dp.SqlCountFunction("p.Id") + @" as IdCount
                from
	                graffiti_Post_Statistics AS ps
                left outer join
	                graffiti_Posts AS p on p.Id = ps.PostId
                where p.CategoryId in " + RolePermissionManager.GetInClauseForReadPermissions(GraffitiUsers.Current) + @"
                group by
	                p.Title, p.Id) as dv
                order by
	                IdCount desc
                ");

            return GetPostDictionary(cmd, 5);
        }

		#region private static ReportData GetPostDictionary(...) overloads

		private static ReportData GetPostDictionary(QueryCommand command)
        {
			return GetPostDictionary(command, int.MaxValue);
        }

        private static ReportData GetPostDictionary(QueryCommand command, int top)
        {
            ReportData data = new ReportData();
			if ( top < 1 ) top = int.MaxValue;
			int counter = 0;

            using (IDataReader reader = DataService.ExecuteReader(command))
            {
                while (reader.Read())
                {
                    data.Counts.Add(Int32.Parse(reader["Id"].ToString()), Int32.Parse(reader["IdCount"].ToString()));
                    data.Titles.Add(Int32.Parse(reader["Id"].ToString()), reader["Title"] as string);
                    /* if for some reason the database return more than Int32.MaxValue this would still exit.  But, the IDictionaries wouldn't  */
                    counter++;
					if( counter == top ) break;
				}

                reader.Close();
            }
            return data;
        }

		#endregion

		#region private static IDictionary<DateTime, int> GetDateDictionary(QueryCommand command)

		private static IDictionary<DateTime, int> GetDateDictionary(QueryCommand command)
        {
            Dictionary<DateTime, int> dates = new Dictionary<DateTime, int>();
            using (IDataReader reader = DataService.ExecuteReader(command))
            {
                while (reader.Read())
                    dates.Add(
                        new DateTime(Int32.Parse(reader["dvYear"].ToString()), Int32.Parse(reader["dvMonth"].ToString()),
                                     Int32.Parse(reader["dvDay"].ToString())),
                        Int32.Parse( reader["IdCount"].ToString()) );

                reader.Close();
            }
            return dates;
        }

        #endregion
    }

    public class ReportData
    {
        public IDictionary<int, int> Counts = new Dictionary<int, int>();
        public IDictionary<int, string> Titles = new Dictionary<int, string>();
    }
}
