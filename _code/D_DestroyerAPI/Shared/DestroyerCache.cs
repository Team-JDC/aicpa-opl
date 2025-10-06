using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
namespace AICPA.Destroyer.Shared
{

    // Global cache of names -> titles (refresh at startup and on a timer if you want)
    public static class DestroyerCache
    {
        private static readonly object _sync = new object();
        // Change dictionaries to store both Name and Title with int key
        public static Dictionary<string, (string Name, string Title)> SiteTitleById = new Dictionary<string, (string, string)>();
        public static Dictionary<string, (string Name, string Title)> FolderTitleById = new Dictionary<string, (string, string)>();
        public static Dictionary<string, (string Name, string Title)> BookTitleById = new Dictionary<string, (string, string)>();

        public static void LoadAll(string connectionString)
        {
            lock (_sync)
            {
                var site = new Dictionary<string, (string Name, string Title)>();
                var folder = new Dictionary<string, (string Name, string Title)>();
                var book = new Dictionary<string, (string Name, string Title)>();
                using (var cn = new SqlConnection(connectionString))
                {
                    cn.Open();

                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT SiteId, Title,Name FROM dbo.D_Site";
                        using (var r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                string id = Convert.ToString(r["SiteId"]);
                                string name = r["Name"] as string ?? "";
                                string title = r["Title"] as string ?? "";
                                site[id] = (name, title);
                            }
                        }
                    }

                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT FolderId, Title,Name FROM dbo.D_SiteFolder";
                        using (var r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                string id = Convert.ToString(r["FolderId"]); 
                                string name = r["Name"] as string ?? "";
                                string title = r["Title"] as string ?? "";
                                folder[id] = (name, title);
                            }
                        }
                    }

                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT BookInstanceId, Title,'' as Name FROM dbo.D_BookInstance";
                        using (var r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                string id = Convert.ToString(r["BookInstanceId"]);
                                string name = r["Name"] as string ?? "";
                                string title = r["Title"] as string ?? "";
                                book[id] = (name, title);
                            }
                        }
                    }

                }

                SiteTitleById = site;
                FolderTitleById = folder;
                BookTitleById = book;
            }
        }

        public static string GetDocumentTitle(int documentInstanceId, string connectionString)
        {
            string title = null;
            string query = @"SELECT Title 
                         FROM [dbo].D_DocumentInstance 
                         WHERE DocumentInstanceId = @DocumentInstanceId";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@DocumentInstanceId", documentInstanceId);

                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            title = result.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // log error here if needed
                throw new Exception("Error fetching document title", ex);
            }

            return title ?? string.Empty;
        }
        public class TableInfo
        {
            public string Name { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
        }
    }

}