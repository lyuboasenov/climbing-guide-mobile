using Climbing.Guide.Api;
using System.Net.Http;

namespace Climbing.Guide.Core.Api {
   public interface IApiClientSettings {
      IAuthenticationManager AuthenticationManager { get; }
      HttpClient HttpClient { get; }
   }
}