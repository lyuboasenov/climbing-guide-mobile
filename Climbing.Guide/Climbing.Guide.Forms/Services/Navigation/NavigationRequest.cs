using System;

namespace Climbing.Guide.Forms.Services.Navigation {
   public interface NavigationRequest {
      bool HasParameters { get; }
      Uri GetNavigationUri();
      object GetParameters();
   }
}
