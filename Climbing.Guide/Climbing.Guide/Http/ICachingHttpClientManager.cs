using System;

namespace Climbing.Guide.Http {
   public interface ICachingHttpClientManager {
      bool UseCache { get; set; }
      bool CacheResponses { get; set; }
      TimeSpan CachePeriod { get; set; }

      ICachingHttpSession CreateCacheSession();
      ICachingHttpSession CreateCacheSession(TimeSpan cachePeriod);
      void FinalizeCacheSession(ICachingHttpSession cachingHttpSession);
      void InvalidateCacheSession(ICachingHttpSession cachingHttpSession);
      string[] GetKeysToInvalidate();
   }
}
