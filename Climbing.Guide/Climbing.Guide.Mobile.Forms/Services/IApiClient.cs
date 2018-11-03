using System;
using System.Threading.Tasks;

namespace Climbing.Guide.Mobile.Forms.Services {
   public interface IApiClient : Core.Api.IApiClient {
      Task<string> DownloadRouteSchemaAsync(int routeId, Uri schemaUri);
      Task<string> DownloadRouteSchemaThumbAsync(int routeId, Uri schemaThumbUri);
   }
}