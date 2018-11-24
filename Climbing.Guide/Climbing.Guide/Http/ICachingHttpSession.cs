using System;

namespace Climbing.Guide.Http {
   public interface ICachingHttpSession : IDisposable {
      string[] Keys { get; }
      TimeSpan CachePeriod { get; }
      void Commit();
      void Invalidate();
      void AddKey(string key);
   }
}