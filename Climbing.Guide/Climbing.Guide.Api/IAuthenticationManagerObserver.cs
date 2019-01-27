namespace Climbing.Guide.Api {
   public interface IAuthenticationManagerObserver {
      void OnLogIn();
      void OnLogOut();
   }
}
