namespace Climbing.Guide.Core.Api {
   public interface IApiClientSettings {
      string BaseUrl { get; set; }
      string RefreshToken { get; set; }
      string Token { get; set; }
      string Username { get; set; }
   }
}