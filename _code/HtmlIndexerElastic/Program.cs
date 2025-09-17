// .NET 6+ Console App: HtmlIndexerApp.cs
// This program reads HTML files from subfolders and indexes them to Elasticsearch with basic auth support

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Elastic.Clients.Elasticsearch.Nodes;
using Elastic.Clients.Elasticsearch;
using HtmlIndexerElastic.Cache;
using System.Threading.Channels;

namespace HtmlIndexerElastic
{
    class Program
    {
        static async Task Main(string[] args)
        {

            var config = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json", optional: false)
                 .Build();

            string rootFolderPath = config["SourceFolder"];
            string indexName = string.IsNullOrWhiteSpace(config["Elasticsearch:Index"]) ? "html_pages" : config["Elasticsearch:Index"];
            string mode = (config["Elasticsearch:Mode"] ?? string.Empty).ToLowerInvariant();
            string cachedJsonFilePath = config["CachedJsonFile"];
            if (!Directory.Exists(rootFolderPath))
            {
                Console.WriteLine(string.IsNullOrWhiteSpace(rootFolderPath)
                    ? "Error: Missing SourceFolder in appsettings.json"
                    : $"Directory not found: {rootFolderPath}");
                return;
            }

            // Decide which implementation to use and from where to read credentials/endpoints
            HtmlIndexerBase indexer;

            if (mode == "serverless")
            {
                var endpoint = config["Elasticsearch:Serverless:Endpoint"];
                var apiKey = config["Elasticsearch:Serverless:ApiKey"];

                if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(apiKey))
                {
                    Console.WriteLine("Error: Missing Serverless endpoint or ApiKey in appsettings.json");
                    return;
                }

                indexer = new ServerlessHtmlIndexer(endpoint, indexName, apiKey);
                Console.WriteLine("Mode: Serverless (API Key)");
            }
            else if (mode == "hosted")
            {
                var endpoint = config["Elasticsearch:Hosted:Endpoint"];
                var username = config["Elasticsearch:Hosted:Username"];
                var password = config["Elasticsearch:Hosted:Password"];

                if (string.IsNullOrWhiteSpace(endpoint))
                {
                    Console.WriteLine("Error: Missing Hosted endpoint in appsettings.json");
                    return;
                }

                indexer = new HostedHtmlIndexer(endpoint, indexName, username, password);
                Console.WriteLine("Mode: Hosted (Basic Auth)");
            }
            else
            {
                // Auto-detect: prefer serverless if ApiKey section looks present, else hosted
                var slEndpoint = config["Elasticsearch:Serverless:Endpoint"];
                var slApiKey = config["Elasticsearch:Serverless:ApiKey"];
                if (!string.IsNullOrWhiteSpace(slEndpoint) && !string.IsNullOrWhiteSpace(slApiKey))
                {
                    indexer = new ServerlessHtmlIndexer(slEndpoint, indexName, slApiKey);
                    Console.WriteLine("Mode: Serverless (API Key) [auto]");
                }
                else
                {
                    var hEndpoint = config["Elasticsearch:Hosted:Endpoint"];
                    var hUser = config["Elasticsearch:Hosted:Username"];
                    var hPass = config["Elasticsearch:Hosted:Password"];

                    if (string.IsNullOrWhiteSpace(hEndpoint))
                    {
                        Console.WriteLine("Error: Could not determine mode. Set Elasticsearch:Mode or provide Serverless/Hosted config sections.");
                        return;
                    }

                    indexer = new HostedHtmlIndexer(hEndpoint, indexName, hUser, hPass);
                    Console.WriteLine("Mode: Hosted (Basic Auth) [auto]");
                }
            }

            try
            {
                // await indexer.EnsureIndexAsync();
                var hasher = new FileHasher(cachedJsonFilePath);
                Console.WriteLine("> Started to check with files modified");
                var changedFiles = hasher.GetChangedFiles(rootFolderPath);
                var deletedFiles = hasher.DeletedFiles;
                Console.WriteLine($"> Found {changedFiles?.Count()} changed files.");
                if (changedFiles?.Count() == 0 && deletedFiles.Count==0)
                {
                    Console.WriteLine("✅ No files changed. Skipping indexing.");
                    return;
                }
                Console.WriteLine("> Indexing Started");
                if(changedFiles?.Count() > 0)
                {
                    foreach (var file in changedFiles)
                    {
                        await indexer.IndexHtmlAsync(file);
                    }
                }

                if (deletedFiles?.Count() > 0)
                {
                    foreach (var (path, docId) in hasher.DeletedFiles)
                    {
                        await indexer.DeleteFromElasticAsync(docId);
                    }
                     
                }


                if (hasher.HasChanges)
                    hasher.Save();
                Console.WriteLine("> Indexing complete.");

                //var htmlFiles = Directory.GetFiles(rootFolderPath, "*.html", SearchOption.AllDirectories);
                //Console.WriteLine($"Found {htmlFiles.Length} HTML files to index.");

                //foreach (var file in htmlFiles)
                //{
                //    await indexer.IndexHtmlAsync(file);
                //}
            }
            finally
            {
                indexer.Dispose();
            }
        }


    }
}
