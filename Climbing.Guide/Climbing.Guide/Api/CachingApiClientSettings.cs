using Climbing.Guide.Caching;
using Climbing.Guide.Http;
using System;

namespace Climbing.Guide.Core.Api {
   public class CachingApiClientSettings : ApiClientSettings {

      private ICache ResponseCache { get; set; }
      private ICache LargeResponseCache { get; set; }
      private ICachingHttpClientManager CachingHttpClientManager { get; set; }

      public CachingApiClientSettings(
         ICachingHttpClientManager cachingHttpClientManager,
         ICache responseCache,
         ICache largeResponseCache,
         string baseUrl,
         string token = null, string refreshToken = null, string username = null) :
            base(baseUrl, token, refreshToken, username) {

         CachingHttpClientManager = cachingHttpClientManager;
         ResponseCache = responseCache;
         LargeResponseCache = largeResponseCache;

         CreateHttpClient();
      }

      protected override void CreateHttpClient() {
         var httpClientHandler = new CachingHttpClientHandler(CachingHttpClientManager, ResponseCache, LargeResponseCache);

         HttpClient = new System.Net.Http.HttpClient(httpClientHandler) {
            BaseAddress = new Uri(BaseUrl)
         };
      }
   }
}
