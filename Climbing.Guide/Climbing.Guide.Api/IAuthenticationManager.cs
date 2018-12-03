using System.Net.Http;
using System.Threading.Tasks;

namespace Climbing.Guide.Api {
   public interface IAuthenticationManager {
      string Username { get; }
      bool IsLoggedIn { get; }

      void SetCredentials(HttpRequestMessage request);

      Task<bool> LoginAsync(string username, string password);
      Task LogoutAsync();
   }
}
