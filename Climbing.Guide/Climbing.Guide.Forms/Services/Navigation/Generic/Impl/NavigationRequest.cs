using System;

namespace Climbing.Guide.Forms.Services.Navigation.Generic.Impl {
   public class NavigationRequest<TParameters> : Services.Navigation.Impl.NavigationRequest, Generic.NavigationRequest<TParameters> {
      public new bool HasParameters => true;

      private TParameters Parameters { get; }

      public NavigationRequest(string path, TParameters parameters, NavigationRequest childNavigationRequest = null) :
         base(path, childNavigationRequest) {
         Parameters = parameters;
      }

      public new TParameters GetParameters() {
         return Parameters;
      }

      object NavigationRequest.GetParameters() {
         return Parameters;
      }
   }
}
