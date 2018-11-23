using System;
using System.Collections.Generic;

namespace Climbing.Guide.Http {
   public class CachingHttpSession : ICachingHttpSession {

      private ICachingHttpClientManager CachingHttpClientManager { get; set; }
      private bool disposed = false;
      private List<string> KeysInternal { get; set; } = new List<string>();
      public string[] Keys { get { return KeysInternal.ToArray(); } }
      public TimeSpan CachePeriod { get; private set; }

      internal CachingHttpSession(ICachingHttpClientManager cachingHttpClientManager, TimeSpan cachePeriod) {
         CachingHttpClientManager = cachingHttpClientManager;
         CachePeriod = cachePeriod;
      }

      ~CachingHttpSession() {
         Dispose(false);
      }

      public void Dispose() {
         // Dispose of unmanaged resources.
         Dispose(true);
         // Suppress finalization.
         GC.SuppressFinalize(this);
      }

      protected virtual void Dispose(bool disposing) {
         if (!disposed) {

            if (disposing) {
               // Free managed resource
               KeysInternal = null;
            }

            CachingHttpClientManager.FinalizeCacheSession(this);

            disposed = true;
         }
      }

      public void Invalidate() {
         if(disposed) {
            throw new InvalidOperationException("Caching session have already been disposed.");
         }

         CachingHttpClientManager.InvalidateCacheSession(this);
         KeysInternal.Clear();
      }

      public void AddKey(string key) {
         KeysInternal.Add(key);
      }
   }
}