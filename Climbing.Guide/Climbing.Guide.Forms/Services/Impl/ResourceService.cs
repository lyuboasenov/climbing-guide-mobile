using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Caching;
using Climbing.Guide.Core.Api;
using Climbing.Guide.Services;

namespace Climbing.Guide.Forms.Services {
   public class ResourceService : IResourceService {
      private IApiClient ApiClient { get; set; }
      private ICache Cache { get; set; }
      private Http.ICachingHttpClientManager CachingHttpClientManager { get; set; }
      private IErrorService ErrorService { get; set; }

      public ResourceService(IApiClient apiClient, ICache cache, Http.ICachingHttpClientManager cachingHttpClientManager, IErrorService errorService) {
         ApiClient = apiClient;
         Cache = cache;
         CachingHttpClientManager = cachingHttpClientManager;
         ErrorService = errorService;
      }

      public async Task<ObservableCollection<Region>> GetRegionsAsync() {
         ObservableCollection<Region> regions = new ObservableCollection<Region>();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
         LoadRegionsAsync(regions);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

         return regions;
      }

      private async Task LoadRegionsAsync(ObservableCollection<Region> regions) {
         using (var cachingSession = CachingHttpClientManager.CreateCacheSession(TimeSpan.FromHours(1))) {
            int retry = 5;

            for (int page = 1; ; page++) {
               try {
                  cachingSession.Commit();
                  // Returned regions get added to the collection
                  var pagedRegions = await ApiClient.RegionsClient.ListAsync(page: page);
                  foreach(var region in pagedRegions.Results) {
                     regions.Add(region);
                  }

                  // If no next page exists no more calls are made
                  if (null == pagedRegions.Next) {
                     break;
                  }
               } catch (ApiCallException ex) {
                  cachingSession.Invalidate();
                  if (retry > 0) {
                     retry--;
                     page--;
                     await Task.Delay(1000);
                  } else {
                     await ErrorService.HandleApiCallExceptionAsync(ex);
                     throw;
                  }
               }
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
