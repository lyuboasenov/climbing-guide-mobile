using Alat.Caching;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Alat.Caching.FileSystem {
   public class FileSystemCacheStore : CacheStore {
      private const string ITEMS_DIRECTORY = "items";
      private readonly object lockObj = new object();
      private string IndexFilePath { get; }
      private string Location { get; }
      private IList<FileSystemCacheItem> Items { get; } = new List<FileSystemCacheItem>();

      public FileSystemCacheStore(Settings settings) {
         Location = settings.Location;
         IndexFilePath = Path.Combine(Location, "climbing.guide.cache.index");
         if (!Directory.Exists(Location)) {
            Directory.CreateDirectory(Location);
         }
      }

      public void Add(string key, Stream content, string tag, DateTime expirationDate) {
         lock (lockObj) {
            var item = new FileSystemCacheItem() {
               Key = key,
               ExpirationDate = expirationDate,
               Tag = tag
            };

            var filePath = Path.Combine(Location,
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
               Reset(key, expirationDate);
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

      public bool Any() {
         return Items.Count > 0;
      }

      public CacheItem Find(string key) {
         var item = Items.FirstOrDefault(i => i.Key.Equals(key, StringComparison.Ordinal));

         CacheItem result = null;
         if(null != item) {
            result = new FileSystemCacheItem() {
               Key = item.Key,
               ExpirationDate = item.ExpirationDate,
               Tag = item.Tag,
               Content = File.OpenRead(Path.Combine(Location, ITEMS_DIRECTORY, key.GetHashCode().ToString()))
            };
         }

         return result;
      }

      public void Reset(string key, DateTime dateTime) {
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

         Directory.Delete(Path.Combine(Location, ITEMS_DIRECTORY), true);
      }

      public long GetSize() {
         return Directory.GetFiles(Location, "*", SearchOption.AllDirectories).Sum(t => (new FileInfo(t).Length));
      }

      private void LoadIndex() {
         System.Xml.Serialization.XmlSerializer reader =
            new System.Xml.Serialization.XmlSerializer(typeof(IList<CacheItem>));

         using (var indexFile = File.Open(IndexFilePath, FileMode.Open, FileAccess.Read)) {
            Items.Clear();
            var loadedItems = (IList<FileSystemCacheItem>)reader.Deserialize(indexFile);
            foreach(var item in loadedItems) {
               Items.Add(item);
            }
         }
      }

      private void SaveIndex() {
         System.Xml.Serialization.XmlSerializer writer =
            new System.Xml.Serialization.XmlSerializer(typeof(List<FileSystemCacheItem>));

         using (var indexFile = File.Open(IndexFilePath, FileMode.Create, FileAccess.Write)) {
            writer.Serialize(indexFile, Items);
         }
      }

   }
}
