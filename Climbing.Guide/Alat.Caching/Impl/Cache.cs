using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Alat.Caching.Serialization;

namespace Alat.Caching.Impl {
   public class Cache : Caching.Cache {
      private const int DEFAULT_CLEANUP_TIME_MINUTES = 5;
      public TimeSpan CleanInterval { get; set; }

      private Serializer Serializer { get; set; }
      private CacheStore CacheStore { get; set; }
      private Timer CleanupTimer { get; set; }
      

      public Cache(CacheStore cacheStore, Serializer serializer) : 
         this(cacheStore, serializer, TimeSpan.FromMinutes(DEFAULT_CLEANUP_TIME_MINUTES)) { }

      public Cache(CacheStore cacheStore, Serializer serializer, TimeSpan cleanInterval) {
         CacheStore = cacheStore;
         Serializer = serializer;
         CleanupTimer = new Timer((state) => Clean());
         StartAutoClean();
      }

      public bool Contains(string key) {
         if (string.IsNullOrWhiteSpace(key)) {
            throw new ArgumentException("Key can not be null or empty.", nameof(key));
         }

         return CacheStore.Contains(key);
      }

      public T FindData<T>(string key) {
         if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key can not be null or empty.", nameof(key));

         CacheItem item = CacheStore.Find(key);

         T result = default(T);
         if (item != null) {
            if (typeof(Stream).GetTypeInfo().IsAssignableFrom(typeof(T).Ge‌​tTypeInfo())) {
               object boxed = item.Content;
               result = (T)boxed;
            } else {
               result = Serializer.Deserialize<T>(item.Content);
            }
         }

         return result;
      }

      public string FindTag(string key) {
         if (string.IsNullOrWhiteSpace(key)) {
            throw new ArgumentException("Key can not be null or empty.", nameof(key));
         }

         var cacheItem = CacheStore.Find(key);
         string result = string.Empty;
         if (cacheItem != null) {
            result = cacheItem.Tag;
         }

         return result;
      }

      public void Add<T>(string key, T data, TimeSpan expireIn, string tag = null) {
         if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key can not be null or empty.", nameof(key));

         if (data == null)
            throw new ArgumentNullException("Data can not be null.", nameof(data));

         Stream objectStream = data as Stream;
         try {
            if (null == objectStream) {
               objectStream = new MemoryStream();
               Serializer.Serialize(objectStream, data);
               objectStream.Seek(0, SeekOrigin.Begin);
            }

            CacheStore.Add(key, objectStream, tag, GetExpiration(expireIn));

            StartAutoClean();
         } finally {
            if (null != objectStream) {
               objectStream.Close();
               objectStream.Dispose();
            }
         }
      }

      public void Remove(params string[] key) {
         if (null == key || key.Length == 0)
            throw new ArgumentException("Key can not be null or empty.", nameof(key));

         CacheStore.Remove(key);
      }

      public void Reset(string key, TimeSpan expireIn) {
         if (string.IsNullOrWhiteSpace(key)) {
            throw new ArgumentException("Key can not be null or empty.", nameof(key));
         }

         CacheStore.Reset(key, GetExpiration(expireIn));
      }

      public void Clean() {
         CacheStore.Clean();
         StartAutoClean();
      }

      public void Invalidate() {
         CacheStore.RemoveAll();
      }

      public long GetCacheSize() {
         return CacheStore.GetSize();
      }

      private static bool IsString<T>(T data) {
         var typeOf = typeof(T);
         if (typeOf.IsGenericType && typeOf.GetGenericTypeDefinition() == typeof(Nullable<>)) {
            typeOf = Nullable.GetUnderlyingType(typeOf);
         }
         var typeCode = Type.GetTypeCode(typeOf);
         return typeCode == TypeCode.String;
      }

      private static DateTime GetExpiration(TimeSpan timeSpan) {
         var result = DateTime.MaxValue;
         if (timeSpan != TimeSpan.MaxValue) {
            result = DateTime.UtcNow.Add(timeSpan);
         }
         return result;
      }

      private void StartAutoClean() {
         if (!CacheStore.Any()) {
            int dueTime = CleanInterval.Milliseconds == 0 ? Timeout.Infinite : CleanInterval.Milliseconds;
            CleanupTimer.Change(dueTime, Timeout.Infinite);
         }
      }
   }
}
