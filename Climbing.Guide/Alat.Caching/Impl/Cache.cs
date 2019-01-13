using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Alat.Caching.Serialization;

namespace Alat.Caching.Impl {
   public class Cache : Caching.Cache {
      private Serializer Serializer { get; set; }
      private CacheRepository CacheRepository { get; set; }
      private Timer Timer { get; set; }
      public TimeSpan CleanInterval { get; set; }

      public Cache(CacheRepository cacheRepository, Serializer serializer) : 
         this(cacheRepository, serializer, TimeSpan.FromMinutes(5)) { }

      public Cache(CacheRepository cacheRepository, Serializer serializer, TimeSpan cleanInterval) {
         CacheRepository = cacheRepository;
         Serializer = serializer;
         Timer = new Timer((state) => Clean());
         StartAutoClean();
      }

      public bool Contains(string key) {
         if (string.IsNullOrWhiteSpace(key)) {
            throw new ArgumentException("Key can not be null or empty.", nameof(key));
         }

         return CacheRepository.Contains(key);
      }

      public T FindData<T>(string key) {
         if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key can not be null or empty.", nameof(key));

         CacheItem item = CacheRepository.Find(key);

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

         var cacheItem = CacheRepository.Find(key);
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

            CacheRepository.Add(key, objectStream, tag, GetExpiration(expireIn));

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

         CacheRepository.Remove(key);
      }

      public void Reset(string key, TimeSpan expireIn) {
         if (string.IsNullOrWhiteSpace(key)) {
            throw new ArgumentException("Key can not be null or empty.", nameof(key));
         }

         CacheRepository.Reset(key, GetExpiration(expireIn));
      }

      public void Clean() {
         CacheRepository.Clean();
         StartAutoClean();
      }

      public void Invalidate() {
         CacheRepository.RemoveAll();
      }

      public long GetCacheSize() {
         return CacheRepository.GetSize();
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
         if (!CacheRepository.IsEmpty()) {
            int dueTime = CleanInterval.Milliseconds == 0 ? Timeout.Infinite : CleanInterval.Milliseconds;
            Timer.Change(dueTime, Timeout.Infinite);
         }
      }
   }
}
