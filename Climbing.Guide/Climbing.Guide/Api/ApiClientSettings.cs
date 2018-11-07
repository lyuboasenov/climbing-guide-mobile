namespace Climbing.Guide.Core.Api {
   public class ApiClientSettings : IApiClientSettings {
      public System.Net.Http.HttpClient HttpClient { get; set; }

      public string Token { get; set; }
      public string RefreshToken { get; set; }
      public string Username { get; set; }
   }
}
