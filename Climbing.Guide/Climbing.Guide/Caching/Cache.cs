using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Climbing.Guide.Serialization;

namespace Climbing.Guide.Caching {
   public class Cache : ICache {
      private ISerializer Serializer { get; set; }
      private ICacheRepository CacheRepository { get; set; }
      private Timer Timer { get; set; }
      public TimeSpan CleanInterval { get; set; }

      public Cache(ICacheRepository cacheRepository, ISerializer serializer) {
         CacheRepository = cacheRepository;
         Serializer = serializer;
         Timer = new Timer(Clean);
         StartAutoClean();
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

            CacheRepository.Add(key, objectStream, tag, GetExpiration(expireIn));

            StartAutoClean();
         } finally {
            if (null != objectStream) {
               objectStream.Close();
               objectStream.Dispose();
            }
         }
      }

      public bool Contains(string key) {
         if (string.IsNullOrWhiteSpace(key)) {
            throw new ArgumentException("Key can not be null or empty.", nameof(key));
         }

         return CacheRepository.Contains(key);
      }

      public T Get<T>(string key) {
         if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key can not be null or empty.", nameof(key));

         ICacheItem item = CacheRepository.Get(key);

         T result = default(T);
         if(item != null) {
            if (typeof(Stream).GetTypeInfo().IsAssignableFrom(typeof(T).Ge‌​tTypeInfo())) {
               object boxed = item.Content;
               result = (T)boxed;
            } else {
               result = Serializer.Deserialize<T>(item.Content);
            }
            
         }

         return result;
      }

      public DateTime? GetExpiration(string key) {
         if (string.IsNullOrWhiteSpace(key)) {
            throw new ArgumentException("Key can not be null or empty.", nameof(key));
         }

         var cacheItem = CacheRepository.Get(key);
         DateTime? result = null;

         if (cacheItem != null)
            result = cacheItem.ExpirationDate;

         return result;
      }

      public string GetTag(string key) {
         if (string.IsNullOrWhiteSpace(key)) {
            throw new ArgumentException("Key can not be null or empty.", nameof(key));
         }

         var cacheItem = CacheRepository.Get(key);
         string result = string.Empty;
         if (cacheItem != null) {
            result = cacheItem.Tag;
         }

         return result;
      }

      public void Invalidate() {
         CacheRepository.RemoveAll();
      }

      public void Refresh(string key, TimeSpan expireIn) {
         if (string.IsNullOrWhiteSpace(key)) {
            throw new ArgumentException("Key can not be null or empty.", nameof(key));
         }

         CacheRepository.Refresh(key, GetExpiration(expireIn));
      }

      public void Remove(params string[] key) {
         if (null == key || key.Length == 0)
            throw new ArgumentException("Key can not be null or empty.", nameof(key));

         CacheRepository.Remove(key);
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

      private void Clean(object state) {
         CacheRepository.Clean();
         StartAutoClean();
      }

      private void StartAutoClean() {
         if (CacheRepository.Count() > 0) {
            Timer.Change(CleanInterval.Milliseconds, -1);
         }
      }

      public long GetCacheSize() {
         return CacheRepository.GetSize();
      }
   }
}
