using System;

namespace Climbing.Guide.Caching {
   public interface ICache {
      TimeSpan CleanInterval { get; set; }

      void Add<T>(string key, T data, TimeSpan expireIn, string tag = null);
      void Remove(params string[] key);

      void Invalidate();

      bool Contains(string key);
      T Get<T>(string key);

      string GetTag(string key);

      DateTime? GetExpiration(string key);
      void Refresh(string key, TimeSpan expireIn);

      long GetCacheSize();
   }
}
