namespace Climbing.Guide.Forms.Services.Navigation.Generic {
   public class NavigationRequest<TParameters> : NavigationRequest, INavigationRequest<TParameters> {
      public new bool HasParameters => true;

      private TParameters Parameters { get; }

      public NavigationRequest(string path, TParameters parameters, INavigationRequest childNavigationRequest = null) :
         base(path, childNavigationRequest) {
         Parameters = parameters;
      }

      public new TParameters GetParameters() {
         return Parameters;
      }

      object INavigationRequest.GetParameters() {
         return Parameters;
      }
   }
}
