using System;
using System.Collections.Generic;
using Climbing.Guide.Collections.ObjectModel;
using System.Threading.Tasks;
using Climbing.Guide.Api.Schemas;
using Climbing.Guide.Core.Api;
using Alat.Caching;
using Alat.Http.Caching.Sessions;

namespace Climbing.Guide.Forms.Services {
   public class Resources : IResource {
      private IApiClient ApiClient { get; }
      private ICache Cache { get; }
      private SessionFactory SessionFactory { get; }
      private IExceptionHandler ExceptionHandler { get; }

      public Resources(IApiClient apiClient, ICache cache, SessionFactory sessionFactory, IExceptionHandler exceptionHandler) {
         ApiClient = apiClient;
         Cache = cache;
         SessionFactory = sessionFactory;
         ExceptionHandler = exceptionHandler;
      }

      public async Task<IEnumerable<Grade>> GetGradeSystemAsync(int gradeSystemId, bool force = false) {
         string GRADE_SYSTEM_CACHE_KEY = Helpers.UriHelper.Get(Helpers.UriHelper.Schema.cache, $"/system/grades/system/{gradeSystemId}").ToString();
         IEnumerable<Grade> result = null;
         if (force || !Cache.Contains(GRADE_SYSTEM_CACHE_KEY)) {
            result = await ApiClient.GradesClient.ReadAsync(gradeSystemId.ToString());
            Cache.Store(GRADE_SYSTEM_CACHE_KEY, result, TimeSpan.MaxValue);
         }
         if (null == result) {
            result = Cache.Retrieve<ObservableCollection<Grade>>(GRADE_SYSTEM_CACHE_KEY);
         }

         return result;
      }

      private readonly string GRADE_SYSTEMS_CACHE_KEY = Helpers.UriHelper.Get(Helpers.UriHelper.Schema.cache, "/system/grades/systems").ToString();
      public async Task<IEnumerable<GradeSystemList>> GetGradeSystemsAsync(bool force = false) {
         IEnumerable<GradeSystemList> result = null;
         if (force || !Cache.Contains(GRADE_SYSTEMS_CACHE_KEY)) {
            result = await ApiClient.GradesClient.ListAsync();
            Cache.Store(GRADE_SYSTEMS_CACHE_KEY, result, TimeSpan.MaxValue);
         }
         if (null == result) {
            result = Cache.Retrieve<ObservableCollection<GradeSystemList>>(GRADE_SYSTEMS_CACHE_KEY);
         }

         return result;
      }

      private readonly string LANGUAGES_CACHE_KEY = Helpers.UriHelper.Get(Helpers.UriHelper.Schema.cache, "/system/languages").ToString();
      public async Task<IEnumerable<Language>> GetLanguagesAsync(bool force = false) {
         IEnumerable<Language> result = null;
         if (force || !Cache.Contains(LANGUAGES_CACHE_KEY)) {
            result = await ApiClient.LanguagesClient.ListAsync();
            Cache.Store(LANGUAGES_CACHE_KEY, result, TimeSpan.MaxValue);
         }
         if (null == result) {
            result = Cache.Retrieve<ObservableCollection<Language>>(LANGUAGES_CACHE_KEY);
         }

         return result;
      }
   }
}
