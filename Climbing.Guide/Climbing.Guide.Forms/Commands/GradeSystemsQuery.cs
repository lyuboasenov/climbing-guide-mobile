using Alat.Caching;
using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Collections.ObjectModel;
using Climbing.Guide.Core.Api;
using Climbing.Guide.Forms.Commands.Generics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.Commands {
   internal class GradeSystemsQuery : IAsyncQuery<IEnumerable<GradeSystemList>> {
      private static readonly string GRADE_SYSTEMS_CACHE_KEY =
         Helpers.UriHelper.Get(Helpers.UriHelper.Schema.cache, "/system/grades/systems").ToString();

      public bool Force { get; set; }

      private IApiClient ApiClient { get; }
      private ICache Cache { get; }

      public GradeSystemsQuery(IApiClient apiClient, ICache cache) {
         ApiClient = apiClient;
         Cache = cache;
      }

      public async Task<IEnumerable<GradeSystemList>> GetResultAsync() {
         IEnumerable<GradeSystemList> result;

         if (GetFreshData()) {
            result = await ApiClient.GradesClient.ListAsync();

            Cache.Store(GRADE_SYSTEMS_CACHE_KEY, result, TimeSpan.MaxValue);
         } else {
            result = Cache.Retrieve<ObservableCollection<GradeSystemList>>(GRADE_SYSTEMS_CACHE_KEY);
         }

         return result;
      }

      private bool GetFreshData() {
         return Force || !Cache.Contains(GRADE_SYSTEMS_CACHE_KEY);
      }

      async Task<object> IAsyncQuery.GetResultAsync() {
         return await GetResultAsync();
      }
   }
}
