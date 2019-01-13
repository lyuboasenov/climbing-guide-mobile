using System;

namespace Climbing.Guide.Forms.Services.Navigation.Generic {
   public interface NavigationRequest<out TParameter> : NavigationRequest {
      new TParameter GetParameters();
   }
}
