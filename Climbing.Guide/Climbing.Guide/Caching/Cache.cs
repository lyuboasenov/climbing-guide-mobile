using System;
using System.Threading;
using Newtonsoft.Json;

namespace Climbing.Guide.Caching {
   public class Cache : ICache {

      private JsonSerializerSettings JsonSettings { get; } = new JsonSerializerSettings {
         ObjectCreationHandling = ObjectCreationHandling.Replace,
         ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
         TypeNameHandling = TypeNameHandling.All,
      };
      private ICacheRepository CacheRepository { get; set; }
      private Timer Timer { get; set; }
      public TimeSpan CleanInterval { get; set; }

      public Cache(ICacheRepository cacheRepository) {
         CacheRepository = cacheRepository;
         Timer = new Timer(Clean);
         StartAutoClean();
      }

      public void Add<T>(string key, T data, TimeSpan expireIn, string tag = null, JsonSerializerSettings jsonSerializationSettings = null) {
         if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key can not be null or empty.", nameof(key));

         if (data == null)
            throw new ArgumentNullException("Data can not be null.", nameof(data));

         var jsonData = string.Empty;
         if (IsString(data)) {
            jsonData = data as string;
         } else {
            jsonData = JsonConvert.SerializeObject(data, jsonSerializationSettings ?? JsonSettings);
         }

         CacheRepository.Add(key, jsonData, tag, GetExpiration(expireIn));

         StartAutoClean();
      }

      public bool Exists(string key) {
         if (string.IsNullOrWhiteSpace(key)) {
            throw new ArgumentException("Key can not be null or empty.", nameof(key));
         }

         return CacheRepository.Contains(key);
      }

      public T Get<T>(string key, JsonSerializerSettings jsonSerializationSettings = null) {
         if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key can not be null or empty.", nameof(key));

         var cacheItem = CacheRepository.Get(key);
         T result = default(T);

         if (cacheItem == null) {

         } else if (IsString(result)) {
            object final = cacheItem.Content;
            result = (T)final;
         } else {
            result = JsonConvert.DeserializeObject<T>(cacheItem.Content, jsonSerializationSettings ?? JsonSettings);
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
   }
}
