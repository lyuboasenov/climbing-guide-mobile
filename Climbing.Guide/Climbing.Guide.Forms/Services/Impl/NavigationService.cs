using Climbing.Guide.Tasks;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.Services {
   public class NavigationService : INavigationService {

      private Prism.Navigation.INavigationService InternalNavigationService { get; set; }
      private IProgressService ProgressService { get; set; }

      public NavigationService(Prism.Navigation.INavigationService navigationService, IProgressService progressService) {
         InternalNavigationService = navigationService;
         ProgressService = progressService;
      }

      public async Task<ITaskResult<bool>> GoBackAsync() {
         var result = await InternalNavigationService.GoBackAsync();
         return result.ToITaskResult();
      }

      public async Task<ITaskResult<bool>> GoBackAsync(params object[] parameters) {
         var result = await InternalNavigationService.GoBackAsync(GetParameters(parameters));
         return result.ToITaskResult();
      }

      public async Task<ITaskResult<bool>> NavigateAsync(Uri uri) {
         var result = await InternalNavigationService.NavigateAsync(uri);
         return result.ToITaskResult();
      }

      public async Task<ITaskResult<bool>> NavigateAsync(Uri uri, params object[] parameters) {
         var result = await InternalNavigationService.NavigateAsync(uri, GetParameters(parameters));
         return result.ToITaskResult();
      }
      
      public Uri GetNavigationUri(string absolutePath) {
         return Helpers.UriHelper.Get(Helpers.UriHelper.Schema.nav, absolutePath);
      }

      public Uri GetShellNavigationUri(string relativePath) {
         return Helpers.UriHelper.Get(Helpers.UriHelper.Schema.nav, $"{nameof(Views.Shell)}/NavigationPage/{relativePath}");
      }

      private INavigationParameters GetParameters(params object[] parameters) {
         Dictionary<Type, int> typeCount = new Dictionary<Type, int>();
         var result = new NavigationParameters();
         foreach(var parameter in parameters) {
            var type = parameter.GetType();
            if (!typeCount.ContainsKey(type)) {
               typeCount[type] = 0;
            }
            typeCount[type] += 1;
            result.Add(Helpers.UriHelper.GetTypeUri(type, typeCount[type]).ToString(), parameter);
         }

         return result;
      }
   }

   public static class INavigationResultExtension {
      public static ITaskResult<bool> ToITaskResult(this INavigationResult result) {
         return new NavigationResult() { Result = result.Success, Exception = result.Exception };
      }
   }

   public class NavigationResult : ITaskResult<bool> {
      public bool Result { get; set; }
      public Exception Exception { get; set; }
   }
}
