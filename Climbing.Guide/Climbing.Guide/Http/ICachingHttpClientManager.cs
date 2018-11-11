using System;

namespace Climbing.Guide.Http {
   public interface ICachingHttpClientManager {
      bool UseCache { get; }
      bool CacheResponses { get; }
      TimeSpan CachePeriod { get; }
   }
}
