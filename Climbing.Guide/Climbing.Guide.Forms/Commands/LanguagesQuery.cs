using Alat.Caching;
using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Collections.ObjectModel;
using Climbing.Guide.Core.Api;
using Climbing.Guide.Forms.Commands.Generics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.Commands {
   internal class LanguagesQuery : IAsyncQuery<IEnumerable<Language>> {
      private static readonly string LANGUAGES_CACHE_KEY =
         Helpers.UriHelper.Get(Helpers.UriHelper.Schema.cache, "/system/languages").ToString();

      public bool Force { get; set; }

      private IApiClient ApiClient { get; }
      private ICache Cache { get; }

      public LanguagesQuery(IApiClient apiClient, ICache cache) {
         ApiClient = apiClient;
         Cache = cache;
      }

      public async Task<IEnumerable<Language>> GetResultAsync() {
         IEnumerable<Language> result = null;

         if (GetFreshData()) {
            result = await ApiClient.LanguagesClient.ListAsync();
            Cache.Store(LANGUAGES_CACHE_KEY, result, TimeSpan.MaxValue);
         } else {
            result = Cache.Retrieve<ObservableCollection<Language>>(LANGUAGES_CACHE_KEY);
         }

         return result;
      }

      private bool GetFreshData() {
         return Force || !Cache.Contains(LANGUAGES_CACHE_KEY);
      }

      async Task<object> IAsyncQuery.GetResultAsync() {
         return await GetResultAsync();
      }
   }
}
