using Alat.Caching;
using Alat.Caching.Serialization;
using SQLite;
using System;
using System.IO;

namespace Climbing.Guide.Caching.Sqlite {
   public class SqliteCacheRepository : CacheRepository {

      private Serializer Serializer { get; set; }
      private readonly object dblock = new object();
      private SQLiteConnection DB { get; set; }
      private string DbFilePath { get; set; }
      private CacheSettings CacheSettings { get; set; }

      public SqliteCacheRepository(CacheSettings settings, Serializer serializer) {
         CacheSettings = settings;
         Serializer = serializer;

         DbFilePath = Path.Combine((string)CacheSettings.Location, "climbing.guide.cache.db");
         if (!Directory.Exists((string)CacheSettings.Location)) {
            Directory.CreateDirectory((string)CacheSettings.Location);
         }

         DB = new SQLiteConnection(DbFilePath);
         DB.CreateTable<SqliteCacheItem>();

         Clean();
      }

      public void Add(string key, Stream content, string tag, DateTime expireAt) {
         using (MemoryStream memoryStream = new MemoryStream()) {
            content.CopyTo(memoryStream);
            var ent = new SqliteCacheItem {
               Key = key,
               ExpirationDate = expireAt,
               Tag = tag,
               RawContent = memoryStream.ToArray()
            };

            lock (dblock) {
               DB.InsertOrReplace(ent);
            }
         }
         
      }

      public void Clean() {
         lock (dblock) {
            var entries = DB.Query<SqliteCacheItem>($"SELECT * FROM SqliteCacheItem WHERE ExpirationDate < ?", DateTime.UtcNow.Ticks);
            DB.RunInTransaction(() => {
               foreach (var k in entries)
                  DB.Delete<SqliteCacheItem>(k.Key);
            });
         }
      }

      public bool Contains(string key) {
         return DB.Find<SqliteCacheItem>(key) != null;
      }

      public bool IsEmpty() {
         lock (dblock) {
            return DB.Table<SqliteCacheItem>().Count() == 0;
         }
      }

      public CacheItem Find(string key) {
         SqliteCacheItem item;
         lock (dblock) {
            item = DB.Find<SqliteCacheItem>(key);
         }
         if (null != item) {
            item.Content = new MemoryStream(item.RawContent);
            item.Content.Seek(0, SeekOrigin.Begin);
            item.RawContent = null;
         }

         return item;
      }

      public void Reset(string key, DateTime expireAt) {
         var item = Find(key);
         item.ExpirationDate = expireAt;
         lock (dblock) {
            DB.Update(item);
         }
      }

      public void Remove(string[] key) {
         lock (dblock) {
            DB.RunInTransaction(() => {
               foreach (var k in key) {
                  if (string.IsNullOrWhiteSpace(k))
                     continue;

                  DB.Delete<SqliteCacheItem>(primaryKey: k);
               }
            });
         }
      }

      public void RemoveAll() {
         lock (dblock) {
            DB.DeleteAll<SqliteCacheItem>();
         }
      }

      public long GetSize() {
         return new FileInfo(DbFilePath).Length;
      }
   }
}
