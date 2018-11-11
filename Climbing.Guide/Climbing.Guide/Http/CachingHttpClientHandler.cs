using Climbing.Guide.Caching;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Climbing.Guide.Http {
   public class CachingHttpClientHandler : HttpClientHandler {

      private ICachingHttpClientManager CachingHttpClientManager { get; set; }
      private ICache ResponseCache { get; set; }
      private ICache LargeResponseCache { get; set; }

      public CachingHttpClientHandler(ICachingHttpClientManager cachingHttpClientManager,
         ICache responseCache,
         ICache largeResponseCache) {
         CachingHttpClientManager = cachingHttpClientManager;
         ResponseCache = responseCache;
         LargeResponseCache = largeResponseCache;
      }

      protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
         HttpResponseMessage response = null;

         if (!CachingHttpClientManager.UseCache || request.Method != HttpMethod.Get) {
            response = await base.SendAsync(request, cancellationToken);
         } else {
            string requestUri = request.RequestUri.ToString();
            if (!ResponseCache.Contains(requestUri) && !LargeResponseCache.Contains(requestUri)) {
               response = await base.SendAsync(request, cancellationToken);
               response.EnsureSuccessStatusCode();

               if (CachingHttpClientManager.CacheResponses) {
                  await CacheResponseAsync(requestUri, response);
                  response = GetResponseFromCache(requestUri);
               }
            } else {
               response = GetResponseFromCache(requestUri);
            }
         }

         return response;
      }

      private HttpResponseMessage GetResponseFromCache(string requestUri) {
         HttpResponseMessage response = null;
         Stream stream = null;

         if (ResponseCache.Contains(requestUri)) {
            stream = ResponseCache.Get<Stream>(requestUri);
         } else if (LargeResponseCache.Contains(requestUri)) {
            stream = LargeResponseCache.Get<Stream>(requestUri);
         }

         if (null != stream) {
            response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            response.Content = new StreamContent(stream);
         }

         return response;
      }

      private async Task CacheResponseAsync(string requestUri, HttpResponseMessage response) {
         using (var contentStream = await response.Content.ReadAsStreamAsync()) {
            ICache cache = ResponseCache;
            if ((response.Content.Headers.ContentLength ?? long.MaxValue) > 500000) {
               cache = LargeResponseCache;
            }
            cache.Add(requestUri, contentStream, CachingHttpClientManager.CachePeriod);
         }
      }
   }
}
