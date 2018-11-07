using System.Net.Http;

namespace Climbing.Guide.Core.Api {
   public interface IApiClientSettings {
      string RefreshToken { get; set; }
      string Token { get; set; }
      string Username { get; set; }
      HttpClient HttpClient { get; set; }
   }
}