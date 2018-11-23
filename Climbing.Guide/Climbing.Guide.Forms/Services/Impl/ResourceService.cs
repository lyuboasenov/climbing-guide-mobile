using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Caching;
using Climbing.Guide.Core.Api;

namespace Climbing.Guide.Forms.Services {
   public class ResourceService : IResourceService {
      private IApiClient ApiClient { get; set; }
      private ICache Cache { get; set; }
      private Http.ICachingHttpClientManager CachingHttpClientManager { get; set; }

      public ResourceService(IApiClient apiClient, ICache cache, Http.ICachingHttpClientManager cachingHttpClientManager) {
         ApiClient = apiClient;
         Cache = cache;
         CachingHttpClientManager = cachingHttpClientManager;
      }

      public async Task<ObservableCollection<Region>> GetRegionsAsync() {
         using (var cachingSession = CachingHttpClientManager.CreateCacheSession(TimeSpan.FromMinutes(1))) {
            try {
               return await ApiClient.RegionsClient.ListAsync();
            } catch(ApiCallException ex) {
               cachingSession.Invalidate();
               throw;
            }
         }
      }

      public async Task<ObservableCollection<Grade>> GetGradeSystemAsync(int gradeSystemId, bool force = false) {
         string GRADE_SYSTEM_CACHE_KEY = Helpers.UriHelper.Get(Helpers.UriHelper.Schema.cache, $"/system/grades/system/{gradeSystemId}").ToString();
         ObservableCollection<Grade> result = null;
         if (force || !Cache.Contains(GRADE_SYSTEM_CACHE_KEY)) {
            result = await ApiClient.GradesClient.ReadAsync(gradeSystemId.ToString());
            Cache.Add(GRADE_SYSTEM_CACHE_KEY, result, TimeSpan.MaxValue);
         }
         if (null == result) {
            result = Cache.Get<ObservableCollection<Grade>>(GRADE_SYSTEM_CACHE_KEY);
         }

         return result;
      }

      private string GRADE_SYSTEMS_CACHE_KEY = Helpers.UriHelper.Get(Helpers.UriHelper.Schema.cache, "/system/grades/systems").ToString();
      public async Task<ObservableCollection<GradeSystemList>> GetGradeSystemsAsync(bool force = false) {
         ObservableCollection<GradeSystemList> result = null;
         if (force || !Cache.Contains(GRADE_SYSTEMS_CACHE_KEY)) {
            result = await ApiClient.GradesClient.ListAsync();
            Cache.Add(GRADE_SYSTEMS_CACHE_KEY, result, TimeSpan.MaxValue);
         }
         if (null == result) {
            result = Cache.Get<ObservableCollection<GradeSystemList>>(GRADE_SYSTEMS_CACHE_KEY);
         }

         return result;
      }

      private string LANGUAGES_CACHE_KEY = Helpers.UriHelper.Get(Helpers.UriHelper.Schema.cache, "/system/languages").ToString();
      public async Task<ObservableCollection<Language>> GetLanguagesAsync(bool force = false) {
         ObservableCollection<Language> result = null;
         if (force || !Cache.Contains(LANGUAGES_CACHE_KEY)) {
            result = await ApiClient.LanguagesClient.ListAsync();
            Cache.Add(LANGUAGES_CACHE_KEY, result, TimeSpan.MaxValue);
         }
         if (null == result) {
            result = Cache.Get<ObservableCollection<Language>>(LANGUAGES_CACHE_KEY);
         }

         return result;
      }
   }
}
