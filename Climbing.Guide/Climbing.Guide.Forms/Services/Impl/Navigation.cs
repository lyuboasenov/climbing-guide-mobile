using Climbing.Guide.Collections;
using Climbing.Guide.Forms.Services.Progress;
using Climbing.Guide.Tasks;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.Services {
   public class Navigation : INavigation {

      private const int NAVIGATION_STACK_SIZE = 10;

      private FixedStack<NavigationItem> NavigationStack { get; set; }
      private INavigationService InternalNavigationService { get; set; }

      public Navigation(INavigationService navigationService) {
         InternalNavigationService = navigationService;
         NavigationStack = new FixedStack<NavigationItem>(NAVIGATION_STACK_SIZE);
      }

      public async Task<ITaskResult<bool>> GoBackAsync() {
         if (NavigationStack.Count > 0) {
            // Remove current and go to the previous page
            NavigationStack.Pop();
         }

         var navigationItem = NavigationStack.Count > 0 ? 
            NavigationStack.Peek() : 
            new NavigationItem() { Uri = GetShellNavigationUri(nameof(Views.HomeView)) };

         INavigationResult result;

         if (null != navigationItem.Parameters) {
            result = await InternalNavigationService.NavigateAsync(navigationItem.Uri, GetParameters(navigationItem.Parameters));
         } else {
            result = await InternalNavigationService.NavigateAsync(navigationItem.Uri);
         }
         return result.ToITaskResult();
      }

      public async Task<ITaskResult<bool>> GoBackAsync(params object[] parameters) {
         if (NavigationStack.Count > 0) {
            // Remove current and go to the previous page
            NavigationStack.Pop();
         }

         var navigationItem = NavigationStack.Count > 0 ?
            NavigationStack.Peek() :
            new NavigationItem() { Uri = GetShellNavigationUri(nameof(Views.HomeView)) };

         var result = await InternalNavigationService.NavigateAsync(navigationItem.Uri, GetParameters(parameters));
         return result.ToITaskResult();
      }

      public async Task<ITaskResult<bool>> NavigateAsync(Uri uri) {
         var result = await InternalNavigationService.NavigateAsync(uri);

         if (result.Success) {
            NavigationStack.Push(new NavigationItem() {
               Uri = uri
            });
         }

         return result.ToITaskResult();
      }

      public async Task<ITaskResult<bool>> NavigateAsync(Uri uri, params object[] parameters) {
         var result = await InternalNavigationService.NavigateAsync(uri, GetParameters(parameters));

         if (result.Success) {
            NavigationStack.Push(new NavigationItem() {
               Uri = uri,
               Parameters = parameters
            });
         }

         return result.ToITaskResult();
      }
      
      public Uri GetNavigationUri(string absolutePath) {
         return Helpers.UriHelper.Get(Helpers.UriHelper.Schema.nav, absolutePath);
      }

      public Uri GetShellNavigationUri(string relativePath) {
         return Helpers.UriHelper.Get(Helpers.UriHelper.Schema.nav, $"{nameof(Views.Shell)}/NavigationPage/{relativePath}");
      }

      private INavigationParameters GetParameters(params object[] parameters) {
         var result = new NavigationParameters();
         for(int i = 0; i <parameters.Length; i++) {
            result.Add(i.ToString(), parameters[i]);
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

   public class NavigationItem {
      public Uri Uri { get; set; }
      public object[] Parameters { get; set; }
   }
}
