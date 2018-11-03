namespace Climbing.Guide.Core.Api {
   public class ApiClientSettings : IApiClientSettings {
      public string BaseUrl { get; set; }

      public string Token { get; set; }
      public string RefreshToken { get; set; }
      public string Username { get; set; }
   }
}
