﻿using System;
using System.Threading.Tasks;

namespace Climbing.Guide.Mobile.Common.Services {
   public interface IRestApiClient : Core.API.IRestApiClient {
      Task<string> DownloadRouteSchemaAsync(int routeId, Uri schemaUri);
      Task<string> DownloadRouteSchemaThumbAsync(int routeId, Uri schemaThumbUri);
   }
}