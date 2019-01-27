using System;

namespace Climbing.Guide.Forms.Services.Navigation {
   public interface INavigationRequest {
      bool HasParameters { get; }
      Uri GetNavigationUri();
      object GetParameters();
   }
}
