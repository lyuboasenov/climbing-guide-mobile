using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Climbing.Guide.Caching.FileSystem {
   public class FileSystemCacheRepository : ICacheRepository {
      private const string ITEMS_DIRECTORY = "items";
      private readonly object lockObj = new object();
      private ICacheSettings CacheSettings { get; set; }
      private string IndexFilePath { get; set; }
      private IList<FileSystemCacheItem> Items { get; set; } = new List<FileSystemCacheItem>();

      public FileSystemCacheRepository(ICacheSettings settings) {
         CacheSettings = settings;

         IndexFilePath = Path.Combine(CacheSettings.Location, "climbing.guide.cache.index");
         if (!Directory.Exists(CacheSettings.Location)) {
            Directory.CreateDirectory(CacheSettings.Location);
         }
      }

      private void LoadIndex() {
         System.Xml.Serialization.XmlSerializer reader =
            new System.Xml.Serialization.XmlSerializer(typeof(IList<ICacheItem>));

         using (var indexFile = File.Open(IndexFilePath, FileMode.Open, FileAccess.Read)) {
            Items = (IList<FileSystemCacheItem>)reader.Deserialize(indexFile);
         }
      }

      private void SaveIndex() {
         System.Xml.Serialization.XmlSerializer writer =
            new System.Xml.Serialization.XmlSerializer(typeof(List<FileSystemCacheItem>));

         using (var indexFile = File.Open(IndexFilePath, FileMode.Create, FileAccess.Write)) {
            writer.Serialize(indexFile, Items);
         }
      }

      public void Add(string key, Stream content, string tag, DateTime expirationDate) {
         lock (lockObj) {
            var item = new FileSystemCacheItem() {
               Key = key,
               ExpirationDate = expirationDate,
               Tag = tag
            };

            var filePath = Path.Combine(CacheSettings.Location,
                  ITEMS_DIRECTORY,
                  key.GetHashCode().ToString());

            var fileInfo = new FileInfo(filePath);
            if (!fileInfo.Directory.Exists) {
               fileInfo.Directory.Create();
            }

            using (var file = 
               File.Open(filePath, FileMode.Create)) {
               content.CopyTo(file);
            }

            if (Contains(key)) {
               Refresh(key, expirationDate);
            } else {
               Items.Add(item);
               SaveIndex();
            }
         }
      }

      public void Clean() {
         lock (lockObj) {
            var itemsToRemove = Items.Where(i => i.ExpirationDate < DateTime.Now).ToArray();
            foreach (var item in itemsToRemove) {
               Items.Remove(item);
            }
            SaveIndex();
         }
      }

      public bool Contains(string key) {
         return Items.Count(i => i.Key.Equals(key, StringComparison.Ordinal)) > 0;
      }

      public long Count() {
         return Items.Count;
      }

      public ICacheItem Get(string key) {
         var item = Items.FirstOrDefault(i => i.Key.Equals(key, StringComparison.Ordinal));

         ICacheItem result = null;
         if(null != item) {
            result = new FileSystemCacheItem() {
               Key = item.Key,
               ExpirationDate = item.ExpirationDate,
               Tag = item.Tag,
               Content = File.OpenRead(Path.Combine(CacheSettings.Location, ITEMS_DIRECTORY, key.GetHashCode().ToString()))
            };
         }

         return result;
      }

      public void Refresh(string key, DateTime dateTime) {
         var item = Items.FirstOrDefault(i => i.Key.Equals(key, StringComparison.Ordinal));

         if(item != null) {
            lock (lockObj) {
               item.ExpirationDate = dateTime;
               SaveIndex();
            }
         }
      }

      public void Remove(string[] key) {
         lock (lockObj) {
            var itemsToRemove = Items.Where(i => key.Contains(i.Key)).ToArray();
            foreach(var item in itemsToRemove) {
               Items.Remove(item);
            }
            SaveIndex();
         }
      }

      public void RemoveAll() {
         lock (lockObj) {
            Items.Clear();
         }

         Directory.Delete(Path.Combine(CacheSettings.Location, ITEMS_DIRECTORY), true);
      }

      public long GetSize() {
         return Directory.GetFiles(CacheSettings.Location, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
      }
   }
}
