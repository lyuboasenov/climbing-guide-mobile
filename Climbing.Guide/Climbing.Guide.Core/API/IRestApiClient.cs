using Climbing.Guide.Core.API.Schemas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Climbing.Guide.Core.API {
   public interface IRestApiClient {
      IRegionsClient RegionsClient { get; }
      IAreasClient AreasClient { get; }
      ISectorsClient SectorsClient { get; }
      IRoutesClient RoutesClient { get; }
      IRegisterClient RegisterClient { get; }

      Task<bool> LoginAsync(string username, string password);
   }
}
