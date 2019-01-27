using Climbing.Guide.Collections;
using Climbing.Guide.Forms.Services.Navigation.Exceptions;
using Prism.Navigation;
using System;
using System.Threading.Tasks;

namespace Climbing.Guide.Forms.Services.Navigation {
   public class Navigation : INavigation {

      private const int NAVIGATION_STACK_SIZE = 10;
      private const string NAVIGATION_PARAMETERS_KEY = "PARAMETERS";

      private FixedStack<INavigationRequest> NavigationStack { get; set; }
      private INavigationService InternalNavigationService { get; set; }

      public Navigation(INavigationService navigationService) {
         InternalNavigationService = navigationService;
         NavigationStack = new FixedStack<INavigationRequest>(NAVIGATION_STACK_SIZE);
      }

      public Task GoBackAsync() {
         if (NavigationStack.Count > 0) {
            // Remove current and go to the previous page
            NavigationStack.Pop();
         }

         var request = NavigationStack.Count > 0 ? 
            NavigationStack.Pop() :
            ViewModels.HomeViewModel.GetNavigationRequest(this);

         return NavigateAsync(request);
      }

      public async Task NavigateAsync(INavigationRequest request) {
         if (null == request) {
            throw new ArgumentNullException(nameof(request));
         }

         var result = await InternalNavigateAsync(request);

         if (result.Success) {
            NavigationStack.Push(request);
         } else {
            throw new NavigationException($"Error navigating to {request.GetNavigationUri()}", result.Exception, request);
         }
      }

      public INavigationRequest GetNavigationRequest(string path, INavigationRequest baseRequest = null) {
         return new NavigationRequest(path, baseRequest);
      }

      public INavigationRequest GetNavigationRequest<TParameters>(string path, TParameters parameters, INavigationRequest baseRequest = null) {
         return new Generic.NavigationRequest<TParameters>(path, parameters, baseRequest);
      }

      private async Task<INavigationResult> InternalNavigateAsync(INavigationRequest request) {
         INavigationResult result = null;

         if (request.HasParameters) {
            result = await InternalNavigationService.NavigateAsync(
               request.GetNavigationUri(), GetParameters(request.GetParameters()));
         } else {
            result = await InternalNavigationService.NavigateAsync(request.GetNavigationUri());
         }

         return result;
      }

      private INavigationParameters GetParameters(object parameters) {
         var result = new NavigationParameters {
            { NAVIGATION_PARAMETERS_KEY, parameters }
         };

         return result;
      }
   }
}
