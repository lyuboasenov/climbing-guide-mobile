using Climbing.Guide.Caching;
using System;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

namespace Climbing.Guide.Http {
   public class CachingHttpClient : HttpClient {

      private ICache ResponseCache { get; set; }
      private ICache LargeResponseCache { get; set; }

      public bool CacheEnabled { get; set; }
      public TimeSpan CachePeriod { get; set; }

      public CachingHttpClient(ICache responseCache, ICache largeResponseCache) {
         ResponseCache = responseCache;
         LargeResponseCache = largeResponseCache;
      }

      public override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
         HttpResponseMessage response = null;

         if (!CacheEnabled) {
            response = await base.SendAsync(request, cancellationToken);
         } else {
            string requestUri = request.RequestUri.ToString();
            if (!ResponseCache.Contains(requestUri) && !LargeResponseCache.Contains(requestUri)) {
               response = await base.SendAsync(request, cancellationToken);
               CacheResponse(response);
            } else {
               response = GetResponseFromCache(requestUri);
            }
         }



         return response;
      }

      private HttpResponseMessage GetResponseFromCache(string requestUri) {
         throw new NotImplementedException();
      }

      private void CacheResponse(HttpResponseMessage response) {
         //BinaryFormatter bf = BinaryFormatter();

         //bf.



         //if ()
      }
   }
}
