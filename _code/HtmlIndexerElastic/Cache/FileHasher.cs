using HtmlIndexerElastic.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HtmlIndexerElastic.Cache
{
    public class FileHasher
    {
        private readonly string _cacheFile;
        private Dictionary<string, FileIndexInfo> _hashCache;
        public bool HasChanges { get; private set; } = false;
        public List<(string path, string docId)> DeletedFiles { get; private set; } = new();
        IndexerHelper indexerHelper = new IndexerHelper();
        public FileHasher(string cachePath = "./Cache/indexed_files_cache.json")
        {

            string exeDir = AppContext.BaseDirectory;
            string cacheDir = Path.Combine(exeDir, "Cache");

            if (!Directory.Exists(cacheDir))
                Directory.CreateDirectory(cacheDir);

            _cacheFile = Path.Combine(cachePath, "indexed_files_cache.json");

            if (File.Exists(_cacheFile) && new FileInfo(_cacheFile).Length > 0)
            {
                var json = File.ReadAllText(_cacheFile);
                _hashCache = JsonSerializer.Deserialize<Dictionary<string, FileIndexInfo>>(json)
                             ?? new Dictionary<string, FileIndexInfo>();
            }
            else
            {
                _hashCache = new Dictionary<string, FileIndexInfo>();
            }
        }

        public IEnumerable<string> GetChangedFiles(string folder)
        {
           
            var changed = new List<string>();
            var files = Directory.GetFiles(folder, "*.html", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var computedHash = ComputeHash(file);
                if (!_hashCache.ContainsKey(file) || _hashCache[file].Hash != computedHash)
                {
                    var docId = indexerHelper.GenerateDocIdFromPath(file);
                    changed.Add(file);
                    _hashCache[file] = new FileIndexInfo { Hash = computedHash, DocId = docId };
                    HasChanges = true;
                }
            }

            var deleted = _hashCache.Keys.Except(files).ToList();
            foreach (var path in deleted)
            {

                DeletedFiles.Add((path, _hashCache[path].DocId));
                _hashCache.Remove(path);
                HasChanges = true;
            }

            return changed.ToArray();

        }

        public void Save()
        {
        
            if (!HasChanges) return;

            var json = JsonSerializer.Serialize(_hashCache, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_cacheFile, json);

        }

        private string ComputeHash(string filePath)
        {
            using var sha = SHA256.Create();
            using var stream = File.OpenRead(filePath);
            var hashBytes = sha.ComputeHash(stream);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
    }

}
