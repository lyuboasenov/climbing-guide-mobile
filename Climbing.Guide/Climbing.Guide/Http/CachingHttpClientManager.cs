using System;
using System.Collections.Generic;

namespace Climbing.Guide.Http {
   public class CachingHttpClientManager : ICachingHttpClientManager {
      public bool UseCache { get; set; } = true;
      public TimeSpan CachePeriodInternal { get; set; } = TimeSpan.FromMinutes(10);
      public TimeSpan CachePeriod {
         get {
            return Sessions.Count > 0 ? Sessions.Peek().CachePeriod : CachePeriodInternal;
         }
         set {
            CachePeriodInternal = value;
         }
      }

      private bool CacheResponseInternal { get; set; } = false;
      public bool CacheResponses {
         get {
            return Sessions.Count > 0 || CacheResponseInternal;
         }
         set {
            CacheResponseInternal = value;
         }
      }

      private List<string> InvalidKeys { get; } = new List<string>();

      private Stack<ICachingHttpSession> Sessions { get; set; } = new Stack<ICachingHttpSession>();
      
      public ICachingHttpSession CreateCacheSession() {
         return CreateCacheSession(CachePeriod);
      }

      public ICachingHttpSession CreateCacheSession(TimeSpan cachePeriod) {
         ICachingHttpSession session = new CachingHttpSession(this, cachePeriod);
         Sessions.Push(session);

         return session;
      }

      public string[] GetKeysToInvalidate() {
         var result = InvalidKeys.ToArray();
         InvalidKeys.Clear();

         return result;
      }

      public void FinalizeCacheSession(ICachingHttpSession cachingHttpSession) {
         ICachingHttpSession currentSession = null;
         // Pops untill the given session is found
         while (Sessions.Count > 0 && (currentSession = Sessions.Pop()) != cachingHttpSession) { }
      }

      public void InvalidateCacheSession(ICachingHttpSession cachingHttpSession) {
         InvalidKeys.AddRange(cachingHttpSession.Keys);
      }

      public void AddKey(string key) {
         if(Sessions.Count > 0) {
            Sessions.Peek().AddKey(key);
         }
      }
   }
}
