using Alat.Caching;
using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Collections.ObjectModel;
using Climbing.Guide.Core.Api;
using Climbing.Guide.Forms.Queries.Generics;
using Climbing.Guide.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.Queries {
   internal class GradeSystemQuery : IAsyncQuery<IEnumerable<Grade>>, IQuery<IEnumerable<Grade>> {
      public int GradeSystemId { get; set; }
      public bool Force { get; set; }

      private IApiClient ApiClient { get; }
      private ICache Cache { get; }
      private ISyncTaskRunner SyncTaskRunner { get; }

      public GradeSystemQuery(IApiClient apiClient, ICache cache, ISyncTaskRunner syncTaskRunner) {
         ApiClient = apiClient;
         Cache = cache;
         SyncTaskRunner = syncTaskRunner;
      }

      public async Task<IEnumerable<Grade>> GetResultAsync() {
         IEnumerable<Grade> result;

         if (GetFreshData()) {
            result = await ApiClient.GradesClient.ReadAsync(GradeSystemId.ToString());

            Cache.Store(GetCacheKey(), result, TimeSpan.MaxValue);
         } else {
            result = Cache.Retrieve<ObservableCollection<Grade>>(GetCacheKey());
         }

         return result;
      }

      private bool GetFreshData() {
         return Force || !Cache.Contains(GetCacheKey());
      }

      private string GetCacheKey() {
         return Helpers.UriHelper.Get(
            Helpers.UriHelper.Schema.cache, $"/system/grades/system/{GradeSystemId}").ToString();
      }

      async Task<object> IAsyncQuery.GetResultAsync() {
         return await GetResultAsync();
      }

      public IEnumerable<Grade> GetResult() {
         return SyncTaskRunner.RunSync(GetResultAsync);
      }

      object IQuery.GetResult() {
         return GetResult();
      }
   }
}
