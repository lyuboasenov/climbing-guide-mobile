using Climbing.Guide.Api.Schemas;
using System;
using System.Threading.Tasks;

namespace Climbing.Guide.Core.Api {
   public interface IApiClient {
      bool IsLoggedIn { get; }
      string RefreshToken { get; }
      string Token { get; }
      string Username { get; }

      IAreasClient AreasClient { get; }
      IRegionsClient RegionsClient { get; }
      IRoutesClient RoutesClient { get; }
      ISectorsClient SectorsClient { get; }
      IUsersClient UsersClient { get; }

      Task DownloadAsync(Uri uri, string localPath, bool overwrite = false);
      Task<bool> LoginAsync(string username, string password);
      Task<bool> LogoutAsync();
      void UpdateRestApiClientSettings(IApiClientSettings settings);
   }
}