using System;

namespace Climbing.Guide.Core.Api {
   public class ApiClientSettings : IApiClientSettings {
      public System.Net.Http.HttpClient HttpClient { get; set; }

      public string Token { get; set; }
      public string RefreshToken { get; set; }
      public string Username { get; set; }

      protected string BaseUrl { get; set; }

      public ApiClientSettings(string baseUrl, string token = null, string refreshToken = null, string username = null) {
         Username = username;
         Token = token;
         RefreshToken = refreshToken;

         BaseUrl = baseUrl;
      }

      protected virtual void CreateHttpClient() {
         HttpClient = new System.Net.Http.HttpClient() {
            BaseAddress = new Uri(BaseUrl)
         };
      }
   }
}
