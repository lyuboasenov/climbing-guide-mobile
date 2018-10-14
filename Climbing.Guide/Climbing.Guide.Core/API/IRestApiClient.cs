﻿using Climbing.Guide.Core.API.Schemas;
using System.Threading.Tasks;

namespace Climbing.Guide.Core.API {
   public interface IRestApiClient {
      IRegionsClient RegionsClient { get; }
      IAreasClient AreasClient { get; }
      ISectorsClient SectorsClient { get; }
      IRoutesClient RoutesClient { get; }
      IUsersClient UsersClient { get; }

      string Username { get; }
      string Token { get; }
      string RefreshToken { get; }

      Task<bool> LoginAsync(string username, string password);
      Task<bool> LogoutAsync();
   }
}
