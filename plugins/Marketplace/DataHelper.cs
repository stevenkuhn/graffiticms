using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Graffiti.Marketplace
{
    /// <summary>
    /// Methods to access custom Marketplace sql objects
    /// </summary>
    internal static class DataHelper
    {

        internal static Dictionary<int, ItemStatistics> GetMarketplaceCategoryStats(int categoryID)
        {
            Dictionary<int, ItemStatistics> stats = new Dictionary<int, ItemStatistics>();
            using (SqlConnection connection = new SqlConnection(GraffitiConnString))
            {
                using (SqlCommand command = new SqlCommand("dbo.marketplace_ItemStats_Get_ByCat", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@categoryID", SqlDbType.Int)).Value = categoryID;

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);

                    while (reader.Read())
                    {
                        ItemStatistics s = new ItemStatistics();
                        s.DownloadCount = Convert.ToInt32(reader["DownloadCount"]);
                        int postID = Convert.ToInt32(reader["PostId"]);
                        stats.Add(postID, s);
                    }
                    reader.Close();

                }
            }

            return stats;
        }

        internal static ItemStatistics GetMarketplacePostStats(int postID)
        {
            ItemStatistics stats = new ItemStatistics();
            using (SqlConnection connection = new SqlConnection(GraffitiConnString))
            {
                using (SqlCommand command = new SqlCommand("dbo.marketplace_ItemStats_Get_ByPost", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@postId", SqlDbType.Int)).Value = postID;

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);

                    while (reader.Read())
                    {
                        stats.DownloadCount = Convert.ToInt32(reader["DownloadCount"]);
                    }
                    reader.Close();

                }
            }

            return stats;
        }

        internal static void UpdateMarketplaceStats(int postID)
        {
            using (SqlConnection connection = new SqlConnection(GraffitiConnString))
            {
                using (SqlCommand command = new SqlCommand("dbo.marketplace_ItemStats_Update", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@postId", SqlDbType.Int)).Value = postID;
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Handy helper to get the default Graffiti connection string
        /// </summary>
        internal static string GraffitiConnString
        {
            get { return ConfigurationManager.ConnectionStrings["Graffiti"].ConnectionString; }
        }
    }
}
