using System;

namespace Climbing.Guide.Http {
   public interface ICachingHttpSession : IDisposable {
      string[] Keys { get; }
      TimeSpan CachePeriod { get; }
      void Invalidate();
      void AddKey(string key);
   }
}