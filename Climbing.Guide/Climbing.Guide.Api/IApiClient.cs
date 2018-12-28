using Climbing.Guide.Api;
using Climbing.Guide.Api.Schemas;
using System;
using System.Threading.Tasks;

namespace Climbing.Guide.Core.Api {
   public interface IApiClient {
      IAuthenticationManager AuthenticationManager { get; }

      IAreasClient AreasClient { get; }
      IRoutesClient RoutesClient { get; }
      IUsersClient UsersClient { get; }
      IGradesClient GradesClient { get; }
      ILanguagesClient LanguagesClient { get; }

      Task DownloadAsync(Uri uri, string localPath, bool overwrite = false);
      void UpdateApiClientSettings(IApiClientSettings settings);
   }
}