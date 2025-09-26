using Microsoft.Extensions.Configuration;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Caching;

namespace HtmlIndexerElastic.Util
{
    public interface ISqlDestroyerService
    {
        // Returns (name, title) for a given node type/id.
        // name: machine-ish (slug), title: friendly label for UI.
        (string Name, string Title) Get(string type, string id);
    }
   

    public sealed class SqlDestroyerService : ISqlDestroyerService
    {
        private readonly string _connStr;
        private readonly MemoryCache _cache = MemoryCache.Default;

        public SqlDestroyerService()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            _connStr = config["ConnectionStrings:DestroyerConnection"]; 
        }

        public (string Name, string Title) Get(string type, string id)
        {
            var key = $"taxname:{type}:{id}";
            var cached = _cache.Get(key) as Tuple<string, string>;
            if (cached != null) return (cached.Item1, cached.Item2);

            (string Name, string Title) row = (null, null);

            using (var cn = new SqlConnection(_connStr))
            using (var cmd = cn.CreateCommand())
            {
                cn.Open();

                switch (type)
                {
                    case "Site":
                        cmd.CommandText = "SELECT [Name],[Title] FROM dbo.D_Site WHERE SiteId=@id";
                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = int.Parse(id);
                        break;

                    case "SiteFolder":
                        cmd.CommandText = "SELECT [Name],[Title] FROM dbo.D_SiteFolder WHERE FolderId=@id";
                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = int.Parse(id);
                        break;

                    case "Book":
                        // No Name column -> use Title for both (or override with HTML later)
                        cmd.CommandText = "SELECT [Title] FROM dbo.D_BookInstance WHERE BookInstanceId=@id";
                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = int.Parse(id);
                        break;

                    case "Document":
                        // No Name column -> use Title for both (or override with HTML later)
                        cmd.CommandText = "SELECT [Title] FROM dbo.D_DocumentInstance WHERE DocumentInstanceId=@id";
                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = int.Parse(id);
                        break;

                    default:
                        return (null, null);
                }

                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        if (type == "Site" || type == "SiteFolder")
                        {
                            row.Name = r["Name"] as string;
                            row.Title = r["Title"] as string;
                        }
                        else
                        {
                            var name = r["Title"] as string;
                            row.Name = name;
                            row.Title = name; // can override later with HTML-derived titles
                        }
                    }
                }
            }

            var ttl = DateTimeOffset.UtcNow.AddMinutes(10);
            _cache.Set(key, Tuple.Create(row.Name, row.Title), ttl);
            return row;
        }
    }
}
