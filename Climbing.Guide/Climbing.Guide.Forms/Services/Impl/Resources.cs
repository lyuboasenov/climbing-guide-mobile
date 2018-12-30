using System;
using System.Collections.Generic;
using Climbing.Guide.Collections.ObjectModel;
using System.Threading.Tasks;
using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Caching;
using Climbing.Guide.Core.Api;
using Climbing.Guide.Exceptions;

namespace Climbing.Guide.Forms.Services {
   public class Resources : IResource {
      private IApiClient ApiClient { get; set; }
      private ICache Cache { get; set; }
      private Http.ICachingHttpClientManager CachingHttpClientManager { get; set; }
      private IExceptionHandler ExceptionHandler { get; set; }

      public Resources(IApiClient apiClient, ICache cache, Http.ICachingHttpClientManager cachingHttpClientManager, IExceptionHandler exceptionHandler) {
         ApiClient = apiClient;
         Cache = cache;
         CachingHttpClientManager = cachingHttpClientManager;
         ExceptionHandler = exceptionHandler;
      }

      //public async Task<IEnumerable<Region>> GetRegionsAsync() {
      //   ObservableCollection<Region> regions = new ObservableCollection<Region>();

      //   await LoadRegionsAsync(regions);

      //   return regions;
      //}

      //private async Task LoadRegionsAsync(ObservableCollection<Region> regions) {
      //   using (var cachingSession = CachingHttpClientManager.CreateCacheSession(TimeSpan.FromHours(1))) {
      //      int retry = 5;

      //      for (int page = 1; ; page++) {
      //         try {
      //            cachingSession.Commit();
      //            // Returned regions get added to the collection
      //            var pagedRegions = await ApiClient.RegionsClient.ListAsync(page: page);
      //            foreach(var region in pagedRegions.Results) {
      //               regions.Add(region);
      //            }

      //            // If no next page exists no more calls are made
      //            if (null == pagedRegions.Next) {
      //               break;
      //            }
      //         } catch (ApiCallException ex) {
      //            cachingSession.Invalidate();
      //            if (retry > 0) {
      //               retry--;
      //               page--;
      //               await Task.Delay(1000);
      //            } else {
      //               await ExceptionHandler.HandleAsync(ex);
      //               throw;
      //            }
      //         }
      //      }
      //   }
      //}

      public async Task<IEnumerable<Grade>> GetGradeSystemAsync(int gradeSystemId, bool force = false) {
         string GRADE_SYSTEM_CACHE_KEY = Helpers.UriHelper.Get(Helpers.UriHelper.Schema.cache, $"/system/grades/system/{gradeSystemId}").ToString();
         IEnumerable<Grade> result = null;
         if (force || !Cache.Contains(GRADE_SYSTEM_CACHE_KEY)) {
            result = await ApiClient.GradesClient.ReadAsync(gradeSystemId.ToString());
            Cache.Add(GRADE_SYSTEM_CACHE_KEY, result, TimeSpan.MaxValue);
         }
         if (null == result) {
            result = Cache.Get<ObservableCollection<Grade>>(GRADE_SYSTEM_CACHE_KEY);
         }

         return result;
      }

      private readonly string GRADE_SYSTEMS_CACHE_KEY = Helpers.UriHelper.Get(Helpers.UriHelper.Schema.cache, "/system/grades/systems").ToString();
      public async Task<IEnumerable<GradeSystemList>> GetGradeSystemsAsync(bool force = false) {
         IEnumerable<GradeSystemList> result = null;
         if (force || !Cache.Contains(GRADE_SYSTEMS_CACHE_KEY)) {
            result = await ApiClient.GradesClient.ListAsync();
            Cache.Add(GRADE_SYSTEMS_CACHE_KEY, result, TimeSpan.MaxValue);
         }
         if (null == result) {
            result = Cache.Get<ObservableCollection<GradeSystemList>>(GRADE_SYSTEMS_CACHE_KEY);
         }

         return result;
      }

      private readonly string LANGUAGES_CACHE_KEY = Helpers.UriHelper.Get(Helpers.UriHelper.Schema.cache, "/system/languages").ToString();
      public async Task<IEnumerable<Language>> GetLanguagesAsync(bool force = false) {
         IEnumerable<Language> result = null;
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
