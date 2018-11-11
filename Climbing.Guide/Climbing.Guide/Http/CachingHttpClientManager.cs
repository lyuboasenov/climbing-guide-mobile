using System;

namespace Climbing.Guide.Http {
   public class CachingHttpClientManager : ICachingHttpClientManager {
      public bool UseCache { get; set; } = true;
      public TimeSpan CachePeriod { get; set; } = TimeSpan.FromMinutes(10);
      public bool CacheResponses { get; set; } = false;
   }
}
