using Climbing.Guide.Caching;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Climbing.Guide.Http {
   public class CachingHttpClientHandler : HttpClientHandler {

      private ICache ResponseCache { get; set; }
      private ICache LargeResponseCache { get; set; }

      public bool CacheEnabled { get; set; } = true;
      public TimeSpan CachePeriod { get; set; }

      public CachingHttpClientHandler(ICache responseCache, ICache largeResponseCache) {
         ResponseCache = responseCache;
         LargeResponseCache = largeResponseCache;
      }

      protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
         HttpResponseMessage response = null;

         if (!CacheEnabled) {
            response = await base.SendAsync(request, cancellationToken);
         } else {
            string requestUri = request.RequestUri.ToString();
            if (!ResponseCache.Contains(requestUri) && !LargeResponseCache.Contains(requestUri)) {
               response = await base.SendAsync(request, cancellationToken);
               response.EnsureSuccessStatusCode();

               await CacheResponseAsync(requestUri, response);
               response = GetResponseFromCache(requestUri);
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
            cache.Add(requestUri, contentStream, CachePeriod);
         }
      }
   }
}
