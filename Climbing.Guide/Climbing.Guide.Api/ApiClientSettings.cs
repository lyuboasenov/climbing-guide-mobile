using System;
using System.Net.Http;
using Climbing.Guide.Api;

namespace Climbing.Guide.Core.Api {
   public class ApiClientSettings : IApiClientSettings {
      public HttpClient HttpClient { get; private set; }
      public IAuthenticationManager AuthenticationManager { get; private set; }

      public ApiClientSettings(HttpClient httpClient, IAuthenticationManager authenticationManager) {
         HttpClient = httpClient;
         AuthenticationManager = authenticationManager;
      }
   }
}
