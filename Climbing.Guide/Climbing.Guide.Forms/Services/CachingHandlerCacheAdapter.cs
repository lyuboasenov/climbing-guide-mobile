using System;
using System.Collections.Generic;

namespace Climbing.Guide.Forms.Services {
   public class CachingHandlerCacheAdapter : Alat.Http.Caching.ICache {
      private Alat.Caching.ICache Cache { get; }

      public CachingHandlerCacheAdapter(Alat.Caching.ICache cache) {
         Cache = cache;
      }

      public bool Contains(string key) {
         return Cache.Contains(key);
      }

      public void Remove(string key) {
         Cache.Remove(key);
      }

      public void Remove(IEnumerable<string> keys) {
         Cache.Remove(keys);
      }

      public T Retrieve<T>(string key) {
         return Cache.Retrieve<T>(key);
      }

      public void Store<TData>(string key, TData data, TimeSpan expireIn, string tag = null) {
         Cache.Store<TData>(key, data, expireIn, tag);
      }
   }
}
