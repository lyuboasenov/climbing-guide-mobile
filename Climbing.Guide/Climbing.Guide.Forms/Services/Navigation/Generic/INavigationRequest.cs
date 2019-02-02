namespace Climbing.Guide.Forms.Services.Navigation.Generic {
   public interface INavigationRequest<out TParameter> : INavigationRequest {
      new TParameter GetParameters();
   }
}
