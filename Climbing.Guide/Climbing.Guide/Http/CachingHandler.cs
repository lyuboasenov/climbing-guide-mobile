using Alat.Caching;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Climbing.Guide.Http {
   public class CachingHandler : DelegatingHandler {

      private ICachingHttpClientManager CachingHttpClientManager { get; set; }
      private Cache ResponseCache { get; set; }
      private Cache LargeResponseCache { get; set; }

      public CachingHandler(ICachingHttpClientManager cachingHttpClientManager,
         Cache responseCache,
         Cache largeResponseCache) : 
         this(cachingHttpClientManager, responseCache, largeResponseCache, new HttpClientHandler()) {
      }

      public CachingHandler(ICachingHttpClientManager cachingHttpClientManager,
         Cache responseCache,
         Cache largeResponseCache,
         HttpMessageHandler innerHandler) : base(innerHandler) {
         Initialize(cachingHttpClientManager, responseCache, largeResponseCache);
      }

      private void Initialize(ICachingHttpClientManager cachingHttpClientManager,
         Cache responseCache,
         Cache largeResponseCache) {
         CachingHttpClientManager = cachingHttpClientManager;
         ResponseCache = responseCache;
         LargeResponseCache = largeResponseCache;
      }

      protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, 
         CancellationToken cancellationToken) {
         HttpResponseMessage response = null;

         RemoveInvalidatedRequests();

         if (!CachingHttpClientManager.UseCache || request.Method != HttpMethod.Get) {
            response = await base.SendAsync(request, cancellationToken);
         } else {
            string requestUri = request.RequestUri.ToString();
            if (!ResponseCache.Contains(requestUri) && !LargeResponseCache.Contains(requestUri)) {
               response = await base.SendAsync(request, cancellationToken);
               response.EnsureSuccessStatusCode();

               if (CachingHttpClientManager.CacheResponses) {
                  CachingHttpClientManager.AddKey(requestUri);
                  await CacheResponseAsync(requestUri, response);
                  if (null != response) {
                     response.Dispose();
                  }
                  response = GetResponseFromCache(requestUri);
               }
            } else {
               response = GetResponseFromCache(requestUri);
            }
         }

         return response;
      }

      private void RemoveInvalidatedRequests() {
         var invalidKeys = CachingHttpClientManager.GetKeysToInvalidate();
         if(null != invalidKeys && invalidKeys.Length > 0) {
            ResponseCache.Remove(invalidKeys);
            LargeResponseCache.Remove(invalidKeys);
         }
      }

      private HttpResponseMessage GetResponseFromCache(string requestUri) {
         HttpResponseMessage response = null;
         Stream stream = null;

         if (ResponseCache.Contains(requestUri)) {
            stream = ResponseCache.FindData<Stream>(requestUri);
         } else if (LargeResponseCache.Contains(requestUri)) {
            stream = LargeResponseCache.FindData<Stream>(requestUri);
         }

         if (null != stream) {
            response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            response.Content = new StreamContent(stream);
         }

         return response;
      }

      private async Task CacheResponseAsync(string requestUri, HttpResponseMessage response) {
         using (var contentStream = await response.Content.ReadAsStreamAsync()) {
            Cache cache = ResponseCache;
            if ((response.Content.Headers.ContentLength ?? long.MaxValue) > 500000) {
               cache = LargeResponseCache;
            }
            cache.Add(requestUri, contentStream, CachingHttpClientManager.CachePeriod);
         }
      }
   }
}
