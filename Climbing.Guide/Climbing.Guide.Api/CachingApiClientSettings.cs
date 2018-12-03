using Climbing.Guide.Api;
using Climbing.Guide.Caching;
using Climbing.Guide.Http;
using System;
using System.Net.Http;

namespace Climbing.Guide.Core.Api {
   public class CachingApiClientSettings : IApiClientSettings {

      private ICache ResponseCache { get; set; }
      private ICache LargeResponseCache { get; set; }
      private ICachingHttpClientManager CachingHttpClientManager { get; set; }

      public IAuthenticationManager AuthenticationManager { get; private set; }

      public HttpClient HttpClient { get; private set; }

      public CachingApiClientSettings(
         ICachingHttpClientManager cachingHttpClientManager,
         ICache responseCache,
         ICache largeResponseCache,
         string baseUrl,
         IAuthenticationManager authenticationManager) {

         CachingHttpClientManager = cachingHttpClientManager;
         ResponseCache = responseCache;
         LargeResponseCache = largeResponseCache;
         AuthenticationManager = authenticationManager;

         InitializeHttpClient(baseUrl);
      }

      private void InitializeHttpClient(string baseUrl) {
         var httpClientHandler = new CachingHttpClientHandler(CachingHttpClientManager, ResponseCache, LargeResponseCache);

         HttpClient = new HttpClient(httpClientHandler) {
            BaseAddress = new Uri(baseUrl)
         };
      }
   }
}
