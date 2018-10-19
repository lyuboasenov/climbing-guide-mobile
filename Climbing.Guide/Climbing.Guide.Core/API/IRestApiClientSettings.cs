namespace Climbing.Guide.Core.API {
   public interface IRestApiClientSettings {
      string BaseUrl { get; set; }
      string RefreshToken { get; set; }
      string Token { get; set; }
      string Username { get; set; }
   }
}