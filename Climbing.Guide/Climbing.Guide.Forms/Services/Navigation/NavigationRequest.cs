using System;

namespace Climbing.Guide.Forms.Services.Navigation {
   public class NavigationRequest : INavigationRequest {
      public bool HasParameters => false;
      protected INavigationRequest ChildNavigationRequest { get; }
      private string Path { get; }

      public NavigationRequest(string path, INavigationRequest childNavigationRequest = null) {
         Path = path;
         ChildNavigationRequest = childNavigationRequest;
      }

      public Uri GetNavigationUri() {
         string childPath = string.Empty;
         string leadingAbsolutePath = $"{nameof(Views.Shell)}/";

         if (null != ChildNavigationRequest) {
            // Child path without the leading absolute path
            childPath = $"/{ChildNavigationRequest.GetNavigationUri().AbsolutePath.Substring(leadingAbsolutePath.Length)}";
         }

         return Helpers.UriHelper.Get(Helpers.UriHelper.Schema.nav, $"{leadingAbsolutePath}{Path}{childPath}");
      }

      public object GetParameters() {
         throw new ArgumentNullException(nameof(HasParameters));
      }
   }
}
